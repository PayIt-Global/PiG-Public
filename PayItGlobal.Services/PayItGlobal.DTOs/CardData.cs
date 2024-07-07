using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class CardData
    {
        public int CardDataID { get; set; }
        public string Title { get; set; }        
        public int RecurringID { get; set; }
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CCNumber { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Addr1 { get; set; }
        public string City { get; set; }
        public string StateCD { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DynamicFieldVal { get; set; }
        public string DynamicFieldVal2 { get; set; }
        public string BusinessName { get; set; }
    }
}
