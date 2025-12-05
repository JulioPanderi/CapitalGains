using System.Text.Json.Serialization;

namespace CapitalGains.Domain
{
    public class OperationDTO
    {
        string operation;

        [JsonPropertyName("operation")]
        public string Operation {
            get => operation;
            set => operation = value.Trim().ToLower();
        }

        [JsonPropertyName("unit-cost")]
        public decimal UnitCost {  get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
