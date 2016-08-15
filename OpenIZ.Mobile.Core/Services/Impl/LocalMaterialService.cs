using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Local material service
    /// </summary>
    public class LocalMaterialService : IMaterialRepositoryService
    {
        /// <summary>
        /// Find the specified manufactured material
        /// </summary>
        public IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();
            if (pers == null)
                throw new InvalidOperationException("Persistence service not found");
            return pers.Query(expression);
        }

        /// <summary>
        /// Find manufactured material with the specified controls
        /// </summary>
        public IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression, int offset, int count, out int totalCount)
        {
            var pers = ApplicationContext.Current.GetService<IDataPersistenceService<ManufacturedMaterial>>();
            if (pers == null)
                throw new InvalidOperationException("Persistence service not found");
            return pers.Query(expression, offset, count, out totalCount);
        }

        public IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression, int offset, int count, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public ManufacturedMaterial GetManufacturedMaterial(Guid id, Guid versionId)
        {
            throw new NotImplementedException();
        }

        public Material GetMaterial(Guid id, Guid versionId)
        {
            throw new NotImplementedException();
        }

        public ManufacturedMaterial InsertManufacturedMaterial(ManufacturedMaterial ManufacturedMaterial)
        {
            throw new NotImplementedException();
        }

        public Material InsertMaterial(Material material)
        {
            throw new NotImplementedException();
        }

        public ManufacturedMaterial ObsoleteManufacturedMaterial(Guid key)
        {
            throw new NotImplementedException();
        }

        public Material ObsoleteMaterial(Guid key)
        {
            throw new NotImplementedException();
        }

        public ManufacturedMaterial SaveManufacturedMaterial(ManufacturedMaterial ManufacturedMaterial)
        {
            throw new NotImplementedException();
        }

        public Material SaveMaterial(Material material)
        {
            throw new NotImplementedException();
        }
    }
}
