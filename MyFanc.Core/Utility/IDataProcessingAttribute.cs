using static MyFanc.Core.Enums;

namespace MyFanc.Core.Utility
{
    public interface IDataProcessingAttribute
    {
        DataProcessingType Process { get; }

        string KeyName { get; }
    }
}
