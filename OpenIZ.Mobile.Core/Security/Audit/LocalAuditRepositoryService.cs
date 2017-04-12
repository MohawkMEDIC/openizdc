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
        /// Ctor (prune on startup)
        /// </summary>
        public LocalAuditRepositoryService()
        {
            ApplicationContext.Current.Started += (o, e) =>
            {
                try
                {
                    this.Prune();
                }
                catch(Exception ex)
                {
                    this.m_tracer.TraceError("Error pruning audit repository: {0}", ex);
                }
            };
        }

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
        /// Prune the audit database
        /// </summary>
        public void Prune()
        {
            var config = ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>();

            this.m_tracer.TraceInfo("Prune audits older than {0}", config?.AuditRetention);
            var conn = this.CreateConnection();
            using (conn.Lock())
            {

            }

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
            try
            {
                totalResults = 0;
                return null;
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Could not query audit {0}: {1}", query, e);
                throw;
            }
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

                    var eventId = audit.EventTypeCode;
                    if (eventId != null)
                    {
                        var existing = conn.Table<DbAuditCode>().Where(o => o.Code == eventId.Code && o.CodeSystem == eventId.CodeSystem).FirstOrDefault();
                        if (existing == null)
                            dbAudit.EventTypeId = conn.Insert(new DbAuditCode() { Code = eventId.Code, CodeSystem = eventId.CodeSystem });
                        else
                            dbAudit.EventTypeId = existing.Id;
                    }

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
                                var roleCode = act.ActorRoleCode?.FirstOrDefault();
                                if (roleCode != null)
                                {
                                    var existing = conn.Table<DbAuditCode>().Where(o => o.Code == roleCode.Code && o.CodeSystem == roleCode.CodeSystem).FirstOrDefault();
                                    if (existing == null)
                                        dbAct.RoleCodeId = conn.Insert(new DbAuditCode() { Code = roleCode.Code, CodeSystem = roleCode.CodeSystem });
                                    else
                                        dbAct.RoleCodeId = existing.Id;
                                }

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
