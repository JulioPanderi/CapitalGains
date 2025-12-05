using CapitalGains.Domain;
using CapitalGains.Domain.JsonConverters;
using System.Text.Json;

namespace CapitalGains.Common
{
    public class Converters
    {
        public static List<OperationDTO> ConvertJsonArrayToDTO(string jsonArray) => JsonSerializer.Deserialize<List<OperationDTO>>(jsonArray);

        public static string ConvertDTOtoJsonArray(List<ProcessedOperationDTO> operations)
            => JsonSerializer.Serialize(operations, new JsonSerializerOptions() { Converters = { new JsonConverterDecimal() } } );
    }
}