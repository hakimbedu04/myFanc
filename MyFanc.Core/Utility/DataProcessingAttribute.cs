using static MyFanc.Core.Enums;

namespace MyFanc.Core.Utility
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class DataProcessingAttribute : Attribute, IDataProcessingAttribute
    {
        private readonly DataProcessingType process;
        private readonly string keyName;

        public DataProcessingAttribute(DataProcessingType calculation, string keyName)
        {
            this.process = calculation;
            this.keyName = keyName;
        }

        public DataProcessingType Process { get => process; }

        public string KeyName { get => keyName; }
    }
}
