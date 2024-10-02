using Microsoft.EntityFrameworkCore;

namespace MyFanc.Core.Utility
{
    public class EntityAttributeKey
    {
        public object Entity { get; set; }

        public object Id { get; set; }

        public EntityState State { get; set; }
    }
}
