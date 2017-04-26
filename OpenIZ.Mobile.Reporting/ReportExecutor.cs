using OpenIZ.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenIZ.Mobile.Reporting.Model;
using System.Xml.Linq;
using System.Collections;
using System.Reflection;
using ExpressionEvaluator;
using System.Dynamic;
using System.Xml;
using OpenIZ.Core.Model.Map;
using System.Linq.Expressions;
using OpenIZ.Core.Model;

namespace OpenIZ.Mobile.Reporting
{
    /// <summary>
    /// Represents a class which executes the report
    /// </summary>
    public class ReportExecutor
    {

        // Reporting
        private readonly XNamespace xs_report = "http://openiz.org/mobile/reporting";
        private readonly XNamespace xs_html = "http://www.w3.org/1999/xhtml";

        private Dictionary<String, Delegate> m_cachedExpressions = new Dictionary<string, Delegate>();

        private class ReportExecutionContext
        {

            /// <summary>
            /// Create new root report execution context
            /// </summary>
            public ReportExecutionContext(ReportDefinition report, Object scope, IEnumerable dataset)
            {
                this.ParentScope = default(ReportExecutionContext);
                this.Report = report;
                this.Scope = scope;
                this.Dataset = dataset;
            }

            /// <summary>
            /// Parent scope
            /// </summary>
            public ReportExecutionContext(ReportExecutionContext parentScope, Object scope)
            {
                this.ParentScope = parentScope;
                this.Scope = scope;
                this.Dataset = parentScope.Dataset;
                this.Report = parentScope.Report;
            }

            /// <summary>
            /// Parent scope
            /// </summary>
            public ReportExecutionContext(ReportExecutionContext parentScope, Object scope, IEnumerable dataset)
            {
                this.ParentScope = parentScope;
                this.Scope = scope;
                this.Dataset = dataset;
                this.Report = parentScope.Report;
            }

            /// <summary>
            /// Gets or sets the root scope
            /// </summary>
            public ReportExecutionContext ParentScope { get; private set; }
            /// <summary>
            /// Gets or sets the report
            /// </summary>
            public ReportDefinition Report { get; private set; }
            /// <summary>
            /// Gets or sets the scope
            /// </summary>
            public Object Scope { get; private set; }
            /// <summary>
            /// Gets or sets the dataset in which "scope" is located
            /// </summary>
            public IEnumerable Dataset { get; private set; }

            /// <summary>
            /// Get root scope
            /// </summary>
            public object RootScope
            {
                get
                {
                    var rs = this;
                    while (rs.ParentScope != null) rs = rs.ParentScope;
                    return rs;
                }
            }
        }

