using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyFanc.Core.Utility
{
    public class EnumNameConverter<TEnum> : JsonConverter<TEnum> where TEnum : Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Implement this method if deserialization is needed
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
