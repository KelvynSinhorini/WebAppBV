using System;
using System.Linq;
using WebAppBV.Models;
using WebAppBV.ViewModels;

namespace WebAppBV.Helpers
{
    public static class Converters
    {
        public static TransactionViewModel ConvertTransactionToViewModel(Transaction transaction)
        {
            var transactionViewModel = new TransactionViewModel(transaction.TransactionId, transaction.Description, transaction.Local, 
                transaction.Value, transaction.Date, transaction.NumberOfParcel, transaction.TotalParcel, transaction.OnlyThisMonth, transaction.Owner.ToString());

            return transactionViewModel;
        }

        public static TransactionIndexViewModel ConvertTransactionToIndexViewModel(Transaction transaction)
        {
            var transactionIndexViewModel = new TransactionIndexViewModel(transaction.TransactionId, transaction.Description, transaction.Local,
                transaction.Value, transaction.Date, transaction.NumberOfParcel, transaction.TotalParcel, transaction.OnlyThisMonth, transaction.Owner.ToString());

            return transactionIndexViewModel;
        }

        public static Transaction ConvertTransactionViewModelToModel(TransactionViewModel transactionViewModel)
        {
            var owner = transactionViewModel.Owners?.Select(o =>
            {
                Enum.TryParse(o, true, out Owner owner);
                return owner;
            }).Aggregate((prev, next) => prev | next);

            var transaction = new Transaction(transactionViewModel.TransactionId, transactionViewModel.Description, transactionViewModel.Local,
                transactionViewModel.Value, transactionViewModel.Date, transactionViewModel.NumberOfParcel, transactionViewModel.TotalParcel, transactionViewModel.OnlyThisMonth, owner);

            return transaction;
        }
    }
}
