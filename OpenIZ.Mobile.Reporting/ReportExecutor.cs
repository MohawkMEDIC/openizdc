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
using OpenIZ.Mobile.Core;

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

            // Last row values
            private Dictionary<String, String> m_lastValue = new Dictionary<string, string>();

            /// <summary>
            /// Create new root report execution context
            /// </summary>
            public ReportExecutionContext(ReportDefinition report, IDictionary<String, Object> arguments, Object scope, IEnumerable dataset)
            {
                this.ParentScope = default(ReportExecutionContext);
                this.Report = report;
                this.Scope = scope;
                this.Dataset = dataset;
                this.Arguments = arguments;
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
            /// Arguments
            /// </summary>
            public IDictionary<String, Object>  Arguments { get; set; }

            /// <summary>
            /// Get root scope
            /// </summary>
            public ReportExecutionContext RootScope
            {
                get
                {
                    var rs = this;
                    while (rs.ParentScope != null) rs = rs.ParentScope;
                    return rs;
                }
            }

            /// <summary>
            /// Creates a sub-context from the current context
            /// </summary>
            public ReportExecutionContext Create(dynamic o)
            {
                return new ReportExecutionContext(this, o);
            }

            /// <summary>
            /// Set last 
            /// </summary>
            public void SetLast(string key, string value)
            {
                if (!this.m_lastValue.ContainsKey(key))
                    this.m_lastValue.Add(key, value);
                else
                    this.m_lastValue[key] = value;
            }

            /// <summary>
            /// Get last row data
            /// </summary>
            public string GetLast(string key)
            {
                var value = String.Empty;
                this.m_lastValue.TryGetValue(key, out value);
                return value;
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
                    else if (kv.Value != null)
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
                exeSets.Add(itm.Name, this.RenderDataset(rdl.ConnectionString, itm, cParms));

            // Now we have our data, let us render it!!!
            if (view.Body != null) // HTML
            {
                XElement renderedBody = new XElement(view.Body);

                // Look for control parameters
                this.RenderScope(renderedBody, new ReportExecutionContext(rdl, cParms, exeSets, exeSets));

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
            else if (facet.Name == xs_report + "switch")
            {
                bool success = false;
                if (facet.Attribute("expr") == null)
                    throw new InvalidOperationException("Switch must have expr attribute");
                var value = this.CompileExpression($"{context.Report.Description.Name}.{context.Scope.GetType().Name}.{facet.Attribute("expr").Value}", facet.Attribute("expr").Value).DynamicInvoke(context.Scope);


                var when = facet.Attribute("when")?.Value;
                switch (when)
                {
                    case "changed":
                        if (context.ParentScope?.GetLast(facet.Value) == value.ToString())
                        {
                            facet.Value = ""; // no change        
                            return;
                        }
                        break;
                }
                context.ParentScope?.SetLast(facet.Value, value.ToString());

                var xel = facet.Elements(xs_report + "when").FirstOrDefault();
                while (xel != null)
                {
                    ExpressionType comparator = ExpressionType.Equal;
                    switch (xel.Attribute("op")?.Value)
                    {
                        case "gt":
                            comparator = ExpressionType.GreaterThan;
                            break;
                        case "gte":
                            comparator = ExpressionType.GreaterThanOrEqual;
                            break;
                        case "lt":
                            comparator = ExpressionType.LessThan;
                            break;
                        case "lte":
                            comparator = ExpressionType.LessThanOrEqual;
                            break;
                        case "ne":
                            comparator = ExpressionType.NotEqual;
                            break;
                    }
                    object operand = null;

                    if(value != null && !MapUtil.TryConvert(xel.Attribute("value").Value, value.GetType(), out operand))
                        throw new InvalidCastException($"Can't convert {xel.Attribute("value")} to {value.GetType().Name}");

                    bool result = false;
                    if (value != null)
                    {
                        var parm = Expression.Parameter(value.GetType(), "p");
                        var expr = Expression.Lambda(Expression.MakeBinary(comparator, parm, Expression.Constant(operand)), parm).Compile();
                        result = (bool)expr.DynamicInvoke(value);
                    }

                    // Success an result
                    if (!success && result)
                    {
                        success = true;
                        this.RenderScope(xel, context);
                        xel.ReplaceWith((XNode)xel.Elements().FirstOrDefault() ?? new XText(xel.Value));
                    }
                    else
                        xel.Remove();
                    xel = facet.Elements(xs_report + "when").FirstOrDefault();
                }

                // Default!!!!
                if (facet.Element(xs_report + "default") != null)
                {
                    if (!success)
                    {
                        this.RenderScope(facet.Element(xs_report + "default"), context);
                        facet.ReplaceWith(facet.Elements().First());
                    }
                    else
                        facet.Element(xs_report + "default").Remove();
                }


            }
            else if (facet.Name == xs_report + "value")
            {
                var when = facet.Attribute("when")?.Value;
                var value = this.RenderValue(facet, context);
                switch (when)
                {
                    case "changed":
                        if (context.ParentScope?.GetLast(facet.Value) == value)
                        {
                            facet.Value = ""; // no change        
                            return;
                        }
                        break;
                }
                context.ParentScope?.SetLast(facet.Value, value);
                facet.ReplaceWith(new XText(value));
            }
            else if(facet.Name == xs_report + "parm")
            {
                facet.ReplaceWith(new XText(String.Format(facet.Attribute("format")?.Value ?? "{0}", context.RootScope.Arguments[facet.Value])));
            }
            else if (facet.Name == xs_report + "expr")
            {
                Delegate evaluator = this.CompileExpression($"{context.Report.Description.Name}.{context.Scope.GetType().Name}.{facet.Value}", facet.Value);

                var toAtt = facet.Attribute("to-att")?.Value;
                if (!String.IsNullOrEmpty(toAtt))
                {
                    facet.Parent.Add(new XAttribute(toAtt, String.Format(facet.Attribute("format")?.Value ?? "{0}", this.RenderString(evaluator.DynamicInvoke(context.Scope), context))));
                    facet.Remove();
                }
                else
                    facet.ReplaceWith(new XText(String.Format(facet.Attribute("format")?.Value ?? "{0}", this.RenderString(evaluator.DynamicInvoke(context.Scope), context))));
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
                    var expr = this.CompileExpression($"{context.Report.Description.Name}.count.{facet.Value}", facet.Value);

                    switch (function)
                    {
                        case "sum":
                            vValue = Enumerable.Sum<dynamic>(scopeEnum, o => (decimal?)expr.DynamicInvoke(context.Create(o).Scope));
                            break;
                        case "count":
                            if (String.IsNullOrEmpty(facet.Value))
                                vValue = Enumerable.Count<dynamic>(scopeEnum);
                            else
                            {
                                vValue = Enumerable.Count<dynamic>(scopeEnum, o => (bool)expr.DynamicInvoke(context.Create(o).Scope));
                            }
                            break;
                        case "min":
                            vValue = Enumerable.Min<dynamic>(scopeEnum, o => (decimal?)expr.DynamicInvoke(context.Create(o).Scope));
                            break;
                        case "max":
                            vValue = Enumerable.Max<dynamic>(scopeEnum, o => (decimal?)expr.DynamicInvoke(context.Create(o).Scope));
                            break;
                        case "avg":
                            vValue = Enumerable.Average<dynamic>(scopeEnum, o => (decimal?)expr.DynamicInvoke(context.Create(o).Scope));
                            break;
                    }
                    facet.ReplaceWith(new XText(String.Format(facet.Attribute("format")?.Value ?? "{0}", this.RenderString(vValue, context))));
                }
            }
            else
            {
                var elements = new List<XElement>(facet.Elements());
                foreach (var cel in elements)
                {
                    this.RenderScope(cel, context);
                }
            }
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
        private String RenderValue(XElement valueElement, ReportExecutionContext context)
        {
            String format = valueElement.Attribute("format")?.Value;

            if (format != null)
                return String.Format(format, valueElement.Value.Split(',').Select(o => this.RenderString(this.GetBind(context, o), context)).ToArray());
            else
                return this.RenderString(this.GetBind(context, valueElement.Value), context)?.ToString() ?? "";

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
            else
            {
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

            var retVal = dsProvider.ExecuteDataset(connectionString, stmt, values);

            if (dataset.Pivot != null)
            {

                // Algorithm for pivoting : 
                List<ExpandoObject> nRetVal = new List<ExpandoObject>();
                IDictionary<String, Object> cobject = null;
                foreach (IDictionary<String, Object> itm in retVal)
                {
                    var key = itm[dataset.Pivot.Key];
                    if (cobject == null || !key.Equals(cobject[dataset.Pivot.Key]) && cobject.Count > 0)
                    {
                        if (cobject != null)
                            nRetVal.Add(cobject as ExpandoObject);
                        cobject = new ExpandoObject();
                        cobject.Add(dataset.Pivot.Key, key);
                    }
                    // Same key, so lets create or accumulate
                    var column = itm[dataset.Pivot.Columns];
                    if (!cobject.ContainsKey(column.ToString()))
                        cobject.Add(column.ToString(), itm[dataset.Pivot.Value]);
                    else
                    {
                        var cvalue = cobject[column.ToString()];
                        var avalue = itm[dataset.Pivot.Value];
                        if (cvalue is Decimal)
                            cvalue = (decimal)cvalue + (decimal)avalue;
                        else if (cvalue is double)
                            cvalue = (double)cvalue + (double)avalue;
                        else if (cvalue is long)
                            cvalue = (long)cvalue + (long)avalue;
                        cobject[column.ToString()] = cvalue;
                    }
                }
                nRetVal.Add(cobject as ExpandoObject);
                retVal = nRetVal;

            }
            return retVal;
        }
    }
}
