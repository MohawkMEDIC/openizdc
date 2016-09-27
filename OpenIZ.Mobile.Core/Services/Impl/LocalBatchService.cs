using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using System;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Local batch service
	/// </summary>
	public class LocalBatchService : IBatchRepositoryService
	{
		/// <summary>
		/// Insert the bundle
		/// </summary>
		public Bundle Insert(Bundle data)
		{
			data = this.Validate(data);
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
			if (persistence == null)
				throw new InvalidOperationException("Missing persistence service");
			return persistence.Insert(data);
		}

		/// <summary>
		/// Obsolete all the contents in the bundle
		/// </summary>
		public Bundle Obsolete(Bundle obsolete)
		{
			obsolete = this.Validate(obsolete);
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
			if (persistence == null)
				throw new InvalidOperationException("Missing persistence service");
			return persistence.Obsolete(obsolete);
		}

		/// <summary>
		/// Update the specified data in the bundle
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public Bundle Update(Bundle data)
		{
			data = this.Validate(data);
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
			if (persistence == null)
				throw new InvalidOperationException("Missing persistence service");
			return persistence.Insert(data);
		}

		/// <summary>
		/// Validate the bundle and its contents
		/// </summary>
		public Bundle Validate(Bundle bundle)
		{
			bundle = bundle.Clean() as Bundle;
			for (int i = 0; i < bundle.Item.Count; i++)
			{
				var itm = bundle.Item[i];
				if (itm is Patient)
					bundle.Item[i] = ApplicationContext.Current.GetService<IPatientRepositoryService>().Validate(itm as Patient);
				else if (itm is Act)
					bundle.Item[i] = ApplicationContext.Current.GetService<IActRepositoryService>().Validate(itm as Act);
			}
			return bundle;
		}
	}
}