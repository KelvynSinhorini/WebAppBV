using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppBV.Models
{
    [Flags]
    public enum Owner
    {
        Kelvyn = 1,
        Mano = 2,
        Jocelaine = 4,
        Ketlyn = 8,
        Elias = 16,
        Joslei = 32,
        Compartilhado = 64,
        Desconto = 128
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

        public void SetNewTransactionId()
        {
            TransactionId = Guid.NewGuid();
        }
    }
}
