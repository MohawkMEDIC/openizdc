using Mono.Data.Sqlite;
using OpenIZ.Mobile.Reporting;
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
        public IEnumerable<dynamic> ExecuteDataset(string connectionString, string sql, List<object> sqlParms)
        {
            // Lock the main database
            var connectionStringPath = ApplicationContext.Current.Configuration.GetConnectionString(connectionString).Value;
            var connection = OpenIZ.Mobile.Core.Data.Connection.SQLiteConnectionManager.Current.GetConnection(connectionStringPath);
            using (connection.Lock())
            using (var conn = new SqliteConnection(connectionStringPath))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                foreach (var p in sqlParms)
                {
                    var parm = cmd.CreateParameter();
                    parm.Value = p;
                }

                // data reader
                using (var dr = cmd.ExecuteReader())
                {
                    var retVal = new List<Object>();
                    while (dr.Read())
                        retVal.Add(this.MapExpando<ExpandoObject>(dr));
                    return retVal;
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
                retVal.Add(name, value);
            }
            return (TModel)retVal;
        }
    }
}
