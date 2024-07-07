using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class ACHData
    {
        public int ACHDataID { get; set; }
        public string Title { get; set; }
        public int CustomerID { get; set; }
        public int RecurringID { get; set; }

        public string NameOnAccount { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AcctType { get; set; }        
        public string CustomerCD { get; set; }

        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DynamicFieldVal { get; set; }
        public string DynamicFieldVal2 { get; set; }
        public string BusinessName { get; set; }
    }
}
