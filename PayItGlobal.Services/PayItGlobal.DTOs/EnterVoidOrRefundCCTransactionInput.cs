using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class EnterVoidOrRefundCCTransactionInput 
    {
        #region Internal Field Holder
        private Credentials credentialsField;

        private string typeField;
        private string transactionIdField;
        private decimal amountField;

        #endregion

        public Credentials Credentials
        {
            get
            {
                return this.credentialsField;
            }
            set
            {
                this.credentialsField = value;
            }
        }

        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        public string TransactionID
        {
            get
            {
                return this.transactionIdField;
            }
            set
            {
                this.transactionIdField = value;
            }
        }

        public decimal Amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }
}
