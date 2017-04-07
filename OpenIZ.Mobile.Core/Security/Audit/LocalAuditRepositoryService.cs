using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Auditing.Data;
using System.Linq.Expressions;
using SQLite.Net;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Core.Model.Map;
using System.Reflection;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Mobile.Core.Security.Audit.Model;

namespace OpenIZ.Mobile.Core.Security.Audit
{
    /// <summary>
    /// Local audit repository service
    /// </summary>
    public class LocalAuditRepositoryService : IAuditRepositoryService
    {

        // Model mapper
        private ModelMapper m_mapper = new ModelMapper(typeof(LocalAuditRepositoryService).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Security.Audit.Model.ModelMap.xml"));

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalAuditRepositoryService));

        /// <summary>
        /// Create a connection
        /// </summary>
        /// <returns>The connection.</returns>
        private SQLiteConnectionWithLock CreateConnection()
        {
            return SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current.Configuration.GetConnectionString(
                "openIzAudit"
            ).Value);
        }

        /// <summary>
        /// Finds the specified audit in the data repository
        /// </summary>
        public IEnumerable<AuditData> Find(Expression<Func<AuditData, bool>> query)
        {
            int tr = 0;
            return this.Find(query, 0, null, out tr);
        }

        /// <summary>
        /// Fids the specified audit in the local repository
        /// </summary>
        public IEnumerable<AuditData> Find(Expression<Func<AuditData, bool>> query, int offset, int? count, out int totalResults)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert audit data
        /// </summary>
        public AuditData Insert(AuditData audit)
        {
            var conn = this.CreateConnection();
            using(conn.Lock())
            {
                try
                {
                    conn.BeginTransaction();

                    // Insert core
                    var dbAudit = this.m_mapper.MapModelInstance<AuditData, DbAuditData>(audit);
                    dbAudit.CreationTime = DateTime.Now;
                    dbAudit.Id = conn.Insert(dbAudit);

                    // Insert secondary properties
                    if(audit.Actors != null)
                        foreach(var act in audit.Actors)
                        {
                            var dbAct = conn.Table<DbAuditActor>().Where(o => o.UserName == act.UserName).FirstOrDefault();
                            if(dbAct == null)
                            {
                                dbAct = this.m_mapper.MapModelInstance<AuditActorData, DbAuditActor>(act);
                                dbAct.Id = conn.Insert(dbAct);
                            }
                            conn.Insert(new DbAuditActorAssociation()
                            {
                                ActorId = dbAct.Id,
                                AuditId = dbAudit.Id
                            });
                        }

                    // Audit objects
                    if(audit.AuditableObjects != null)
                        foreach(var ao in audit.AuditableObjects)
                        {
                            var dbAo = this.m_mapper.MapModelInstance<AuditableObject, DbAuditObject>(ao);
                            dbAo.AuditId = dbAudit.Id;
                            conn.Insert(dbAo);
                        }

                    conn.Commit();

                    // Add audit info
                    audit.AuditableObjects.Add(new AuditableObject()
                    {
                        IDTypeCode = AuditableObjectIdType.ReportNumber,
                        LifecycleType = AuditableObjectLifecycle.Creation,
                        ObjectId = dbAudit.Id.ToString(),
                        Role = AuditableObjectRole.SecurityResource,
                        Type = AuditableObjectType.SystemObject
                    });

                    return audit;
                }
                catch (Exception ex)
                {
                    conn.Rollback();
                    this.m_tracer.TraceError("Error inserting audit: {0}", ex);
                    throw;
                }
            }
        }
    }
}
