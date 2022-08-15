using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppBV.Models
{
    public enum Owner
    {
        Kelvyn,
        Mano,
        Jocelaine,
        Ketlyn,
        Elias,
        Joslei,
        Compartilhado,
        Desconto
    }

    public class Transaction
    {
        public Transaction() { }

        public Transaction(Guid transactionId)
        {
            TransactionId = transactionId;
        }

        public Transaction(Guid transactionId, string description, string local, decimal value, DateTime date, int? numberOfParcel, int? totalParcel, bool onlyThisMonth, Owner? owner)
        {
            TransactionId = transactionId;
            Description = description;
            Local = local;
            Value = value;
            Date = date;
            NumberOfParcel = numberOfParcel;
            TotalParcel = totalParcel;
            Owner = owner;
            OnlyThisMonth = onlyThisMonth;
        }

        [Required]
        [Column("TransactionId")]
        public Guid TransactionId { get; private set; }

        [Column("Description")]
        public string Description { get; set; }
        [Column("Local")]
        public string Local { get; set; }
        [Column("Value")]
        public decimal Value { get; set; }
        [Column("Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Numero da parcela
        /// </summary>
        [Column("NumberOfParcel")]
        public int? NumberOfParcel { get; set; }

        /// <summary>
        /// Quantidade de parcelas (Total)
        /// </summary>
        [Column("TotalParcel")]
        public int? TotalParcel { get; set; }
        [Column("OnlyThisMonth")]
        public bool OnlyThisMonth { get; set; } = false;
        [Column("Owner")]
        public Owner? Owner { get; set; }
    }
}
