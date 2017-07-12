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
 * Date: 2017-6-28
 */
using Mono.Data.Sqlite;
using OpenIZ.Mobile.Reporting;
using OpenIZ.Mobile.Reporting.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Data
{
    /// <summary>
    /// A report source which uses SQLite
    /// </summary>
    public class SQLiteReportDatasource : IReportDatasource
    {
        /// <summary>
        /// Get the name of the report datasource
        /// </summary>
        public string Name
        {
            get
            {
                return "sqlite";
            }
        }

        /// <summary>
        /// Executes the specified dataset
        /// </summary>
        public IEnumerable<dynamic> ExecuteDataset(List<ReportConnectionString> connectionString, string sql, List<object> sqlParms)
        {
            // Lock the main database
            var connectionStringPath = ApplicationContext.Current.Configuration.GetConnectionString(connectionString.First(o => String.IsNullOrEmpty(o.Identifier)).Value).Value;
            //var connection = OpenIZ.Mobile.Core.Data.Connection.SQLiteConnectionManager.Current.GetConnection(connectionStringPath);
            //using (connection.Lock())
            using (var conn = new SqliteConnection($"Data Source={connectionStringPath}"))
            {

                // Create command on main datasource
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        //connection.Close();

                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        foreach (var itm in sqlParms)
                        {
                            var parm = cmd.CreateParameter();
                            parm.Value = itm;
                            if (itm is String) parm.DbType = System.Data.DbType.String;
                            else if (itm is DateTime || itm is DateTimeOffset)
                            {
                                parm.DbType = System.Data.DbType.Int64;
                                if (itm is DateTime)
                                    parm.Value = ((DateTime)itm).ToUniversalTime().Ticks;
                                else
                                    parm.Value = ((DateTimeOffset)itm).ToUniversalTime().Ticks;
                            }
                            else if (itm is Int32) parm.DbType = System.Data.DbType.Int32;
                            else if (itm is Boolean) parm.DbType = System.Data.DbType.Boolean;
                            else if (itm is byte[])
                            {
                                parm.DbType = System.Data.DbType.Binary;
                                parm.Value = itm;
                            }
                            else if (itm is Guid || itm is Guid?)
                            {
                                parm.DbType = System.Data.DbType.Binary;
                                if (itm != null)
                                    parm.Value = ((Guid)itm).ToByteArray();
                                else parm.Value = DBNull.Value;
                            }
                            else if (itm is float || itm is double) parm.DbType = System.Data.DbType.Double;
                            else if (itm is Decimal) parm.DbType = System.Data.DbType.Decimal;
                            else if (itm == null)
                            {
                                parm.Value = DBNull.Value;
                            }
                            cmd.Parameters.Add(parm);
                        }

                        // data reader
                        try
                        {
                            conn.SetPassword(ApplicationContext.Current.GetCurrentContextSecurityKey());
                            conn.Open();
                            // Attach further connection strings
                            foreach (var itm in connectionString.Where(o => !String.IsNullOrEmpty(o.Identifier)))
                            {
                                using (var attcmd = conn.CreateCommand())
                                {
                                    if(ApplicationContext.Current.GetCurrentContextSecurityKey() == null)
                                        attcmd.CommandText = $"ATTACH DATABASE '{ApplicationContext.Current.Configuration.GetConnectionString(itm.Value).Value}' AS {itm.Identifier} KEY ''";
                                    else
                                        attcmd.CommandText = $"ATTACH DATABASE '{ApplicationContext.Current.Configuration.GetConnectionString(itm.Value).Value}' AS {itm.Identifier} KEY X'{BitConverter.ToString(ApplicationContext.Current.GetCurrentContextSecurityKey()).Replace("-","")}'";

                                    attcmd.CommandType = System.Data.CommandType.Text;
                                    attcmd.ExecuteNonQuery();
                                }
                            }

                            using (var dr = cmd.ExecuteReader())
                            {
                                var retVal = new List<Object>();
                                while (dr.Read())
                                    retVal.Add(this.MapExpando<ExpandoObject>(dr));
                                return retVal;
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                    finally
                    {
                        //connection.ope
                        //OpenIZ.Mobile.Core.Data.Connection.SQLiteConnectionManager.Current.Remove(connection);
                    }
                }
            }
        }

        /// <summary>
        /// Map expando object
        /// </summary>
        private TModel MapExpando<TModel>(DbDataReader rdr)
        {
            var retVal = new ExpandoObject() as IDictionary<String, Object>;
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                var value = rdr[i];
                var name = rdr.GetName(i);
                if (value == DBNull.Value)
                    value = null;
                if (value is byte[] && (value as byte[]).Length == 16)
                    value = new Guid(value as byte[]);
                else if (
                    (name.ToLower().Contains("date") ||
                    name.ToLower().Contains("time") ||
                    name.ToLower().Contains("utc")) && (value is int || value is long))
                    value = new DateTime(value is int ? (int)value : (long)value, DateTimeKind.Utc);
                retVal.Add(name, value);
            }
            return (TModel)retVal;
        }
    }
}
