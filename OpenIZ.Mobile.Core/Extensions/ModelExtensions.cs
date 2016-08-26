using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
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
        public static TModel LoadImmediateRelations<TModel>(this TModel me) where TModel : BaseEntityData
        {
            if (me == null) return null;
            var delayLoad = me.IsDelayLoadEnabled;
            me.SetDelayLoad(true);
            if (me is Act)
            {
                var act = me as Act;
                var loadEntity = act.Relationships.Where(o => o.TargetAct == null).ToList();
                if (act.Participations.Count < 100)
                    act.Participations.Where(o => o.PlayerEntity == null).ToList();
            }
            else if (me is Entity)
            {
                var entity = me as Entity;
                var loadEntity = entity.Relationships.Where(o => o.TargetEntity == null).ToList();
                loadEntity = entity.Relationships.Where(o => o.InversionIndicator && o.Holder == null).ToList();
                if (entity.Participations.Count < 100)
                    entity.Participations.Where(o => o.Act == null).ToList();
            }

            me.SetDelayLoad(delayLoad);
            return me;

        }
    }
}
