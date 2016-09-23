using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.EntityLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Extensions
{
    /// <summary>
    /// Model extensions
    /// </summary>
    public static class ModelExtensions
    {

        /// <summary>
        /// Load common properties for display
        /// </summary>
        public static TModel LoadDisplayProperties<TModel>(this TModel me) where TModel : BaseEntityData
        {
            if (me == null) return null;
            me.SetDelayLoad(true);

            // Load all properties on the entity
            foreach (var pi in typeof(TModel).GetRuntimeProperties())
                pi.GetValue(me);

            return me;
        }

        /// <summary>
        /// Load immediate relationships and participations for user interface
        /// </summary>
        public static TModel LoadImmediateRelations<TModel>(this TModel me) where TModel : BaseEntityData
        {
            if (me == null) return null;
            me.SetDelayLoad(true);
            
            // Act
            if (me is Act)
            {
                var act = me as Act;

				// Relationships
                for (int i = 0; i < act.Relationships.Count; i++)
                {
                    var te = act.Relationships[i].TargetAct.LoadDisplayProperties();
                }

				// Participations
                for (int i = 0; i < act.Participations.Count; i++)
                {
                    var tp = act.Participations[i].PlayerEntity.LoadDisplayProperties();
                }
            }

			// Entity
            else if (me is Entity)
            {
                var entity = me as Entity;
                Object l;

                // Relationships
                for (int i = 0; i < entity.Relationships.Count; i++)
                {
                    if(!entity.Relationships[i].InversionIndicator)
					{
						l = entity.Relationships[i].TargetEntity.LoadDisplayProperties();
					}
                    else
					{
						l = entity.Relationships[i].Holder.LoadDisplayProperties();
					}
                }

                // Participations
                for (int i = 0; i < entity.Participations.Count; i++)
                {
                    var tp= entity.Participations[i].Act.LoadDisplayProperties();
                }

            }

            return me;

        }
    }
}
