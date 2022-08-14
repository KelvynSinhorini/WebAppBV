using System.Collections.Generic;
using System.Threading.Tasks;
using WebAppBV.Models;

namespace WebAppBV.Services
{
    public interface ITransactionService
    {
        List<Transaction> CheckExistenceAndReturnList(List<Transaction> transactions);
        Task Create(Transaction transaction);
    }
}