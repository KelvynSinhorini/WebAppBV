using System.Collections.Generic;
using System.Threading.Tasks;
using WebAppBV.Models;

namespace WebAppBV.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> CheckExistenceAndReturnList(List<Transaction> transactions);
        Task Create(Transaction transaction);
        Task Create(IEnumerable<Transaction> transactions);
        List<Transaction> GetExistingByTransactions(List<Transaction> transactions);
    }
}