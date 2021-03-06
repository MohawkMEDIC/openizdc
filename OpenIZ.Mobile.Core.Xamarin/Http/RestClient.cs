﻿/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
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
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors;
using System.Reflection;
using System.Xml.Serialization;
using System.Net.Sockets;

namespace OpenIZ.Mobile.Core.Xamarin.Http
{
    /// <summary>
    /// Represents an android enabled rest client
    /// </summary>
    public class RestClient : RestClientBase
    {
        /// <summary>
        /// Identified data
        /// </summary>
        [XmlType(nameof(ErrorResult), Namespace = "http://openiz.org/imsi")]
        [XmlRoot(nameof(ErrorResult), Namespace = "http://openiz.org/imsi")]
        public class ErrorResult : IdentifiedData
        {

            /// <summary>
            /// Gets the date this was modified
            /// </summary>
            public override DateTimeOffset ModifiedOn
            {
                get
                {
                    return DateTimeOffset.Now;
                }
            }

            /// <summary>
            /// Represents an error result
            /// </summary>
            public ErrorResult()
            {
                this.Details = new List<ResultDetail>();
            }

            /// <summary>
            /// Gets or sets the details of the result
            /// </summary>
            [XmlElement("detail")]
            public List<ResultDetail> Details { get; set; }

            /// <summary>
            /// String representation
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return String.Join("\r\n", Details.Select(o => $">> {o.Type} : {o.Text}"));
            }
        }

        /// <summary>
        /// Gets or sets the detail type
        /// </summary>
        [XmlType(nameof(DetailType), Namespace = "http://openiz.org/imsi")]
        public enum DetailType
        {
            [XmlEnum("I")]
            Information,
            [XmlEnum("W")]
            Warning,
            [XmlEnum("E")]
            Error
        }

        /// <summary>
        /// A single result detail
        /// </summary
        [XmlType(nameof(ResultDetail), Namespace = "http://openiz.org/imsi")]
        public class ResultDetail
        {
            /// <summary>
            /// Default ctor
            /// </summary>
            public ResultDetail()
            { }

            /// <summary>
            /// Creates a new result detail
            /// </summary>
            public ResultDetail(DetailType type, string text)
            {
                this.Type = type;
                this.Text = text;
            }
            /// <summary>
            /// Gets or sets the type of the error
            /// </summary>
            [XmlAttribute("type")]
            public DetailType Type { get; set; }

            /// <summary>
            /// Gets or sets the text of the error
            /// </summary>
            [XmlText]
            public string Text { get; set; }
        }

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
            if (config.Binding.Security?.ClientCertificate != null)
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

            // Proxy?
            if (!String.IsNullOrEmpty(this.m_configurationSection.ProxyAddress))
                retVal.Proxy = new WebProxy(this.m_configurationSection.ProxyAddress);

            try
            {
                retVal.ServerCertificateValidationCallback = this.RemoteCertificateValidation;
            }
            catch
            {
                this.m_tracer.TraceWarning("Cannot assign certificate validtion callback, will set servicepointmanager");
                ServicePointManager.ServerCertificateValidationCallback = this.RemoteCertificateValidation;
            }

            // Set appropriate header
            if (this.Description.Binding.Optimize)
            {
                switch ((this.Description.Binding as ServiceClientBinding)?.OptimizationMethod)
                {
                    case OptimizationMethod.Lzma:
                        retVal.Headers[HttpRequestHeader.AcceptEncoding] = "lzma,bzip2,gzip,deflate";
                        break;
                    case OptimizationMethod.Bzip2:
                        retVal.Headers[HttpRequestHeader.AcceptEncoding] = "bzip2,gzip,deflate";
                        break;
                    case OptimizationMethod.Gzip:
                        retVal.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate";
                        break;
                    case OptimizationMethod.Deflate:
                        retVal.Headers[HttpRequestHeader.AcceptEncoding] = "deflate";
                        break;
                    case OptimizationMethod.None:
                        retVal.Headers[HttpRequestHeader.AcceptEncoding] = null;
                        break;

                }
            }

            // Set user agent
            var asm = Assembly.GetEntryAssembly() ?? typeof(RestClient).Assembly;
            retVal.UserAgent = String.Format("{0} {1} ({2})", asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title, asm.GetName().Version, asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
            return retVal;
        }

