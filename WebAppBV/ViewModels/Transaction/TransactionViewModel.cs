using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WebAppBV.ViewModels
{
    public class TransactionViewModel
    {
        public TransactionViewModel()
        {
        }

        public TransactionViewModel(Guid transactionId, string description, string local, decimal value, DateTime date, int? numberOfParcel, int? totalParcel, bool onlyThisMonth, string owner)
        {
            TransactionId = transactionId;
            Description = description;
            Local = local;
            Value = value;
            Date = date;
            NumberOfParcel = numberOfParcel;
            TotalParcel = totalParcel;
            OnlyThisMonth = onlyThisMonth;
            Owners = owner.Split(',').ToList();
        }

        [DisplayName("Id")]
        public Guid TransactionId { get; set; }

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
        [DisplayName("Dono(s)")]
        public List<string> Owners { get; set; }

    }

    public class TransactionIndexViewModel
    {
        public TransactionIndexViewModel()
        {

        }

        public TransactionIndexViewModel(Guid transactionId, string description, string local, decimal value, DateTime date, int? numberOfParcel, int? totalParcel, bool onlyThisMonth, string owner)
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
        public Guid TransactionId { get; set; }

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
        [DisplayName("Dono(s)")]
        public string Owner { get; set; }
    }
}
