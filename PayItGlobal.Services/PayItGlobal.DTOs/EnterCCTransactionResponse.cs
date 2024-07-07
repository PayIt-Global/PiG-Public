using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PayItGlobal.DTOs
{
    public class EnterCCTransactionResponse
    {
        private string transactionIDField;
        private string responseCDField;
        private string responseDescField;

        private Customer customerField;

        public Customer Customer
        {
            get
            {
                return this.customerField;
            }
            set
            {
                this.customerField = value;
            }
        }

        public string TransactionID
        {
            get
            {
                return this.transactionIDField;
            }
            set
            {
                this.transactionIDField = value;
            }
        }

        public string ResponseCD
        {
            get
            {
                return this.responseCDField;
            }
            set
            {
                this.responseCDField = value;
            }
        }

        public string ResponseDesc
        {
            get
            {
                return this.responseDescField;
            }
            set
            {
                this.responseDescField = value;
            }
        }
    }
}
