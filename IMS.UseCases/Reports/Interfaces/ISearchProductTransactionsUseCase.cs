using IMS.CoreBusiness;
using IMS.Plugins.InMemory;

namespace IMS.UseCases.Reports.Interfaces
{
    public interface ISearchProductTransactionsUseCase
    {
        Task<IEnumerable<ProductTransaction>> ExecuteAsync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType);
    }
}