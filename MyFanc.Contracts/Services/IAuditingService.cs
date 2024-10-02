using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Contracts.Services
{
    public interface IAuditingService
    {
        void Audit(EntityEntry entry);
    }
}
