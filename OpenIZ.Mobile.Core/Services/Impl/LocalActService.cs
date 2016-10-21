using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Represents an act repository service.
	/// </summary>
	public class LocalActService : IActRepositoryService
	{
		/// <summary>
		/// Finds acts based on a specific query.
		/// </summary>
		public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> filter, int offset, int? count, out int totalResults) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept persistence service found");

			return persistenceService.Query(filter, offset, count, out totalResults);
		}

		/// <summary>
		/// Get the specified act
		/// </summary>
		public TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			return persistenceService.Get(key);
		}

		/// <summary>
		/// Insert the specified act
		/// </summary>
		public TAct Insert<TAct>(TAct insert) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			return persistenceService.Insert(insert);
		}

		/// <summary>
		/// Obsolete the specified act
		/// </summary>
		public TAct Obsolete<TAct>(Guid key) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			var act = persistenceService.Get(key);

			if (act == null)
			{
				throw new InvalidOperationException("Act not found");
			}

			return persistenceService.Obsolete(act);
		}

		/// <summary>
		/// Insert or update the specified act
		/// </summary>
		/// <param name="act"></param>
		/// <returns></returns>
		public TAct Save<TAct>(TAct act) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			try
			{
				return persistenceService.Update(act);
			}
			catch (KeyNotFoundException)
			{
				return persistenceService.Insert(act);
			}
		}

		/// <summary>
		/// Validates an act.
		/// </summary>
		public TAct Validate<TAct>(TAct data) where TAct : Act
		{
			// Correct author information and controlling act information
			data = data.Clean() as TAct;

			ISecurityRepositoryService userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var currentUserEntity = AuthenticationContext.Current.Session?.UserEntity;

			if (currentUserEntity != null && !data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator))
			{
				data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));
			}

			return data;
		}
	}
}