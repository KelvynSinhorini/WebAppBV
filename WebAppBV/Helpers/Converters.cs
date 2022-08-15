using WebAppBV.Models;
using WebAppBV.ViewModels;

namespace WebAppBV.Helpers
{
    public static class Converters
    {
        public static TransactionViewModel ConvertTransactionToViewModel(Transaction transaction)
        {
            var transactionViewModel = new TransactionViewModel(transaction.TransactionId, transaction.Description, transaction.Local, 
                transaction.Value, transaction.Date, transaction.NumberOfParcel, transaction.TotalParcel, transaction.OnlyThisMonth, transaction.Owner);

            return transactionViewModel;
        }

        public static Transaction ConvertTransactionViewModelToModel(TransactionViewModel transactionViewModel)
        {
            var transaction = new Transaction(transactionViewModel.TransactionId, transactionViewModel.Description, transactionViewModel.Local,
                transactionViewModel.Value, transactionViewModel.Date, transactionViewModel.NumberOfParcel, transactionViewModel.TotalParcel, transactionViewModel.OnlyThisMonth, transactionViewModel.Owner);

            return transaction;
        }
    }
}
