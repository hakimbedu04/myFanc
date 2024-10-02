
namespace MyFanc.Core.Utility
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class LinkAttribute : Attribute
    {
        private readonly Type target;
        private readonly string keyName;

        public LinkAttribute(Type target, string keyName)
        {
            this.target = target;
            this.keyName = keyName;
        }

        public Type Target { get => target; }

        public string KeyName { get => keyName; }
    }
}