        /// <summary>
        /// Remote certificate validation errors
        /// </summary>
        private bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

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
                        try
                        {
                            // Get request object
                            using (var requestTask = Task.Run(async () => { return await requestObj.GetRequestStreamAsync(); }))
                            {
                                try
                                {
                                    if (!requestTask.Wait(this.Description.Endpoint[0].Timeout))
                                    {
                                        requestObj.Abort();
                                        throw new TimeoutException();
                                    }

                                    requestStream = requestTask.Result;
                                }
                                catch(AggregateException e)
                                {
                                    requestObj.Abort();
                                    throw e.InnerExceptions.First();
                                }
                            }
                            
                            if (contentType == null && typeof(TResult) != typeof(Object))
                                throw new ArgumentNullException(nameof(contentType));

                            serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(contentType, typeof(TBody));
                            // Serialize and compress with deflate
                            using (MemoryStream ms = new MemoryStream())
                            {
                                if (this.Description.Binding.Optimize)
                                {
                                    switch ((this.Description.Binding as ServiceClientBinding)?.OptimizationMethod)
                                    {
                                        case OptimizationMethod.Lzma:
                                            requestObj.Headers.Add("Content-Encoding", "lzma");
                                            using (var df = new LZipStream(requestStream, CompressionMode.Compress, leaveOpen: true))
                                                serializer.Serialize(df, body);
                                            break;
                                        case OptimizationMethod.Bzip2:
                                            requestObj.Headers.Add("Content-Encoding", "bzip2");
                                            using (var df = new BZip2Stream(requestStream, CompressionMode.Compress, leaveOpen: true))
                                                serializer.Serialize(df, body);
                                            break;
                                        case OptimizationMethod.Gzip:
                                            requestObj.Headers.Add("Content-Encoding", "gzip");
                                            using (var df = new GZipStream(requestStream, CompressionMode.Compress, leaveOpen: true))
                                                serializer.Serialize(df, body);
                                            break;
                                        case OptimizationMethod.Deflate:
                                            requestObj.Headers.Add("Content-Encoding", "deflate");
                                            using (var df = new DeflateStream(requestStream, CompressionMode.Compress, leaveOpen: true))
                                                serializer.Serialize(df, body);
                                            break;
                                        case OptimizationMethod.None:
                                        default:
                                            serializer.Serialize(ms, body);
                                            break;
                                    }
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
                    try
                    {

                        // Get request object
                        using (var responseTask = Task.Run(async () => { return await requestObj.GetResponseAsync(); }))
                        {
                            try
                            {
                                if (!responseTask.Wait(this.Description.Endpoint[0].Timeout))
                                {
                                    requestObj.Abort();
                                    throw new TimeoutException();
                                }
                                response = (HttpWebResponse)responseTask.Result;
                            }
                            catch (AggregateException e)
                            {
                                requestObj.Abort();
                                throw e.InnerExceptions.First();
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
                                ApplicationContext.Current.PerformanceLog(nameof(RestClient), "InvokeInternal", $"{nameof(TBody)}-INT", sw.Elapsed);
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
                                        using (DeflateStream df = new DeflateStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                            retVal = (TResult)serializer.DeSerialize(df);
                                        break;
                                    case "gzip":
                                        using (GZipStream df = new GZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                            retVal = (TResult)serializer.DeSerialize(df);
                                        break;
                                    case "bzip2":
                                        using (var bzs = new BZip2Stream(ms, CompressionMode.Decompress, leaveOpen: true))
                                            retVal = (TResult)serializer.DeSerialize(bzs);
                                        break;
                                    case "lzma":
                                        using (var lzmas = new LZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                            retVal = (TResult)serializer.DeSerialize(lzmas);
                                        break;
                                    default:
                                        retVal = (TResult)serializer.DeSerialize(ms);
                                        break;
                                }
                                //retVal = (TResult)serializer.DeSerialize(ms);
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
                        {
                            response.Close();
                            response.Dispose();
                        }
                        //responseTask.Dispose();
                    }
                }
                catch (TimeoutException e)
                {
                    this.m_tracer.TraceError("Request timed out:{0}", e.Message);
                    throw;
                }
                catch (WebException e)
                {

                    var errorResponse = (e.Response as HttpWebResponse);
                    if(errorResponse?.StatusCode == HttpStatusCode.NotModified)
                    {
                        this.m_tracer.TraceInfo("Server indicates not modified {0} {1} : {2}", method, url, e.Message);
                        responseHeaders = errorResponse?.Headers;
                        return default(TResult);
                    }

                    this.m_tracer.TraceError("Error executing {0} {1} : {2}", method, url, e.Message);

                    // status
                    switch (e.Status)
                    {
                        case WebExceptionStatus.ProtocolError:

                            // Deserialize
                            object errorResult = default(ErrorResult);
                            
                            var responseContentType = errorResponse.ContentType;
                            if (responseContentType.Contains(";"))
                                responseContentType = responseContentType.Substring(0, responseContentType.IndexOf(";"));

                            var ms = new MemoryStream(); // copy response to memory
                            errorResponse.GetResponseStream().CopyTo(ms);
                            ms.Seek(0, SeekOrigin.Begin);

                            try
                            {
                                var serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(responseContentType, typeof(TResult));

                                try
                                {
                                    switch (errorResponse.Headers[HttpResponseHeader.ContentEncoding])
                                    {
                                        case "deflate":
                                            using (DeflateStream df = new DeflateStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                                errorResult = (TResult)serializer.DeSerialize(df);
                                            break;
                                        case "gzip":
                                            using (GZipStream df = new GZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                                errorResult = (TResult)serializer.DeSerialize(df);
                                            break;
                                        case "bzip2":
                                            using (var bzs = new BZip2Stream(ms, CompressionMode.Decompress, leaveOpen: true))
                                                errorResult = (TResult)serializer.DeSerialize(bzs);
                                            break;
                                        case "lzma":
                                            using (var lzmas = new LZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                                errorResult = (TResult)serializer.DeSerialize(lzmas);
                                            break;
                                        default:
                                            errorResult = (TResult)serializer.DeSerialize(ms);
                                            break;
                                    }
                                }
                                catch
                                {
                                    serializer = this.Description.Binding.ContentTypeMapper.GetSerializer(responseContentType, typeof(ErrorResult));

                                    ms.Seek(0, SeekOrigin.Begin); // rewind and try generic error codes
                                    switch (errorResponse.Headers[HttpResponseHeader.ContentEncoding])
                                    {
                                        case "deflate":
                                            using (DeflateStream df = new DeflateStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                                errorResult = (ErrorResult)serializer.DeSerialize(df);
                                            break;
                                        case "gzip":
                                            using (GZipStream df = new GZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                                errorResult = (ErrorResult)serializer.DeSerialize(df);
                                            break;
                                        case "bzip2":
                                            using (var bzs = new BZip2Stream(ms, CompressionMode.Decompress, leaveOpen: true))
                                                errorResult = (ErrorResult)serializer.DeSerialize(bzs);
                                            break;
                                        case "lzma":
                                            using (var lzmas = new LZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
                                                errorResult = (ErrorResult)serializer.DeSerialize(lzmas);
                                            break;
                                        default:
                                            errorResult = (ErrorResult)serializer.DeSerialize(ms);
                                            break;
                                    }
                                }
                                //result = (TResult)serializer.DeSerialize(errorResponse.GetResponseStream());
                            }
                            catch (Exception dse)
                            {
                                this.m_tracer.TraceError("Could not de-serialize error response! {0}", dse.Message);
                            }

                            Exception exception = null;
                            if (errorResult is TResult)
                                exception = new RestClientException<TResult>((TResult)errorResult, e, e.Status, e.Response);
                            else
                                exception = new RestClientException<ErrorResult>((ErrorResult)errorResult, e, e.Status, e.Response);

                            switch (errorResponse.StatusCode)
                            {
                                case HttpStatusCode.Unauthorized: // Validate the response
                                    if (this.ValidateResponse(errorResponse) != ServiceClientErrorType.Valid)
                                        throw exception;
                                    break;
                                case HttpStatusCode.NotModified:
                                    responseHeaders = errorResponse?.Headers;
                                    return default(TResult);
                                default:
                                    throw exception;
                            }
                            break;
                        case WebExceptionStatus.ConnectFailure:
                            if ((e.InnerException as SocketException)?.SocketErrorCode == SocketError.TimedOut)
                                throw new TimeoutException();
                            else
                                throw;
                        case WebExceptionStatus.Timeout:
                            throw new TimeoutException();
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

