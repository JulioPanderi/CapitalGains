using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CapitalGains.Domain.JsonConverters
{
    public class JsonConverterDecimal : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader,
            Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value,
            JsonSerializerOptions options)
        {
            writer.WriteRawValue(value.ToString("0.0#",
                CultureInfo.InvariantCulture));
        }
    }
}
