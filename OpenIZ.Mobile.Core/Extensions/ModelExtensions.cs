using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.EntityLoader;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Load immediate relationships and participations for user interface
        /// </summary>
        public static TEntity LoadImmediateRelations<TEntity>(this TEntity me) where TEntity : Entity
        {

            var delayLoad = me.IsDelayLoadEnabled;
            me.SetDelayLoad(true);
            var loadEntity = me.Relationships.Where(o => o.TargetEntity == null).ToList();
            if (me.Participations.Count < 100) 
                me.Participations.Where(o => o.Act == null).ToList();
            me.SetDelayLoad(delayLoad);
            return me;
        }
    }
}
