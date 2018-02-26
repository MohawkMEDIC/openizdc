/*
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
using OpenIZ.Core.Data.QueryBuilder;
using SQLite.Net;
using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Connection
{

    public static class SQLiteCommandBuilder
    {
        /// <summary>
        /// Get member information from lambda
        /// </summary>
        private static MemberInfo GetMember(Expression expression)
        {
            if (expression is MemberExpression) return (expression as MemberExpression).Member;
            else if (expression is UnaryExpression) return GetMember((expression as UnaryExpression).Operand);
            else if (expression is LambdaExpression) return GetMember((expression as LambdaExpression).Body);
            else throw new InvalidOperationException($"{expression} not supported, please use a member access expression");
        }

        /// <summary>
        /// Build delete text
        /// </summary>
        public static String Delete(Type objectType)
        {
            StringBuilder sb = new StringBuilder("DELETE FROM ");
            var mapping = OpenIZ.Core.Data.QueryBuilder.TableMapping.Get(objectType);
            sb.Append(mapping.TableName);
            sb.Append(" WHERE ");
            sb.Append(mapping.Columns.FirstOrDefault(o => o.IsPrimaryKey).Name);
            sb.Append(" = ?;");
            return sb.ToString();
        }

        /// <summary>
        /// Build delete text
        /// </summary>
        public static String Delete<T>(params Expression<Func<T, dynamic>>[] whereProperties)
        {
            StringBuilder sb = new StringBuilder("DELETE FROM ");
            var mapping = OpenIZ.Core.Data.QueryBuilder.TableMapping.Get(typeof(T));
            sb.Append(mapping.TableName);
            sb.Append(" WHERE ");

            if (whereProperties == null || whereProperties.Length == 0)
            {
                sb.Append(mapping.Columns.FirstOrDefault(o => o.IsPrimaryKey).Name);
                sb.Append(" = ?;");
            }
            else
            {
                for (int i = 0; i < whereProperties.Length; i++)
                {
                    var getMember = GetMember(whereProperties[i]);
                    var column = mapping.GetColumn(getMember);
                    sb.AppendFormat("{0} = ?", column.Name);
                    if (i < whereProperties.Length - 1)
                        sb.AppendFormat(" AND ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Build insert statement
        /// </summary>
        public static String Insert<T>()
        {
            return Insert(typeof(T));
        }

        /// <summary>
        /// Build insert statement
        /// </summary>
        public static string Insert(Type objectType)
        {
            StringBuilder sb = new StringBuilder("INSERT INTO ");
            var mapping = OpenIZ.Core.Data.QueryBuilder.TableMapping.Get(objectType);
            sb.Append(mapping.TableName);
            sb.Append("(");
            var columns = mapping.Columns.ToArray();

            for (int i = 0; i < columns.Count(); i++)
            {
                sb.Append(columns[i].Name);
                if (i < columns.Length - 1)
                    sb.Append(",");
            }
            sb.Append(") VALUES (");
            for (int i = 0; i < columns.Length; i++)
                sb.Append("?,");
            sb.Remove(sb.Length - 1, 1);
            sb.Append(");");
            return sb.ToString();
        }

    }
    /// <summary>
    /// Extensions to SQLite connection
    /// </summary>
    public static class SQLiteConnectionExtensions
    {
        // Negative PTR
        public static readonly IntPtr NEGATIVE_PTR = new IntPtr(-1);

       
        /// <summary>
        /// Creates a prepared statmeent
        /// </summary>
        public static IDbStatement Prepare(this SQLiteConnection me, String cmd)
        {
            var sqlite3 = ApplicationContext.Current.GetService<ISQLitePlatform>().SQLiteApi;
            return sqlite3.Prepare2(me.Handle, cmd);
        }

        /// <summary>
        /// Prepare an insert statement
        /// </summary>
        public static IDbStatement PrepareInsert(this SQLiteConnection me, Type objectType)
        {
            // Prepare 
            return me.Prepare(SQLiteCommandBuilder.Insert(objectType));
        }

        /// <summary>
        /// Prepare an insert statement
        /// </summary>
        public static IDbStatement PrepareDelete(this SQLiteConnection me, Type objectType)
        {
            // Prepare 
            return me.Prepare(SQLiteCommandBuilder.Delete(objectType));
        }

        /// <summary>
        /// Prepare a delete statement
        /// </summary>
        public static IDbStatement PrepareDelete<T>(this SQLiteConnection me, params Expression<Func<T, dynamic>>[] whereProperties)
        {
            // Prepare 
            return me.Prepare(SQLiteCommandBuilder.Delete<T>(whereProperties));

        }
        
        /// <summary>
        /// Prepare a insert statement
        /// </summary>
        public static IDbStatement PrepareInsert<T>(this SQLiteConnection me)
        {
            return me.PrepareInsert(typeof(T));
        }


        /// <summary>
        /// Bind insert value
        /// </summary>
        public static void BindInsert(this IDbStatement me, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var mapping = OpenIZ.Core.Data.QueryBuilder.TableMapping.Get(value.GetType());

            object[] values = new object[mapping.Columns.Count()];
            int idx = 0;
            // Bind the insert columns
            foreach (var col in mapping.Columns)
                values[idx++] = col.SourceProperty.GetValue(value);

            me.BindParameters(values);
        }

        /// <summary>
        /// Bind parameters
        /// </summary>
        public static void BindParameters(this IDbStatement me, params object[] parameters)
        {
            var sqlite3 = ApplicationContext.Current.GetService<ISQLitePlatform>().SQLiteApi;
            int nidx = 1;
            foreach (var obj in parameters)
            {
                if (obj is Guid)
                    sqlite3.BindBlob(me, nidx++, ((Guid)obj).ToByteArray(), 16, NEGATIVE_PTR);
                else if (obj is String)
                    sqlite3.BindText16(me, nidx++, obj as String, -1, NEGATIVE_PTR);
                else if (obj is Byte || obj is UInt16 || obj is SByte || obj is Int16)
                    sqlite3.BindInt(me, nidx++, Convert.ToInt32(obj));
                else if (obj is Boolean)
                    sqlite3.BindInt(me, nidx++, (bool)obj ? 1 : 0);
                else if (obj is UInt32 || obj is Int64)
                    sqlite3.BindInt64(me, nidx++, Convert.ToInt64(obj));
                else if (obj is Single || obj is Double || obj is Decimal)
                    sqlite3.BindDouble(me, nidx++, Convert.ToDouble(obj));
                else if (obj is TimeSpan)
                    sqlite3.BindInt64(me, nidx++, ((TimeSpan)obj).Ticks);
                else if (obj is DateTime)
                    sqlite3.BindInt64(me, nidx++, ((DateTime)obj).Ticks);
                else if (obj is byte[])
                    sqlite3.BindBlob(me, nidx++, (byte[])obj, ((byte[])obj).Length, NEGATIVE_PTR);
                else
                    sqlite3.BindNull(me, nidx++);
            }
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        public static void ExecutePreparedNonQuery(this IDbStatement me, bool reuse = true)
        {
            var sqlite3 = ApplicationContext.Current.GetService<ISQLitePlatform>().SQLiteApi;
            var r = sqlite3.Step(me);
            if (r != Result.Done)
                throw SQLiteException.New(r, "Error executing SQLite prepared statement");
            if (reuse)
                sqlite3.Reset(me);
        }

        /// <summary>
        /// Finalize the statement
        /// </summary>
        public static void Finalize(this IDbStatement me)
        {
            var sqlite3 = ApplicationContext.Current.GetService<ISQLitePlatform>().SQLiteApi;
            sqlite3.Finalize(me);
        }
    }
}
