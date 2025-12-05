using CapitalGains.Application.Interfaces;
using CapitalGains.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CapitalGains.Application.Services
{
    public class OperationsService : IOperationsService
    {
        private readonly IServiceScope scope;
        private readonly ILogger<OperationsService> logger;
        private bool disposedValue;

        public OperationsService(IServiceScopeFactory serviceScopeFactory, ILogger<OperationsService> logger) 
        {
            scope = serviceScopeFactory.CreateScope();
            this.logger = logger;
        }

        public List<ProcessedOperationDTO> ProcessMessage(List<OperationDTO> operations, decimal taxPercent, decimal minimumTaxed)
        {
            List<ProcessedOperationDTO> processedOperations = new List<ProcessedOperationDTO>();
            int currentStockQuantity = 0;
            decimal newWeightedAvgPrice = 0;
            decimal accumulatedLost = 0;

            //foreach (OperationDTO operationDTO in operations)
            for(int i=0; i<operations.Count; i++)
            {
                OperationDTO operationDTO=operations[i];
                try
                {
                    //Verify all data is ok
                    ValidatesOperation(operationDTO, currentStockQuantity, i);

                    //Calculate the tax
                    decimal tax = 0;
                    ProcessedOperationDTO op = new ProcessedOperationDTO(operationDTO);

                    //Initializes WAP
                    if (processedOperations.Count == 0)
                    {
                        newWeightedAvgPrice = op.UnitCost;
                    }
                    
                    if (op.Operation == "buy")
                    {
                        //new-weighted-average-price = ((current-stock-quantity * weighted-average-price) + (new-stock-quantity * new-price)) / (current-stock-quantity + new-stock-quantity)
                        op.WeightedAvgPrice = ((currentStockQuantity * newWeightedAvgPrice) + (op.Quantity * op.UnitCost)) / (currentStockQuantity + op.Quantity);
                        op.WeightedAvgPrice = Math.Round(op.WeightedAvgPrice, 2);
                        newWeightedAvgPrice = op.WeightedAvgPrice;
                        currentStockQuantity += op.Quantity;
                    }
                    if (op.Operation == "sell")
                    {
                        decimal cost = newWeightedAvgPrice * op.Quantity;
                        decimal totalSell = op.Quantity * op.UnitCost;
                        op.Profit = totalSell - cost;

                        //Calculates tax only if there is profit
                        if (op.Profit > 0)
                        {
                            //Total operation is bigger than the minimum
                            if (totalSell > minimumTaxed)
                            {
                                //Lost are bigger?
                                if (Math.Abs(accumulatedLost) > op.Profit)
                                {
                                    //Don't pay taxes, but discount profit from lost
                                    tax = 0;
                                    accumulatedLost += op.Profit;
                                }
                                else
                                {
                                    //Discount lost from profit, and pay tax over the difference
                                    tax = (op.Profit + accumulatedLost) * (taxPercent/100);
                                    accumulatedLost = 0;
                                }
                            }
                            else
                            {
                                //Don't pay taxes, but lost don't change
                                tax = 0;
                            }
                        }
                        else
                        {
                            //Keep adding lost
                            accumulatedLost += op.Profit;
                        }
                        //Updates stock
                        currentStockQuantity -= op.Quantity;
                    }
                    op.Tax = tax;
                    processedOperations.Add(op);
                }
                catch(Exception ex)
                {
                    logger.Log(LogLevel.Error, ex, $"Error at ProcessMessage - operation #{i}: operation {operationDTO.Operation}, Quantity {operationDTO.Quantity}, UnitCost {operationDTO.UnitCost}");
                    throw;
                }
            }
            return processedOperations;
        }

        private void ValidatesOperation(OperationDTO op, int currentStockQuantity, int line)
        {
            //Validates Operation
            if (!(op.Operation=="sell" || op.Operation == "buy"))
            {
                throw new ArgumentException($"The only operations allowed are 'sell' or 'buy': operation {op.Operation} (line {line}).");
            }
            //Validates Quantity
            if (op.Quantity < 0)
            {
                throw new ArgumentOutOfRangeException($"Quantity can't be negative: {op.Quantity} (line {line}).");
            }
            //Validates stock
            if (op.Operation == "sell" && (currentStockQuantity - op.Quantity) < 0)
            {
                throw new InvalidOperationException($"Can't sell more than current stock: quantity {op.Quantity}, stock {currentStockQuantity} (line {line}).");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) scope.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
