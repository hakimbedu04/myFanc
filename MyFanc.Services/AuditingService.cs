using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.BLL;
using MyFanc.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Services
{
    public class AuditingService : IAuditingService
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly ILogger<AuditingService> _logger;

        public AuditingService(IAuthenticationHelper authenticationHelper, ILogger<AuditingService> logger)
        {
            _authenticationHelper = authenticationHelper;
            _logger = logger;
        }

        public void Audit(EntityEntry entry)
        {
            if (entry.Entity is AuditedEntity auditedEntity)
            {
                var userId = _authenticationHelper.GetConnectedUserId();
                switch (entry.State)
                {
                    case EntityState.Modified:
                        {
                            auditedEntity.LatestUpdateTime = DateTime.Now;
                            auditedEntity.LatestUpdateUserId = userId;
                            _logger.LogTrace("An update has been done", new { entry.CurrentValues, entry.OriginalValues });
                            break;
                        }
                    case EntityState.Deleted:
                        {
                            auditedEntity.DeletedTime = DateTime.Now;
                            auditedEntity.DeleterUserId = userId;
                            entry.State = EntityState.Modified;
                            _logger.LogTrace("A soft delete has been done", new { entry.CurrentValues });
                            break;
                        }
                    case EntityState.Added:
                        {
                            auditedEntity.CreationTime = DateTime.Now;
                            auditedEntity.CreatorUserId = userId;
                            _logger.LogTrace("A new entry has been created", new { entry.CurrentValues });
                            break;
                        }
                }
            }
            else
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        {
                            _logger.LogTrace("An update has been done", new { entry.CurrentValues, entry.OriginalValues });
                            break;
                        }
                    case EntityState.Deleted:
                        {
                            _logger.LogTrace("A delete has been done", new { entry.CurrentValues });
                            break;
                        }
                    case EntityState.Added:
                        {
                            _logger.LogTrace("A new entry has been created", new { entry.CurrentValues });
                            break;
                        }
                }
            }
        }

    }
}
