using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services.Impl
{
    /// <summary>
    /// Tag persistence service for act
    /// </summary>
    public class LocalTagPersistenceService : ITagPersistenceService
    {
        /// <summary>
        /// Save tag
        /// </summary>
        public void Save(Guid sourceKey, ITag tag)
        {

            if (tag is EntityTag)
            {
                var idp = ApplicationContext.Current.GetService<IDataPersistenceService<EntityTag>>();
                var existing = idp.Query(o => o.SourceEntityKey == sourceKey && o.TagKey == tag.TagKey).FirstOrDefault();
                if (existing != null)
                {
                    existing.Value = tag.Value;
                    if (existing.Value == null)
                        idp.Obsolete(existing);
                    else
                        idp.Update(existing as EntityTag);
                }
                else
                    idp.Insert(tag as EntityTag);
            }
            else if (tag is ActTag)
            {
                var idp = ApplicationContext.Current.GetService<IDataPersistenceService<ActTag>>();
                var existing = idp.Query(o => o.SourceEntityKey == sourceKey && o.TagKey == tag.TagKey).FirstOrDefault();
                if (existing != null)
                {
                    existing.Value = tag.Value;
                    if (existing.Value == null)
                        idp.Obsolete(existing);
                    else
                        idp.Update(existing as ActTag);
                }
                else
                    idp.Insert(tag as ActTag);
            }
        }
    }


}
