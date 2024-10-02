
using Microsoft.EntityFrameworkCore;
using MyFanc.Contracts.DAL;
using MyFanc.Core.Utility;
using static MyFanc.Core.Enums;

namespace MyFanc.Contracts.Services
{
    public interface IDataProcessingService
    {
        IUnitOfWork UnitOfWork { get; set; }

        Task<int> Process(Dictionary<EntityAttributeKey, List<DataProcessingType>> calculationsByEntity);

        Task<int> ApplyLink(Dictionary<EntityAttributeKey, List<Type>> typesByEntity);

    }
}
