using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAppBV.Models;

namespace WebAppBV.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAll();
        Task<Transaction> Get(Guid? transactionId);
        Task Create(Transaction transaction);
        Task Create(IEnumerable<Transaction> transactions);
        Task Edit(Transaction transaction);
        Task Delete(Transaction transaction);
        List<Transaction> GetExistingByTransactions(List<Transaction> transactions);
    }
}