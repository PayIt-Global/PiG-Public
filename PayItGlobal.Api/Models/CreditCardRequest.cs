using System.ComponentModel.DataAnnotations;

namespace PayEzPaymentApi.Models
{
    public partial class CreditCardRequest 
    {                
        [StringLength(20)]
        public string FirstName { get; set; }

        [StringLength(25)]
        public string LastName { get; set; }

        [StringLength(185)]
        public string BusinessName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Street { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(2)]
        public string StateAbbreviation { get; set; }

        [StringLength(15)]
        public string Zipcode { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(18)]
        public string CCNumber { get; set; }

        [Required]
        [StringLength(2)]
        public string ExpirationMonth { get; set; }

        [Required]
        [StringLength(2)]
        public string ExpirationYear { get; set; }

        [StringLength(6)]
        public string CVV { get; set; }
        
        public bool IsVoid { get; set; }
        public bool IsRefund { get; set; }
        public string VoidRefundConfirmationNumber { get; set; }
        public decimal VoidRefundAmount { get; set; }

    }
}