using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppBV.Models
{
    public class Transaction
    {
        [Required]
        [Column("TransactionId")]
        public Guid TransactionId { get; set; }

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
    }
}
