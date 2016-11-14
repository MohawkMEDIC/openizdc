/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-10-11
 */
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
