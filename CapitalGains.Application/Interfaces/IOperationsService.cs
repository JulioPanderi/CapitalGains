using CapitalGains.Domain;

namespace CapitalGains.Application.Interfaces
{
    public interface IOperationsService : IDisposable
    {
        List<ProcessedOperationDTO> ProcessMessage(List<OperationDTO> operations, decimal taxPercent, decimal minimumTaxed);
    }
}
