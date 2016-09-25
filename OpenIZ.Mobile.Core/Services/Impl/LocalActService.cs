using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Acts;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Represents a local persistence service for acts
    /// </summary>
    public class LocalActService : IActRepositoryService
    {
        /// <summary>
        /// Finds the specified acts
        /// </summary>
        public IEnumerable<Act> FindActs(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();
            if (pers == null)
                throw new InvalidOperationException("Persistence service not found");
            return pers.Query(query, offset, count, out totalResults);
        }

        /// <summary>
        /// Find substance administrations
        /// </summary>
        public IEnumerable<SubstanceAdministration> FindSubstanceAdministrations(Expression<Func<SubstanceAdministration, bool>> filter, int offset, int? count, out int totalResults)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SubstanceAdministration>>();
            if (pers == null)
                throw new InvalidOperationException("Persistence service not found");
            return pers.Query(filter, offset, count, out totalResults);

        }

        /// <summary>
        /// Gets the specified act
        /// </summary>
        public Act Get(Guid key, Guid versionId)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<SubstanceAdministration>>();
            if (pers == null)
                throw new InvalidOperationException("Persistence service not found");
            return pers.Get(key);
        }

        /// <summary>
        /// Insert the specified act
        /// </summary>
        public Act Insert(Act data)
        {
            IDataPersistenceService pers = this.GetPersistenceService(data);

            // Persistence not found
            if (pers == null)
                throw new InvalidOperationException("Persistence service not found");

            // Insert
            data = this.Validate(data);
            return pers.Insert(data) as Act;
        }

        /// <summary>
        /// Get the specified persistence service
        /// </summary>
        private IDataPersistenceService GetPersistenceService(Act data)
        {
            IDataPersistenceService pers = null;
            // Get appropriate persister
            if (data is SubstanceAdministration)
                pers = ApplicationContext.Current.GetService<IDataPersistenceService<SubstanceAdministration>>();
            else if (data is TextObservation)
                pers = ApplicationContext.Current.GetService<IDataPersistenceService<TextObservation>>();
            else if (data is QuantityObservation)
                pers = ApplicationContext.Current.GetService<IDataPersistenceService<QuantityObservation>>();
            else if (data is CodedObservation)
                pers = ApplicationContext.Current.GetService<IDataPersistenceService<CodedObservation>>();
            else if (data is PatientEncounter)
                pers = ApplicationContext.Current.GetService<IDataPersistenceService<PatientEncounter>>();
            else if (data is ControlAct)
                pers = ApplicationContext.Current.GetService<IDataPersistenceService<ControlAct>>();
            else if (data is Act)
                pers = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();
            else
                throw new ArgumentOutOfRangeException(nameof(data));

            return pers;
        }

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Act Obsolete(Guid key)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();
            if (pers == null)
                throw new InvalidOperationException("Persistence service not found");
            return pers.Obsolete(new Act() { Key = key });
        }

        /// <summary>
        /// Save the specified act
        /// </summary>
        public Act Save(Act data)
        {
            IDataPersistenceService pers = this.GetPersistenceService(data);

            // Persistence not found
            if (pers == null)
                throw new InvalidOperationException("Persistence service not found");

            data = this.Validate(data);

            try // to insert
            {
                return pers.Update(data) as Act;
            }
            catch(KeyNotFoundException)
            {
                return pers.Insert(data) as Act;
            }
        }

        /// <summary>
        /// Validate data or rather prepare for insert
        /// </summary>
        public Act Validate(Act data)
        {
            // Correct author information and controlling act information
            data = data.Clean() as Act;
            ISecurityRepositoryService userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
            var currentUserEntity = userService.GetUserEntity(ApplicationContext.Current.Principal.Identity);
            if (!data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator))
                data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));

			//foreach (var relationship in data.Relationships)
			//{
			//	if (!relationship.TargetAct.Participations.Any(o => o.ParticipationRole?.Mnemonic == "RecordTarget"))
			//	{
			//		relationship.TargetAct.Participations.AddRange(data.Participations.Where(p => p.ParticipationRole?.Mnemonic == "RecordTarget"));
			//	}

			//	if (!relationship.TargetAct.Participations.Any(o => o.ParticipationRole?.Mnemonic == "Performer"))
			//	{
			//		relationship.TargetAct.Participations.AddRange(data.Participations.Where(p => p.ParticipationRole?.Mnemonic == "Performer"));
			//	}
			//}

			return data;
        }
    }
}
