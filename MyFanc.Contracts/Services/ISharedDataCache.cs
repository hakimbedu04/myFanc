using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Contracts.Services
{
    public interface ISharedDataCache
    {
        void SetData<T>(string key, T data);
        T? GetData<T>(string key);
        void RemoveData<T>(string key);
        void ClearAllData();
    }
}
