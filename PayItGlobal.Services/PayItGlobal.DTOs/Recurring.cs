using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class Recurring
    {
        public int RecurringID { get; set; }
        public int CustomerID { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Amount { get; set; }
        public string RecurringName { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public bool Active { get; set; }
        public bool IsDelayPay { get; set; }
        public DateTime DeleteDate { get; set; }
        public bool IsWeekly { get; set; }
        public bool Is4Weekly { get; set; }
        public bool Is2Weekly { get; set; }

        public CardData CardData { get; set; }
        public ACHData ACHData { get; set; }
    }
}
