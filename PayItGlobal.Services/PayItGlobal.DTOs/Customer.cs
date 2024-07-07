using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PayItGlobal.DTOs
{
    public class Customer
    {        
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClientCustomerID { get; set; }
        public bool Active { get; set; }     
        public string DynamicFieldVal { get; set; }
        public string DynamicFieldVal2 { get; set; }
        public string Notes { get; set; }

        private List<CardData> cardData = new List<CardData>();
        public List<CardData> CreditCards 
        {
            get { return cardData; }
            set { cardData = value; }
        }

        private List<ACHData> achData = new List<ACHData>();
        public List<ACHData> CheckingAccounts
        {
            get { return achData; }
            set { achData = value; }
        }
    }

}
