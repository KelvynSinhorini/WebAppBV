﻿using System.Collections.Generic;
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

        // TODO: Refatorar
        public async Task<List<Transaction>> CheckExistenceAndReturnList(List<Transaction> transactions)
        {
            var newTransactions = new List<Transaction>();

            if (transactions.Any())
            {
                foreach (var transaction in transactions)
                {
                    if (!transaction.OnlyThisMonth)
                    {
                        var transactionInDatabase = _context.Transactions.FirstOrDefault(t =>
                            t.Description.Equals(transaction.Description) &&
                            t.Value.Equals(transaction.Value) &&
                            t.Local.Equals(transaction.Local) &&
                            t.Date.Date.Equals(transaction.Date.Date));

                        if (transactionInDatabase == null)
                        {
                            _context.Transactions.Add(transaction);
                            newTransactions.Add(transaction);
                        }
                        else
                        {
                            newTransactions.Add(transactionInDatabase);
                        }
                    }
                    else
                    {
                        newTransactions.Add(transaction);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return newTransactions;
        }

        public List<Transaction> GetExistingByTransactions(List<Transaction> transactions) // TODO Tornar assincrono
        {
            if (transactions == null)
                throw new System.ArgumentNullException();

            var existingTransactions = new List<Transaction>();

            foreach(var transaction in transactions.Where(t => !t.OnlyThisMonth))
            {
                var transactionInDatabase = _context.Transactions.FirstOrDefault(t =>
                            t.Description.Equals(transaction.Description) &&
                            t.Value.Equals(transaction.Value) &&
                            t.Local.Equals(transaction.Local) &&
                            t.Date.Date.Equals(transaction.Date.Date));

                if(transactionInDatabase != null)
                    existingTransactions.Add(transactionInDatabase);
            }

            return existingTransactions;
        }
    }
}