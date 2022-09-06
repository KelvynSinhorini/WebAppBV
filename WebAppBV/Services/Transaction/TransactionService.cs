using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppBV.Data;
using WebAppBV.Helpers;
using WebAppBV.Models;

namespace WebAppBV.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BVContext _context;

        public TransactionService(BVContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAll() // TODO Arrumar a perfomance, GetAll é recomendado nao fazer.
        {
            var transactions = await _context.Transactions.ToListAsync();
            return transactions;
        }

        public async Task<Transaction> Get(Guid? transactionId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(m => m.TransactionId == transactionId);
            return transaction;
        }

        public async Task Create(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Create(IEnumerable<Transaction> transactions)
        {
            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(Transaction transaction)
        {
            _context.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        public List<Transaction> GetTransactionsToAdd(List<Transaction> transactions) // TODO Tornar assincrono
        {
            if (transactions == null)
                throw new ArgumentNullException();

            var transactionsToAdd = new List<Transaction>();

            foreach(var transaction in transactions)
            {
                var transactionInDatabase = _context.Transactions.FirstOrDefault(t =>
                            t.Description.Equals(transaction.Description) &&
                            t.Value.Equals(transaction.Value) &&
                            t.Local.Equals(transaction.Local) &&
                            t.Date.Date.Equals(transaction.Date.Date));

                if(transactionInDatabase == null)
                    transactionsToAdd.Add(transaction);
            }

            return transactionsToAdd;
        }

        public IEnumerable<Transaction> GetAndFilterTransactions(List<Transaction> transactions)
        {
            if (transactions == null)
                throw new ArgumentNullException();

            foreach (var transaction in transactions)
            {
                var transactionInDatabase = _context.Transactions.FirstOrDefault(t =>
                            t.Description.Equals(transaction.Description) &&
                            t.Value.Equals(transaction.Value) &&
                            t.Local.Equals(transaction.Local) &&
                            t.Date.Date.Equals(transaction.Date.Date));

                if (transactionInDatabase != null)
                {
                    transaction.Owner = transactionInDatabase.Owner;
                }

                yield return transaction;
            }
        }
    }
}
