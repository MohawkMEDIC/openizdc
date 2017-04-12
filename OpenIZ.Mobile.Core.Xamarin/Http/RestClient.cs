/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-3-31
 */
using System;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Xamarin.Security;
using System.Security;
using System.IO.Compression;
using OpenIZ.Core.Http;
using System.IO;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenIZ.Core.Model.Query;
using System.Net.Security;

namespace OpenIZ.Mobile.Core.Xamarin.Http
{
    /// <summary>
    /// Represents an android enabled rest client
    /// </summary>
    public class RestClient : RestClientBase
    {

        // Config section
        private ServiceClientConfigurationSection m_configurationSection;

        // Tracer
        private Tracer m_tracer;

        // Trusted certificates
        private static HashSet<String> m_trustedCerts = new HashSet<String>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Xamarin.Http.RestClient"/> class.
        /// </summary>
        public RestClient() : base()
        {
            this.m_tracer = Tracer.GetTracer(this.GetType());
            this.m_configurationSection = ApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection>();

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Xamarin.Http.RestClient"/> class.
        /// </summary>
        public RestClient(ServiceClientDescription config) : base(config)
        {
            this.m_configurationSection = ApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection>();
            this.m_tracer = Tracer.GetTracer(this.GetType());
            // Find the specified certificate
            if (config.Binding.Security.ClientCertificate != null)
            {
                this.ClientCertificates = new X509Certificate2Collection();
                var cert = X509CertificateUtils.FindCertificate(config.Binding.Security.ClientCertificate.FindType,
                              config.Binding.Security.ClientCertificate.StoreLocation,
                              config.Binding.Security.ClientCertificate.StoreName,
                              config.Binding.Security.ClientCertificate.FindValue);
                if (cert == null)
                    throw new SecurityException(String.Format("Certificate described by {0} could not be found in {1}/{2}",
                        config.Binding.Security.ClientCertificate.FindValue,
                        config.Binding.Security.ClientCertificate.StoreLocation,
                        config.Binding.Security.ClientCertificate.StoreName));
                this.ClientCertificates.Add(cert);
            }
        }

        /// <summary>
        /// Create HTTP Request object
        /// </summary>
        protected override WebRequest CreateHttpRequest(string url, NameValueCollection query)
        {
            var retVal = (HttpWebRequest)base.CreateHttpRequest(url, query);

            // Certs?
            if (this.ClientCertificates != null)
                retVal.ClientCertificates.AddRange(this.ClientCertificates);

            // Compress?
            if (this.Description.Binding.Optimize)
                retVal.Headers.Add(HttpRequestHeader.AcceptEncoding, "deflate"); // use deflate as it appears to be a little faster

            // Proxy?
            if (!String.IsNullOrEmpty(this.m_configurationSection.ProxyAddress))
                retVal.Proxy = new WebProxy(this.m_configurationSection.ProxyAddress);

            retVal.ServerCertificateValidationCallback = this.RemoteCertificateValidation;

            return retVal;
        }

        /// <summary>
        /// Remote certificate validation errors
        /// </summary>
        private bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            lock (m_trustedCerts)
            {
                if (m_trustedCerts.Contains(certificate.Subject))
                    return true;
                else if (ApplicationContext.Current.Confirm(String.Format(Strings.locale_certificateValidation, certificate.Subject, certificate.Issuer)))
                {
                    m_trustedCerts.Add(certificate.Subject);
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Invokes the specified method against the url provided
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="url">URL.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="body">Body.</param>
        /// <param name="query">Query.</param>
        /// <typeparam name="TBody">The 1st type parameter.</typeparam>
        /// <typeparam name="TResult">The 2nd type parameter.</typeparam>
        protected override TResult InvokeInternal<TBody, TResult>(string method, string url, string contentType, WebHeaderCollection additionalHeaders, out WebHeaderCollection responseHeaders, TBody body, NameValueCollection query)
        {

            if (String.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));
            //if (String.IsNullOrEmpty(url))
            //    throw new ArgumentNullException(nameof(url));

            // Three times:
            // 1. With provided credential
            // 2. With challenge
            // 3. With challenge again
            for (int i = 0; i < 2; i++)
            {
                // Credentials provided ?
                HttpWebRequest requestObj = this.CreateHttpRequest(url, query) as HttpWebRequest;
                if (!String.IsNullOrEmpty(contentType))
                    requestObj.ContentType = contentType;
                requestObj.Method = method;

                // Additional headers
                if (additionalHeaders != null)
                    foreach (var hdr in additionalHeaders.AllKeys)
                    {
                        if (hdr == "If-Modified-Since")
                            requestObj.IfModifiedSince = DateTime.Parse(additionalHeaders[hdr]);
                        else
                            requestObj.Headers.Add(hdr, additionalHeaders[hdr]);
                    }

#if PERFMON
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif
                // Body was provided?
                try
                {

                    // Try assigned credentials
                    IBodySerializer serializer = null;
                    if (body != null)
                    {
                        // GET Stream, 
                        Stream requestStream = null;
                        Exception requestException = null;

                        try
                        {


                            //requestStream = requestObj.GetRequestStream();
                            var requestTask = requestObj.GetRequestStreamAsync().ContinueWith(r =>
                            {
                                if (r.IsFaulted)
                                    requestException = r.Exception.InnerExceptions.First();
                                else
                                    requestStream = r.Result;
                            }, TaskContinuationOptions.LongRunning);

                            if (!requestTask.Wait(this.Description.Endpoint[0].Timeout))
                            {
                                throw new TimeoutException();
                            }
                            else if (requestException != null) throw requestException;

                            if (contentType == null && typeof(TResult) != typeof(Object))
                                throw new ArgumentNullException(nameof(contentType));

                            serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(contentType, typeof(TBody));
                            // Serialize and compress with deflate
                            using (MemoryStream ms = new MemoryStream())
                            {
                                if (this.Description.Binding.Optimize)
                                {
                                    requestObj.Headers.Add("Content-Encoding", "deflate");
                                    using (var df = new DeflateStream(ms, CompressionMode.Compress))
                                        serializer.Serialize(df, body);
                                }
                                else
                                    serializer.Serialize(ms, body);

                                // Trace
                                if (this.Description.Trace)
                                    this.m_tracer.TraceVerbose("HTTP >> {0}", Convert.ToBase64String(ms.ToArray()));

                                using (var nms = new MemoryStream(ms.ToArray()))
                                    nms.CopyTo(requestStream);

                            }
                        }
                        finally
                        {
                            if (requestStream != null)
                                requestStream.Dispose();
                        }
                    }

                    // Response
                    HttpWebResponse response = null;
                    Exception responseError = null;

                    try
                    {
                        var responseTask = requestObj.GetResponseAsync().ContinueWith(r =>
                        {
                            if (r.IsFaulted)
                                responseError = r.Exception.InnerExceptions.First();
                            else
                                response = r.Result as HttpWebResponse;
                        }, TaskContinuationOptions.LongRunning);
                        if (!responseTask.Wait(this.Description.Endpoint[0].Timeout))
                            throw new TimeoutException();
                        else
                        {
                            if (responseError != null)
                            {
                                if (((responseError as WebException)?.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotModified)
                                {
                                    responseHeaders = response?.Headers;
                                    return default(TResult);
                                }
                                else
                                    throw responseError;
                            }
                        }

#if PERFMON
                        sw.Stop();
                        ApplicationContext.Current.PerformanceLog(nameof(RestClient), "InvokeInternal", $"{nameof(TBody)}-RCV", sw.Elapsed);
                        sw.Reset();
                        sw.Start();
#endif

                        responseHeaders = response.Headers;
                        var validationResult = this.ValidateResponse(response);
                        if (validationResult != ServiceClientErrorType.Valid)
                        {
                            this.m_tracer.TraceError("Response failed validation : {0}", validationResult);
                            throw new WebException(Strings.err_response_failed_validation, null, WebExceptionStatus.Success, response);
                        }

                        // No content - does the result want a pointer maybe?
                        if (response.StatusCode == HttpStatusCode.NoContent)
                        {
                            return default(TResult);
                        }
                        else
                        {
                            // De-serialize
                            var responseContentType = response.ContentType;
                            if (String.IsNullOrEmpty(responseContentType))
                                return default(TResult);

                            if (responseContentType.Contains(";"))
                                responseContentType = responseContentType.Substring(0, responseContentType.IndexOf(";"));

                            if (response.StatusCode == HttpStatusCode.NotModified)
                                return default(TResult);

                            serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(responseContentType, typeof(TResult));

                            TResult retVal = default(TResult);
                            // Compression?
                            using (MemoryStream ms = new MemoryStream())
                            {
                                if (this.Description.Trace)
                                    this.m_tracer.TraceVerbose("Received response {0} : {1} bytes", response.ContentType, response.ContentLength);

                                response.GetResponseStream().CopyTo(ms);

#if PERFMON
                                sw.Stop();
                                ApplicationContext.Current.PerformanceLog(nameof(RestClient), "InvokeInternal", $"{nameof(TBody)}-RCV", sw.Elapsed);
                                sw.Reset();
                                sw.Start();
#endif

                                ms.Seek(0, SeekOrigin.Begin);

                                // Trace
                                if (this.Description.Trace)
                                    this.m_tracer.TraceVerbose("HTTP << {0}", Convert.ToBase64String(ms.ToArray()));

                                switch (response.Headers[HttpResponseHeader.ContentEncoding])
                                {
                                    case "deflate":
                                        using (DeflateStream df = new DeflateStream(ms, CompressionMode.Decompress))
                                            retVal = (TResult)serializer.DeSerialize(df);
                                        break;
                                    case "gzip":
                                        using (GZipStream df = new GZipStream(ms, CompressionMode.Decompress))
                                            retVal = (TResult)serializer.DeSerialize(df);
                                        break;
                                    default:
                                        retVal = (TResult)serializer.DeSerialize(ms);
                                        break;
                                }

                            }

#if PERFMON
                            sw.Stop();
                            ApplicationContext.Current.PerformanceLog(nameof(RestClient), "InvokeInternal", $"{nameof(TBody)}-RET", sw.Elapsed);
                            sw.Reset();
                            sw.Start();
#endif

                            return retVal;
                        }
                    }
                    finally
                    {
                        if (response != null)
                            response.Dispose();
                    }
                }
                catch (TimeoutException e)
                {
                    this.m_tracer.TraceError("Request timed out:{0}", e);
                    throw;
                }
                catch (WebException e)
                {

                    this.m_tracer.TraceError(e.ToString());

                    // status
                    switch (e.Status)
                    {
                        case WebExceptionStatus.ProtocolError:

                            // Deserialize
                            TResult result = default(TResult);
                            var errorResponse = (e.Response as HttpWebResponse);
                            var responseContentType = errorResponse.ContentType;
                            if (responseContentType.Contains(";"))
                                responseContentType = responseContentType.Substring(0, responseContentType.IndexOf(";"));
                            var serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(responseContentType, typeof(TResult));
                            try
                            {
                                switch (errorResponse.Headers[HttpResponseHeader.ContentEncoding])
                                {
                                    case "deflate":
                                        using (DeflateStream df = new DeflateStream(errorResponse.GetResponseStream(), CompressionMode.Decompress))
                                            result = (TResult)serializer.DeSerialize(df);
                                        break;
                                    case "gzip":
                                        using (GZipStream df = new GZipStream(errorResponse.GetResponseStream(), CompressionMode.Decompress))
                                            result = (TResult)serializer.DeSerialize(df);
                                        break;
                                    default:
                                        result = (TResult)serializer.DeSerialize(errorResponse.GetResponseStream());
                                        break;
                                }
                            }
                            catch (Exception dse)
                            {
                                this.m_tracer.TraceError("Could not de-serialize error response! {0}", dse);
                            }

                            switch (errorResponse.StatusCode)
                            {
                                case HttpStatusCode.Unauthorized: // Validate the response
                                    if (this.ValidateResponse(errorResponse) != ServiceClientErrorType.Valid)
                                        throw new RestClientException<TResult>(
                                            result,
                                            e,
                                            e.Status,
                                            e.Response);

                                    break;
                                case HttpStatusCode.Conflict:
                                    throw new RestClientException<TBody>(
                                        body,
                                        e,
                                        e.Status,
                                        e.Response);
                                default:
                                    throw new RestClientException<TResult>(
                                        result,
                                        e,
                                        e.Status,
                                        e.Response);
                            }
                            break;
                        default:
                            throw;
                    }
                }

            }

            responseHeaders = new WebHeaderCollection();
            return default(TResult);
        }

        /// <summary>
        /// Gets or sets the client certificate
        /// </summary>
        /// <value>The client certificate.</value>
        public X509Certificate2Collection ClientCertificates
        {
            get;
            set;
        }
    }
}

