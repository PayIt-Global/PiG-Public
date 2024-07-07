using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class ListTransactionsResponse 
    {
        #region Internal Field Holder
        private Credentials credentialsField;

        private string transactionIDField;
        private decimal amountField;
        private DateTime transDateTimeField;
        private string payerNameField;
        private int customerIDField;
        private string cardTypeField;

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

        public DateTime TransDateTime
        {
            get
            {
                return this.transDateTimeField;
            }
            set
            {
                this.transDateTimeField = value;
            }
        }

        public string PayerName
        {
            get
            {
                return this.payerNameField;
            }
            set
            {
                this.payerNameField = value;
            }
        }

        public int CustomerID
        {
            get
            {
                return this.customerIDField;
            }
            set
            {
                this.customerIDField = value;
            }
        }

        public string CardType
        {
            get
            {
                return this.cardTypeField;
            }
            set
            {
                this.cardTypeField = value;
            }
        }
    }
}
