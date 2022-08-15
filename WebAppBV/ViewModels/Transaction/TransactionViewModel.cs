using System;
using System.ComponentModel;
using WebAppBV.Models;

namespace WebAppBV.ViewModels
{
    public class TransactionViewModel
    {
        public TransactionViewModel(Guid transactionId, string description, string local, decimal value, DateTime date, int? numberOfParcel, int? totalParcel, bool onlyThisMonth, Owner? owner)
        {
            TransactionId = transactionId;
            Description = description;
            Local = local;
            Value = value;
            Date = date;
            NumberOfParcel = numberOfParcel;
            TotalParcel = totalParcel;
            OnlyThisMonth = onlyThisMonth;
            Owner = owner;
        }

        [DisplayName("Id")]
        public Guid TransactionId { get; private set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }
        [DisplayName("Local")]
        public string Local { get; set; }
        [DisplayName("Valor")]
        public decimal Value { get; set; }
        [DisplayName("Data da compra")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Numero da parcela
        /// </summary>
        [DisplayName("Número da parcela")]
        public int? NumberOfParcel { get; set; }

        /// <summary>
        /// Quantidade de parcelas (Total)
        /// </summary>
        [DisplayName("Total de parcelas")]
        public int? TotalParcel { get; set; }
        [DisplayName("Somente esse mês")]
        public bool OnlyThisMonth { get; set; } = false;
        [DisplayName("Dono")]
        public Owner? Owner { get; set; }

    }
}
