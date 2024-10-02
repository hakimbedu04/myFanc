using Microsoft.EntityFrameworkCore;
using MyFanc.Core.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Contracts.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();

        bool HasTransaction { get; }

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;
    }
}
