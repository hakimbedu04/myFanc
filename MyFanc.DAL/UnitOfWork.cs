using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MyFanc.Contracts.DAL;
using MyFanc.Contracts.Services;
using MyFanc.Core.Utility;
using System.Diagnostics;
using System.Reflection;
using System.Transactions;
using static MyFanc.Core.Enums;

namespace MyFanc.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly MyFancDbContext _context;
        private readonly IDataProcessingService _dataProcessingService;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly IAuditingService _auditingService;
        private IDbContextTransaction? _transaction;
        private bool disposedValue;

        public UnitOfWork(MyFancDbContext context, ILogger<UnitOfWork> logger, IAuditingService auditingService, IDataProcessingService dataProcessingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dataProcessingService = dataProcessingService;
            _dataProcessingService.UnitOfWork = this;
            _logger = logger;
            _auditingService = auditingService;
        }

        public bool HasTransaction => _transaction != null;

        public void BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = _context.Database.BeginTransaction();
            }
            else
            {
                _logger.LogWarning("A transaction already exists issue at {stacktrace}", StackTraceHelper.GetText(new StackTrace()));
            }
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
            else
            {
                throw new InvalidOperationException("A commit has been called without transaction");
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
            else
            {
                throw new InvalidOperationException("A rollback has been called without transaction");
            }
        }

        //load a context automatically
        // the context is disposed when this UOW obejct is disposed > at end of webrequest (simple injector weblifestyle request)

        public IGenericRepository<TEntity> GetGenericRepository<TEntity>()
            where TEntity : class
        {
            return new GenericRepository<TEntity>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            var links = GetLinks();

            var updatedEntries = _context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached).ToArray();
            foreach (var entry in updatedEntries)
            {
                _auditingService.Audit(entry);
            }

            var dataToProcess = GetDataToProcess();

            var res = await _context.SaveChangesAsync();

            if (links.Any())
                res = await _dataProcessingService.ApplyLink(links);

            if (dataToProcess.Any())
                res = await _dataProcessingService.Process(dataToProcess);

            return res;
        }

        private Dictionary<EntityAttributeKey, List<DataProcessingType>> GetDataToProcess()
        {
            var updatedEntries = _context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached).ToArray();
            var processById = new Dictionary<EntityAttributeKey, List<DataProcessingType>>();
            foreach (var entry in updatedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        break;
                    case EntityState.Modified:
                        var updatedMembers = entry.Members.Where(m => m.IsModified).ToList();
                        foreach (var member in updatedMembers)
                        {
                            var attributeSource = member.Metadata.GetMemberInfo(true, true).DeclaringType?
                                .GetMembers().FirstOrDefault(m => m.Name == member.Metadata.Name);
                            if(attributeSource != null)
                                AddProcessOfDataProcessingAttribute<DataProcessingAttribute>(processById, entry, attributeSource);
                        }
                        break;
                    case EntityState.Added:
                        var members = entry.Entity.GetType().GetMembers();
                        foreach (var member in members)
                        {
                            AddProcessOfDataProcessingAttribute<DataProcessingAttribute>(processById, entry, member);
                        }
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    default:
                        break;
                }

            }
            return processById;
        }

        private void AddProcessOfDataProcessingAttribute<TAttributeType>(Dictionary<EntityAttributeKey, List<DataProcessingType>> processById, EntityEntry entry, MemberInfo attributeSource) where TAttributeType : Attribute, IDataProcessingAttribute
        {
            var attributes = attributeSource.GetCustomAttributes<TAttributeType>();
            foreach (var attribute in attributes)
            {
                var key = entry.Entity.GetType().GetProperty(attribute.KeyName)?.GetValue(entry.Entity, null);
                if (key == null)
                    continue;
                var existing = processById.FirstOrDefault(c => c.Key.Entity.GetType().Equals(entry.Entity.GetType()) && c.Key.Id.Equals(key));
                if (existing.Key == null)
                {
                    processById.Add(new EntityAttributeKey { Entity = entry.Entity, Id = key }, new List<DataProcessingType>());
                    existing = processById.FirstOrDefault(c => c.Key.Entity.GetType().Equals(entry.Entity.GetType()) && c.Key.Id.Equals(key));
                }
                if (!existing.Value.Contains(attribute.Process))
                {
                    existing.Value.Add(attribute.Process);
                    _logger.LogDebug("add {ProcessType} process due to an update on a {TargetEntity} on property {UpdatedProperty} with key {LinkProperty} {Key}"
                        , attribute.Process
                        , entry.Entity.GetType().Name
                        , attributeSource.Name
                        , attribute.KeyName
                        , key);
                }
            }
        }

        private Dictionary<EntityAttributeKey, List<Type>> GetLinks()
        {
            var updatedEntries = _context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached).ToArray();
            var typesById = new Dictionary<EntityAttributeKey, List<Type>>();
            foreach (var entry in updatedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        AddTypeOfLinkAttribute(typesById, entry, entry.State);
                        break;
                    case EntityState.Modified:
                        var updatedMembers = entry.Members.Where(m => m.IsModified).ToList();
                        foreach (var member in updatedMembers)
                        {
                            var types = new List<Type>();
                            var attributeSource = member.Metadata.GetMemberInfo(true, true).DeclaringType?
                                .GetMembers().FirstOrDefault(m => m.Name == member.Metadata.Name);
                            if(attributeSource != null)
                                AddTypeOfLinkAttribute(typesById, entry, attributeSource, entry.State);
                        }
                        break;
                    case EntityState.Added:
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    default:
                        break;
                }

            }
            return typesById;
        }

        private void AddTypeOfLinkAttribute(Dictionary<EntityAttributeKey, List<Type>> typesById, EntityEntry entry, MemberInfo attributeSource, EntityState state)
        {
            var attributes = attributeSource.GetCustomAttributes<LinkAttribute>();
            foreach (var attribute in attributes)
            {
                var key = entry.Entity.GetType().GetProperty(attribute.KeyName)?.GetValue(entry.Entity, null);
                if (key == null)
                    continue;

                var existing = typesById.FirstOrDefault(c => c.Key.Entity.GetType().Equals(entry.Entity.GetType()) && c.Key.Id.Equals(key));
                if (existing.Key == null)
                {
                    typesById.Add(new EntityAttributeKey { Entity = entry.Entity, Id = key, State = state }, new List<Type>());
                    existing = typesById.FirstOrDefault(c => c.Key.Entity.GetType().Equals(entry.Entity.GetType()) && c.Key.Id.Equals(key));
                }
                if (!existing.Value.Contains(attribute.Target))
                {
                    existing.Value.Add(attribute.Target);
                    _logger.LogDebug($"add calculation on type {attribute.Target.Name} for {attribute.KeyName} {key} due to an update on a {entry.Entity.GetType().Name}");
                }
            }
        }
        private void AddTypeOfLinkAttribute(Dictionary<EntityAttributeKey, List<Type>> typesById, EntityEntry entry, EntityState state)
        {
            var attributes = entry.Entity.GetType().GetCustomAttributes<LinkAttribute>();

            foreach (var attribute in attributes)
            {
                var key = entry.Entity.GetType().GetProperty(attribute.KeyName)?.GetValue(entry.Entity, null);
                if (key == null)
                    continue;

                var existing = typesById.FirstOrDefault(c => c.Key.Entity.GetType().Equals(entry.Entity.GetType()) && c.Key.Id.Equals(key));
                if (existing.Key == null)
                {
                    typesById.Add(new EntityAttributeKey { Entity = entry.Entity, Id = key, State = state }, new List<Type>());
                    existing = typesById.FirstOrDefault(c => c.Key.Entity.GetType().Equals(entry.Entity.GetType()) && c.Key.Id.Equals(key));
                }
                if (!existing.Value.Contains(attribute.Target))
                {
                    existing.Value.Add(attribute.Target);
                    _logger.LogDebug($"add calculation on type {attribute.Target.Name} for {attribute.KeyName} {key} due to a delete on a {entry.Entity.GetType().Name}");
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                if (_transaction != null)
                {
                    _logger.LogError("A transaction has not been commited issue on {stacktrace}", StackTraceHelper.GetText(new StackTrace()));
                    _transaction.Dispose();
                }
                _context.Dispose();
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnitOfWork()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
