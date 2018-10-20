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
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Data.QueryBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using System.Collections;
using System.Text.RegularExpressions;
using OpenIZ.Core.Model.Attributes;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Core.Data.QueryBuilder
{

    /// <summary>
    /// Query predicate part
    /// </summary>
    public enum QueryPredicatePart
    {
        Full = Path | Guard | Cast | SubPath,
        Path = 0x1,
        Guard = 0x2,
        Cast = 0x4,
        SubPath = 0x8,
        PropertyAndGuard = Path | Guard,
        PropertyAndCast = Path | Cast
    }

    /// <summary>
    /// Represents the query predicate
    /// </summary>
    public class QueryPredicate
    {
        // Regex to extract property, guards and cast
        public static readonly Regex ExtractionRegex = new Regex(@"^(\w*?)(\[(.*?)\])?(\@(\w*))?(\.(.*))?$");

        private const int PropertyRegexGroup = 1;
        private const int GuardRegexGroup = 3;
        private const int CastRegexGroup = 5;
        private const int SubPropertyRegexGroup = 7;

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        public String Path { get; private set; }

        /// <summary>
        /// Sub-path
        /// </summary>
        public String SubPath { get; private set; }

        /// <summary>
        /// Cast instruction
        /// </summary>
        public String CastAs { get; private set; }

        /// <summary>
        /// Guard condition
        /// </summary>
        public String Guard { get; private set; }

        /// <summary>
        /// Parse a condition
        /// </summary>
        public static QueryPredicate Parse(String condition)
        {
            var matches = ExtractionRegex.Match(condition);
            if (!matches.Success) return null;

            return new QueryPredicate()
            {
                Path = matches.Groups[PropertyRegexGroup].Value,
                CastAs = matches.Groups[CastRegexGroup].Value,
                Guard = matches.Groups[GuardRegexGroup].Value,
                SubPath = matches.Groups[SubPropertyRegexGroup].Value
            };
        }

        /// <summary>
        /// Represent the predicate as a string
        /// </summary>
        public String ToString(QueryPredicatePart parts)
        {
            StringBuilder sb = new StringBuilder();

            if ((parts & QueryPredicatePart.Path) != 0)
                sb.Append(this.Path);
            if ((parts & QueryPredicatePart.Guard) != 0 && !String.IsNullOrEmpty(this.Guard))
                sb.AppendFormat("[{0}]", this.Guard);
            if ((parts & QueryPredicatePart.Cast) != 0 && !String.IsNullOrEmpty(this.CastAs))
                sb.AppendFormat("@{0}", this.CastAs);
            if ((parts & QueryPredicatePart.SubPath) != 0 && !String.IsNullOrEmpty(this.SubPath))
                sb.AppendFormat("{0}{1}", sb.Length > 0 ? "." : "",  this.SubPath);

            return sb.ToString();
        }

    }

    /// <summary>
    /// Query builder for model objects
    /// </summary>
    /// <remarks>
    /// Because the ORM used in the ADO persistence layer is very very lightweight, this query builder exists to parse 
    /// LINQ or HTTP query parameters into complex queries which implement joins/CTE/etc. across tables. Stuff that the
    /// classes in the little data model can't possibly support via LINQ expression.
    /// 
    /// To use this, simply pass a model based LINQ expression to the CreateQuery method. Examples are in the test project. 
    /// 
    /// Some reasons to use this:
    ///     - The generated SQL will gather all table instances up the object hierarchy for you (one hit instead of multiple)
    ///     - The queries it writes use efficient CTE tables
    ///     - It can do intelligent join conditions
    ///     - It uses Model LINQ expressions directly to SQL without the need to translate from Model LINQ to Domain LINQ queries
    /// </remarks>
    /// <example lang="cs" name="LINQ Expression illustrating join across tables">
    /// <![CDATA[QueryBuilder.CreateQuery<Patient>(o => o.DeterminerConcept.Mnemonic == "Instance")]]>
    /// </example>
    /// <example lang="sql" name="Resulting SQL query">
    /// <![CDATA[
    /// WITH 
    ///     cte0 AS (
    ///         SELECT cd_tbl.cd_id 
    ///         FROM cd_vrsn_tbl AS cd_vrsn_tbl 
    ///             INNER JOIN cd_tbl AS cd_tbl ON (cd_tbl.cd_id = cd_vrsn_tbl.cd_id) 
    ///         WHERE (cd_vrsn_tbl.mnemonic = ? )
    ///     )
    /// SELECT * 
    /// FROM pat_tbl AS pat_tbl 
    ///     INNER JOIN psn_tbl AS psn_tbl ON (pat_tbl.ent_vrsn_id = psn_tbl.ent_vrsn_id) 
    ///     INNER JOIN ent_vrsn_tbl AS ent_vrsn_tbl ON (psn_tbl.ent_vrsn_id = ent_vrsn_tbl.ent_vrsn_id) 
    ///     INNER JOIN ent_tbl AS ent_tbl ON (ent_tbl.ent_id = ent_vrsn_tbl.ent_id) 
    ///     INNER JOIN cte0 ON (ent_tbl.dtr_cd_id = cte0.cd_id)
    /// ]]>
    /// </example>
    public class QueryBuilder
    {

        // Join cache
        private Dictionary<String, KeyValuePair<SqlStatement, List<TableMapping>>> s_joinCache = new Dictionary<String, KeyValuePair<SqlStatement, List<TableMapping>>>();

        // Mapper
        private ModelMapper m_mapper;
        private const int PropertyRegexGroup = 1;
        private const int GuardRegexGroup = 3;
        private const int CastRegexGroup = 5;
        private const int SubPropertyRegexGroup = 7;

        // A list of hacks injected into this query builder
        private List<IQueryBuilderHack> m_hacks = new List<IQueryBuilderHack>();

        /// <summary>
        /// Represents model mapper
        /// </summary>
        /// <param name="mapper"></param>
        public QueryBuilder(ModelMapper mapper, params IQueryBuilderHack[] hacks)
        {
            this.m_mapper = mapper;
            this.m_hacks = hacks.ToList();
        }

        /// <summary>
        /// Create a query 
        /// </summary>
        public SqlStatement CreateQuery<TModel>(Expression<Func<TModel, bool>> predicate)
        {
            var nvc = QueryExpressionBuilder.BuildQuery(predicate, true);
            return CreateQuery<TModel>(nvc);
        }

        /// <summary>
        /// Create query
        /// </summary>
        public SqlStatement CreateQuery<TModel>(IEnumerable<KeyValuePair<String, Object>> query, params ColumnMapping[] selector)
        {
            return CreateQuery<TModel>(query, null, selector);
        }

        /// <summary>
        /// Create query 
        /// </summary>
        public SqlStatement CreateQuery<TModel>(IEnumerable<KeyValuePair<String, Object>> query, String tablePrefix, params ColumnMapping[] selector)
        {
            return CreateQuery<TModel>(query, null, false, selector);
        }

        /// <summary>
        /// Query query 
        /// </summary>
        /// <param name="query"></param>
        public SqlStatement CreateQuery<TModel>(IEnumerable<KeyValuePair<String, Object>> query, String tablePrefix, bool skipJoins, params ColumnMapping[] selector)
        {
            var tableType = m_mapper.MapModelType(typeof(TModel));
            var tableMap = TableMapping.Get(tableType);
            List<TableMapping> scopedTables = new List<TableMapping>() { tableMap };

            bool skipParentJoin = true;
            SqlStatement selectStatement = null;
            KeyValuePair<SqlStatement, List<TableMapping>> cacheHit;
            if (skipJoins)
            {
                selectStatement = new SqlStatement($" FROM {tableMap.TableName} AS {tablePrefix}{tableMap.TableName} ");

            }
            else
            {
                if (!s_joinCache.TryGetValue($"{tablePrefix}.{typeof(TModel).Name}", out cacheHit))
                {
                    selectStatement = new SqlStatement($" FROM {tableMap.TableName} AS {tablePrefix}{tableMap.TableName} ");

                    Stack<TableMapping> fkStack = new Stack<TableMapping>();
                    fkStack.Push(tableMap);
                    // Always join tables?
                    do
                    {
                        var dt = fkStack.Pop();
                        foreach (var jt in dt.Columns.Where(o => o.IsAlwaysJoin))
                        {
                            var fkTbl = TableMapping.Get(jt.ForeignKey.Table);
                            var fkAtt = fkTbl.GetColumn(jt.ForeignKey.Column);
                            selectStatement.Append($"INNER JOIN {fkAtt.Table.TableName} AS {tablePrefix}{fkAtt.Table.TableName} ON ({tablePrefix}{jt.Table.TableName}.{jt.Name} = {tablePrefix}{fkAtt.Table.TableName}.{fkAtt.Name}) ");
                            if (!scopedTables.Contains(fkTbl))
                                fkStack.Push(fkTbl);
                            scopedTables.Add(fkAtt.Table);
                        }
                    } while (fkStack.Count > 0);

                    // Add the heavy work to the cache
                    lock (s_joinCache)
                        if (!s_joinCache.ContainsKey($"{tablePrefix}.{typeof(TModel).Name}"))
                            s_joinCache.Add($"{tablePrefix}.{typeof(TModel).Name}", new KeyValuePair<SqlStatement, List<TableMapping>>(selectStatement.Build(), scopedTables));
                }
                else
                {
                    selectStatement = cacheHit.Key.Build();
                    scopedTables = cacheHit.Value;
                }
            }

            // Column definitions
            var columnSelector = selector;
            if (selector == null || selector.Length == 0)
                columnSelector = scopedTables.SelectMany(o => o.Columns).ToArray();
            // columnSelector = scopedTables.SelectMany(o => o.Columns).ToArray();

            List<String> flatNames = new List<string>();
            var columnList = String.Join(",", columnSelector.Select(o =>
                {
                    var rootCol = tableMap.GetColumn(o.SourceProperty);
                    skipParentJoin &= rootCol != null;
                    if (!flatNames.Contains(o.Name))
                    {
                        flatNames.Add(o.Name);
                        return $"{tablePrefix}{o.Table.TableName}.{o.Name} AS {o.Name}";
                    }
                    else if (skipParentJoin)
                        return $"{tablePrefix}{rootCol.Table.TableName}.{rootCol.Name}";
                    else
                        return $"{tablePrefix}{o.Table.TableName}.{o.Name}";
                }));
            selectStatement = new SqlStatement($"SELECT {columnList} ").Append(selectStatement);


            // We want to process each query and build WHERE clauses - these where clauses are based off of the JSON / XML names
            // on the model, so we have to use those for the time being before translating to SQL
            List<KeyValuePair<String, Object>> workingParameters = new List<KeyValuePair<string, object>>(query);

            // Where clause
            SqlStatement whereClause = new SqlStatement();
            List<SqlStatement> cteStatements = new List<SqlStatement>();

            // Construct
            while (workingParameters.Count > 0)
            {
                var parm = workingParameters.First();
                workingParameters.RemoveAt(0);

                // Match the regex and process
                var propertyPredicate = QueryPredicate.Parse(parm.Key);
                if (propertyPredicate == null) throw new ArgumentOutOfRangeException(parm.Key);

                // Next, we want to construct the 
                var otherParms = workingParameters.Where(o => QueryPredicate.Parse(o.Key).ToString(QueryPredicatePart.PropertyAndCast) == propertyPredicate.ToString(QueryPredicatePart.PropertyAndCast)).ToArray();

                // Remove the working parameters if the column is FK then all parameters
                if (otherParms.Any() || !String.IsNullOrEmpty(propertyPredicate.Guard) || !String.IsNullOrEmpty(propertyPredicate.SubPath))
                {
                    foreach (var o in otherParms)
                        workingParameters.Remove(o);

                    // We need to do a sub query

                    IEnumerable<KeyValuePair<String, Object>> queryParms = new List<KeyValuePair<String, Object>>() { parm }.Union(otherParms);

                    // Grab the appropriate builder
                    var subProp = typeof(TModel).GetQueryProperty(propertyPredicate.Path, true);
                    if (subProp == null) throw new MissingMemberException(propertyPredicate.Path);

                    // Link to this table in the other?
                    // Is this a collection?
                    if (!this.m_hacks.Any(o => o.HackQuery(this, selectStatement, whereClause, typeof(TModel), subProp, tablePrefix, propertyPredicate, parm.Value, scopedTables)))
                    {
                        if (typeof(IList).GetTypeInfo().IsAssignableFrom(subProp.PropertyType.GetTypeInfo())) // Other table points at this on
                        {
                            var propertyType = subProp.PropertyType.StripGeneric();
                            // map and get ORM def'n
                            var subTableType = m_mapper.MapModelType(propertyType);
                            var subTableMap = TableMapping.Get(subTableType);
                            var linkColumns = subTableMap.Columns.Where(o => scopedTables.Any(s => s.OrmType == o.ForeignKey?.Table));
                            var linkColumn = linkColumns.Count() > 1 ? linkColumns.FirstOrDefault(o => propertyPredicate.SubPath.StartsWith("source") ? o.SourceProperty.Name != "SourceUuid" : o.SourceProperty.Name == "SourceUuid") : linkColumns.FirstOrDefault();
                            // Link column is null, is there an assoc attrib?
                            SqlStatement subQueryStatement = new SqlStatement();

                            var subTableColumn = linkColumn;
                            string existsClause = String.Empty;

                            if (linkColumn == null)
                            {
                                var tableWithJoin = scopedTables.Select(o => o.AssociationWith(subTableMap)).FirstOrDefault();
                                linkColumn = tableWithJoin.Columns.SingleOrDefault(o => scopedTables.Any(s => s.OrmType == o.ForeignKey?.Table));
                                var targetColumn = tableWithJoin.Columns.SingleOrDefault(o => o.ForeignKey?.Table == subTableMap.OrmType);
                                subTableColumn = subTableMap.GetColumn(targetColumn.ForeignKey.Column);
                                // The sub-query statement needs to be joined as well 
                                var lnkPfx = IncrementSubQueryAlias(tablePrefix);
                                subQueryStatement.Append($"SELECT {lnkPfx}{tableWithJoin.TableName}.{linkColumn.Name} FROM {tableWithJoin.TableName} AS {lnkPfx}{tableWithJoin.TableName} WHERE ");
                                existsClause = $"{lnkPfx}{tableWithJoin.TableName}.{targetColumn.Name}";
                                //throw new InvalidOperationException($"Cannot find foreign key reference to table {tableMap.TableName} in {subTableMap.TableName}");
                            }

                            // Local Table
                            var localTable = scopedTables.Where(o => o.GetColumn(linkColumn.ForeignKey.Column) != null).FirstOrDefault();
                            if (String.IsNullOrEmpty(existsClause))
                                existsClause = $"{tablePrefix}{localTable.TableName}.{localTable.GetColumn(linkColumn.ForeignKey.Column).Name}";

                            // Guards
                            var guardConditions = queryParms.GroupBy(o => QueryPredicate.Parse(o.Key).Guard);
                            int nGuards = 0;
                            foreach (var guardClause in guardConditions)
                            {

                                var subQuery = guardClause.Select(o => new KeyValuePair<String, Object>(QueryPredicate.Parse(o.Key).ToString(QueryPredicatePart.SubPath), o.Value)).ToList();

                                // TODO: GUARD CONDITION HERE!!!!
                                if (!String.IsNullOrEmpty(guardClause.Key))
                                {
                                    StringBuilder guardCondition = new StringBuilder();
                                    var clsModel = propertyType;
                                    while (clsModel.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>() != null)
                                    {
                                        var clsProperty = clsModel.GetRuntimeProperty(clsModel.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>().ClassifierProperty);
                                        clsModel = clsProperty.PropertyType.StripGeneric();
                                        var redirectProperty = clsProperty.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty;
                                        if (redirectProperty != null)
                                            clsProperty = clsProperty.DeclaringType.GetRuntimeProperty(redirectProperty);

                                        guardCondition.Append(clsProperty.GetCustomAttributes<XmlElementAttribute>().First().ElementName);
                                        if (typeof(IdentifiedData).GetTypeInfo().IsAssignableFrom(clsModel.GetTypeInfo()))
                                            guardCondition.Append(".");
                                    }
                                    subQuery.Add(new KeyValuePair<string, object>(guardCondition.ToString(), guardClause.Key.Split('|')));
                                }

                                // Generate method
                                var prefix = IncrementSubQueryAlias(tablePrefix);
                                var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { propertyType }, new Type[] { subQuery.GetType(), typeof(String), typeof(bool), typeof(ColumnMapping[]) });

                                // Sub path is specified
                                if (String.IsNullOrEmpty(propertyPredicate.SubPath) && "null".Equals(parm.Value))
                                    subQueryStatement.And($" {existsClause} NOT IN (");
                                else
                                    subQueryStatement.And($" {existsClause} IN (");

                                nGuards++;
                                existsClause = $"{prefix}{subTableColumn.Table.TableName}.{subTableColumn.Name}";

                                if (subQuery.Count(p => !p.Key.Contains(".")) == 0)
                                    subQueryStatement.Append(genMethod.Invoke(this, new Object[] { subQuery, prefix, true, new ColumnMapping[] { subTableColumn } }) as SqlStatement);
                                else
                                    subQueryStatement.Append(genMethod.Invoke(this, new Object[] { subQuery, prefix, false, new ColumnMapping[] { subTableColumn } }) as SqlStatement);


                                //// TODO: Check if limiting the the query is better
                                //if (guardConditions.Last().Key != guardClause.Key)
                                //    subQueryStatement.Append(" INTERSECT ");
                            }

                            // Unwind guards
                            while (nGuards-- > 0)
                                subQueryStatement.Append(")");

                            if (subTableColumn != linkColumn)
                                whereClause.And($"{tablePrefix}{localTable.TableName}.{localTable.GetColumn(linkColumn.ForeignKey.Column).Name} IN (").Append(subQueryStatement).Append(")");
                            else
                                whereClause.And(subQueryStatement);
                        }
                        else // this table points at other
                        {
                            var subQuery = queryParms.Select(o => new KeyValuePair<String, Object>(QueryPredicate.Parse(o.Key).ToString(QueryPredicatePart.SubPath), o.Value)).ToList();
                            TableMapping tableMapping = null;
                            var subPropKey = typeof(TModel).GetQueryProperty(propertyPredicate.Path);

                            // Get column info
                            PropertyInfo domainProperty = scopedTables.Select(o => { tableMapping = o; return m_mapper.MapModelProperty(typeof(TModel), o.OrmType, subPropKey); })?.FirstOrDefault(o => o != null);
                            ColumnMapping linkColumn = null;
                            // If the domain property is not set, we may have to infer the link
                            if (domainProperty == null)
                            {
                                var subPropType = m_mapper.MapModelType(subProp.PropertyType);
                                // We find the first column with a foreign key that points to the other !!!
                                linkColumn = scopedTables.SelectMany(o => o.Columns).FirstOrDefault(o => o.ForeignKey?.Table == subPropType);
                            }
                            else
                                linkColumn = tableMapping.GetColumn(domainProperty);

                            var fkTableDef = TableMapping.Get(linkColumn.ForeignKey.Table);
                            var fkColumnDef = fkTableDef.GetColumn(linkColumn.ForeignKey.Column);

                            // Create the sub-query
                            SqlStatement subQueryStatement = null;
                            var subSkipJoins = subQuery.Count(o => !o.Key.Contains(".") && o.Key != "obsoletionTime") == 0;

                            if (String.IsNullOrEmpty(propertyPredicate.CastAs))
                            {
                                var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { subProp.PropertyType }, new Type[] { subQuery.GetType(), typeof(string), typeof(bool), typeof(ColumnMapping[]) });
                                subQueryStatement = genMethod.Invoke(this, new Object[] { subQuery, null, subSkipJoins, new ColumnMapping[] { fkColumnDef } }) as SqlStatement;
                            }
                            else // we need to cast!
                            {
                                var castAsType = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType("OpenIZ.Core.Model", propertyPredicate.CastAs);

                                var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { castAsType }, new Type[] { subQuery.GetType(), typeof(String), typeof(bool), typeof(ColumnMapping[]) });
                                subQueryStatement = genMethod.Invoke(this, new Object[] { subQuery, null, false, new ColumnMapping[] { fkColumnDef } }) as SqlStatement;
                            }

                            cteStatements.Add(new SqlStatement($"{tablePrefix}cte{cteStatements.Count} AS (").Append(subQueryStatement).Append(")"));

                            //subQueryStatement.And($"{tablePrefix}{tableMapping.TableName}.{linkColumn.Name} = {sqName}{fkTableDef.TableName}.{fkColumnDef.Name} ");

                            //selectStatement.Append($"INNER JOIN {tablePrefix}cte{cteStatements.Count - 1} ON ({tablePrefix}{tableMapping.TableName}.{linkColumn.Name} = {tablePrefix}cte{cteStatements.Count - 1}.{fkColumnDef.Name})");
                            whereClause.And($"{tablePrefix}{tableMapping.TableName}.{linkColumn.Name} IN (SELECT {tablePrefix}cte{cteStatements.Count - 1}.{fkColumnDef.Name} FROM {tablePrefix}cte{cteStatements.Count - 1})");

                        }

                    }
                }
                else if (!this.m_hacks.Any(o => o.HackQuery(this, selectStatement, whereClause, typeof(TModel), typeof(TModel).GetQueryProperty(propertyPredicate.Path), tablePrefix, propertyPredicate, parm.Value, scopedTables)))
                    whereClause.And(CreateWhereCondition(typeof(TModel), propertyPredicate.Path, parm.Value, tablePrefix, scopedTables));

            }

            // Return statement
            SqlStatement retVal = new SqlStatement();
            if (cteStatements.Count > 0)
            {
                retVal.Append("WITH ");
                foreach (var c in cteStatements)
                {
                    retVal.Append(c);
                    if (c != cteStatements.Last())
                        retVal.Append(",");
                }
            }
            retVal.Append(selectStatement.Where(whereClause));
            return retVal;
        }

        /// <summary>
        /// Increment sub-query alias
        /// </summary>
        private static String IncrementSubQueryAlias(string tablePrefix)
        {
            if (String.IsNullOrEmpty(tablePrefix))
                return "sq0";
            else
            {
                int sq = 0;
                if (Int32.TryParse(tablePrefix.Substring(2), out sq))
                    return "sq" + (sq + 1);
                else
                    return "sq0";
            }
        }

        /// <summary>
        /// Create a single where condition based on the property info
        /// </summary>
        public SqlStatement CreateWhereCondition(Type tmodel, String propertyPath, Object value, String tablePrefix, List<TableMapping> scopedTables, String tableAlias = null)
        {

            SqlStatement retVal = new SqlStatement();

            // Map the type
            var tableMapping = scopedTables.First();
            var propertyInfo = tmodel.GetQueryProperty(propertyPath);
            if (propertyInfo == null)
                throw new ArgumentOutOfRangeException(propertyPath);
            PropertyInfo domainProperty = scopedTables.Select(o => { tableMapping = o; return m_mapper.MapModelProperty(tmodel, o.OrmType, propertyInfo); }).FirstOrDefault(o => o != null);

            // Now map the property path
            if(String.IsNullOrEmpty(tableAlias))
                tableAlias = $"{tablePrefix}{tableMapping.TableName}";

            if (domainProperty == null)
                throw new ArgumentException($"Can't find SQL based property for {propertyPath} on {tableMapping.TableName}");
            var columnData = tableMapping.GetColumn(domainProperty);

            // List of parameters
            var lValue = value as IList;
            if (lValue == null)
                lValue = new List<Object>() { value };

            retVal.Append("(");
            foreach (var itm in lValue)
            {
                retVal.Append($"{tableAlias}.{columnData.Name}");
                var semantic = " OR ";
                var iValue = itm;
                if (iValue is String)
                {
                    var sValue = itm as String;
                    switch (sValue[0])
                    {
                        case '<':
                            semantic = " AND ";
                            if (sValue[1] == '=')
                                retVal.Append(" <= ?", CreateParameterValue(sValue.Substring(2), propertyInfo.PropertyType));
                            else
                                retVal.Append(" < ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '>':
                            semantic = " AND ";
                            if (sValue[1] == '=')
                                retVal.Append(" >= ?", CreateParameterValue(sValue.Substring(2), propertyInfo.PropertyType));
                            else
                                retVal.Append(" > ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '!':
                            semantic = " AND ";
                            if (sValue.Equals("!null"))
                                retVal.Append(" IS NOT NULL");
                            else
                                retVal.Append(" <> ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '~':
                            if (sValue.Contains("*") || sValue.Contains("?"))
                                retVal.Append(" LIKE ? ", CreateParameterValue(sValue.Substring(1).Replace("*","%"), propertyInfo.PropertyType));
                            else
                                retVal.Append(" LIKE '%' || ? || '%'", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '^':
                            retVal.Append(" LIKE ? || '%'", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '$':
                            retVal.Append(" LIKE '%' || ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        default:
                            if (sValue.Equals("null"))
                                retVal.Append(" IS NULL");
                            else
                                retVal.Append(" = ? ", CreateParameterValue(sValue, propertyInfo.PropertyType));
                            break;
                    }
                }
                else
                    retVal.Append(" = ? ", CreateParameterValue(iValue, propertyInfo.PropertyType));

                if (lValue.IndexOf(itm) < lValue.Count - 1)
                    retVal.Append(semantic);
            }

            retVal.Append(")");

            return retVal;
        }

        /// <summary>
        /// Create a parameter value
        /// </summary>
        private static object CreateParameterValue(object value, Type toType)
        {
            object retVal = null;
            if (value is Guid)
                return ((Guid)value).ToByteArray();
            else if (value.GetType() == toType ||
                value.GetType() == toType.StripNullable())
                return value;
            else if (MapUtil.TryConvert(value, toType, out retVal))
                return retVal;
            else
                throw new ArgumentOutOfRangeException(value.ToString());
        }
    }
}
