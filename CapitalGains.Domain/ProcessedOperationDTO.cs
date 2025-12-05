using System.Text.Json.Serialization;

namespace CapitalGains.Domain
{
    public class ProcessedOperationDTO 
    {
        public ProcessedOperationDTO(OperationDTO operationDTO) 
        {
            this.Operation = operationDTO.Operation;
            this.UnitCost = operationDTO.UnitCost;
            this.Quantity = operationDTO.Quantity;
        }

        [JsonIgnore]
        public string Operation { get;  }
        [JsonIgnore]
        public decimal UnitCost { get; }
        [JsonIgnore]
        public int Quantity { get; }

        [JsonIgnore]
        public decimal WeightedAvgPrice { get; set; }
        [JsonIgnore]
        public decimal Profit { get; set; }
        
        [JsonPropertyName("tax")]
        public decimal Tax { get; set; }
    }
}