        /// <summary>
        /// Render the specified report
        /// </summary>
        public byte[] RenderReport(String reportName, String viewName, IDictionary<String, Object> pParms)
        {

            var repository = ApplicationServiceContext.Current.GetService(typeof(IReportRepository)) as IReportRepository;
            if (repository == null)
                throw new InvalidOperationException("Report repository not found");

            // load the specified report
            var rdl = repository.GetReport(reportName);
            if (rdl == null)
                throw new FileNotFoundException(reportName);

            var view = rdl.Views.SingleOrDefault(o => o.Name == viewName);
            if (view == null)
                throw new KeyNotFoundException(viewName);

            var cParms = new Dictionary<String, Object>();

            // Cast parms
            foreach (var kv in pParms)
            {
                try
                {
                    var rparm = rdl.Parameters.FirstOrDefault(o => o.Name == kv.Key);
                    if (rparm == null)
                        continue;
                    if (kv.Value != null)
                        switch (rparm.Type)
                        {
                            case ReportPropertyType.ByteArray:
                                cParms[rparm.Name] = kv.Value;
                                break;
                            case ReportPropertyType.Date:
                            case ReportPropertyType.DateTime:
                                cParms[rparm.Name] = DateTime.Parse(kv.Value.ToString());
                                break;
                            case ReportPropertyType.Decimal:
                                cParms[rparm.Name] = Decimal.Parse(kv.Value.ToString());
                                break;
                            case ReportPropertyType.Integer:
                                cParms[rparm.Name] = Int32.Parse(kv.Value.ToString());
                                break;
                            case ReportPropertyType.String:
                                cParms[rparm.Name] = kv.Value?.ToString();
                                break;
                            case ReportPropertyType.Uuid:
                                cParms[rparm.Name] = Guid.Parse(kv.Value.ToString());
                                break;

                        }
                }
                catch { }
            }

            // Now we want to format our report parameters to appropriate SQL
            Dictionary<String, IEnumerable<dynamic>> exeSets = new Dictionary<string, IEnumerable<dynamic>>(rdl.Datasets.Count);
            foreach (var itm in rdl.Datasets)
                exeSets.Add(itm.Name, this.RenderDataset(rdl.ConnectionString, itm, pParms));
            exeSets.Add("args", new List<dynamic>() { pParms });

            // Now we have our data, let us render it!!!
            if (view.Body != null) // HTML
            {
                XElement renderedBody = new XElement(view.Body);

                // Look for control parameters
                this.RenderScope(renderedBody, new ReportExecutionContext(rdl, exeSets, exeSets));

                using (var ms = new MemoryStream())
                {
                    using (var xw = XmlWriter.Create(ms, new XmlWriterSettings()
                    {
                        OmitXmlDeclaration = true,
                        Encoding = System.Text.Encoding.UTF8

                    }))
                        renderedBody.WriteTo(xw);
                    return ms.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Render scope object
        /// </summary>
        private void RenderScope<TFacet>(TFacet facet, ReportExecutionContext context) where TFacet : XElement
        {
            if (facet.Name == xs_report + "repeat")
            {
                var bind = facet.Attribute("bind");
                var subScope = this.GetBind(context, bind?.Value);
                if (!(subScope is IEnumerable))
                    throw new InvalidOperationException("Repeat must be performed on a IEnumerable scope");
                var subContext = new ReportExecutionContext(context, subScope, subScope as IEnumerable);

                List<XElement> newChildren = new List<XElement>();
                foreach (var itm in subScope as IEnumerable)
                {
                    foreach (var cel in facet.Elements())
                    {
                        var nchild = new XElement(cel);
                        this.RenderScope(nchild, new ReportExecutionContext(subContext, itm));
                        newChildren.Add(nchild);
                    }
                }
                foreach (var itm in facet.Elements())
                    itm.Remove();
                facet.Add(newChildren.ToArray());
                //facet.Remove();
            }
            else if (facet.Name == xs_report + "value")
            {
                this.RenderValue(facet, context);
            }
            else if (facet.Name == xs_report + "expr")
            {
                Delegate evaluator = this.CompileExpression($"{context.Report.Description.Name}.{context.Scope.GetType().Name}.{facet.Value}", facet.Value);

                // Aggregation?
                facet.ReplaceWith(new XText(this.RenderString(evaluator.DynamicInvoke(context.Scope), context).ToString()));
            }
            else if (facet.Name == xs_report + "aggregate")
            {
                String function = facet.Attribute("fn")?.Value,
                    bind = facet.Attribute("bind")?.Value;
                Object aScope = context.Dataset as IEnumerable;

                if (!String.IsNullOrEmpty(bind))
                    aScope = this.GetBind(context, bind);

                if (aScope is IEnumerable)
                {
                    object vValue = null;
                    IEnumerable<dynamic> scopeEnum = aScope as IEnumerable<dynamic>;
                    switch (function)
                    {
                        case "sum":
                            vValue = Enumerable.Sum<dynamic>(scopeEnum, o => this.GetBind(o, facet.Value));
                            break;
                        case "count":
                            if (String.IsNullOrEmpty(facet.Value))
                                vValue = Enumerable.Count<dynamic>(scopeEnum);
                            else
                            {
                                var expr = this.CompileExpression($"{context.Report.Description.Name}.count.{facet.Value}", facet.Value);
                                vValue = Enumerable.Count<dynamic>(scopeEnum, o => (bool)expr.DynamicInvoke(o));
                            }
                            break;
                        case "min":
                            vValue = Enumerable.Min<dynamic>(scopeEnum, o => this.GetBind(o, facet.Value));
                            break;
                        case "max":
                            vValue = Enumerable.Max<dynamic>(scopeEnum, o => this.GetBind(o, facet.Value));
                            break;
                        case "avg":
                            vValue = Enumerable.Average<dynamic>(scopeEnum, o => this.GetBind(o, facet.Value));
                            break;
                    }
                    facet.ReplaceWith(new XText(this.RenderString(vValue, context).ToString()));
                }
            }
            else
                foreach (var cel in facet.Elements())
                    this.RenderScope(cel, context);

        }

        /// <summary>
        /// Compile expression
        /// </summary>
        private Delegate CompileExpression(string name, string body)
        {
            Delegate evaluator = null;

            if (!this.m_cachedExpressions.TryGetValue(name, out evaluator))
            {

                var expression = new CompiledExpression(body);
                expression.TypeRegistry = new TypeRegistry();
                expression.TypeRegistry.RegisterDefaultTypes();
                expression.TypeRegistry.RegisterType<Guid>();
                expression.TypeRegistry.RegisterType<DateTimeOffset>();
                expression.TypeRegistry.RegisterType<TimeSpan>();
                evaluator = expression.ScopeCompile<ExpandoObject>();

                lock (this.m_cachedExpressions)
                    this.m_cachedExpressions.Add(name, evaluator);

            }
            return evaluator;
        }

        /// <summary>
        /// Render value 
        /// </summary>
        private void RenderValue(XElement valueElement, ReportExecutionContext context)
        {
            String format = valueElement.Attribute("format")?.Value;

            if (format != null)
                valueElement.ReplaceWith(new XText(String.Format(format, valueElement.Value.Split(',').Select(o => this.RenderString(this.GetBind(context, o), context).ToString()).ToArray())));
            else
                valueElement.ReplaceWith(new XText(this.RenderString(this.GetBind(context, valueElement.Value), context).ToString()));

        }

        /// <summary>
        /// Render string
        /// </summary>
        private object RenderString(object value, ReportExecutionContext context)
        {
            if (value is DateTime)
            {
                if (((DateTime)value).ToString("HH:mm:ss") == "00:00:00")
                    return ((DateTime)value).ToString(context.Report.Formatter.Date);
                else
                    return ((DateTime)value).ToString(context.Report.Formatter.DateTime);
            }
            return value;
        }

        /// <summary>
        /// Get bind value
        /// </summary>
        private object GetBind(ReportExecutionContext context, String bind)
        {
            bind = bind.Replace("[", "").Replace("]", "").Replace("\"", "");
            if (String.IsNullOrEmpty(bind) || context == null)
                return null;
            else {
                try
                {
                    if (context.Scope is IDictionary)
                        return (context.Scope as IDictionary)[bind];
                    else if (context.Scope is ExpandoObject)
                        return (context.Scope as IDictionary<String, Object>)[bind];
                    else
                    {
                        var mi = context.Scope.GetType().GetRuntimeProperty(bind);
                        if (mi == null)
                            return mi?.GetValue(context.Scope);
                    }
                }
                catch { }

                if (context.ParentScope != null)
                    return this.GetBind(context.ParentScope, bind); // go up one level
                else
                    return null;
            }
        }

        /// <summary>
        /// Render parameter
        /// </summary>
        public IEnumerable<dynamic> RenderParameterValues(String reportName, String parameterName, IDictionary<String, Object> pParms)
        {
            var repository = ApplicationServiceContext.Current.GetService(typeof(IReportRepository)) as IReportRepository;
            if (repository == null)
                throw new InvalidOperationException("Report repository not found");

            // load the specified report
            var rdl = repository.GetReport(reportName);
            if (rdl == null)
                throw new FileNotFoundException(reportName);

            var parm = rdl.Parameters.SingleOrDefault(p => p.Name == parameterName);

            if (parm.ValueSet == null)
                return null;
            else
                return this.RenderDataset(rdl.ConnectionString, parm.ValueSet, pParms);

        }

        /// <summary>
        /// Render dataset
        /// </summary>
        private IEnumerable<dynamic> RenderDataset(List<ReportConnectionString> connectionString, ReportDatasetDefinition dataset, IDictionary<string, object> pParms)
        {
            var dsProvider = ApplicationServiceContext.Current.GetService(typeof(IReportDatasource)) as IReportDatasource;
            if (dsProvider == null)
                throw new InvalidOperationException("Cannot find report datasource");

            // Prepare the SQL
            var sql = dataset.Sql.FirstOrDefault(o => o.Provider == dsProvider.Name);
            if (sql == null)
                throw new InvalidOperationException($"Cannot bind dataset {dataset.Name} no data source for {dsProvider.Name}");

            var regEx = new Regex(@"\$\{(\w*?)\}");
            var stmt = sql.Sql;
            List<Object> values = new List<object>();

            // Replace ${ParmName} with parameter
            foreach (Match m in regEx.Matches(stmt))
            {
                object pValue = null;
                pParms.TryGetValue(m.Groups[1].Value, out pValue);
                values.Add(pValue);
            }
            stmt = regEx.Replace(stmt, "?");

            return dsProvider.ExecuteDataset(connectionString, stmt, values);
        }
    }
}
