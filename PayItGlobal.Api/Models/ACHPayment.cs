using System.ComponentModel.DataAnnotations;

namespace PayItGlobalApi.Models
{
    public partial class ACHPayment
    {
        public string ConfirmationNumber { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseText { get; set; }

        [StringLength(120)]
        public string NameOnAccount { get; set; }

        [StringLength(185)]
        public string BusinessName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Street { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(2)]
        public string StateAbbreviation { get; set; }

        [StringLength(15)]
        public string Zipcode { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(9)]
        public string Routing { get; set; }

        [Required]
        [StringLength(45)]
        public string AccountNumber { get; set; }


    }
}