using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class ListRecurringResponse
    {
        #region Internal Field Holder

        private int recurringIDField;
        private decimal amountField;
        private DateTime startDateField;
        private int cardDataIDField;
        private int achDataIDField;
        private int customerIDField;
        private string cardTypeField;

        #endregion

        public int RecurringID
        {
            get
            {
                return this.recurringIDField;
            }
            set
            {
                this.recurringIDField = value;
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

        public DateTime StartDateField
        {
            get
            {
                return this.startDateField;
            }
            set
            {
                this.startDateField = value;
            }
        }

        public int CardDataID
        {
            get
            {
                return this.cardDataIDField;
            }
            set
            {
                this.cardDataIDField = value;
            }
        }

        public int ACHDataID
        {
            get
            {
                return this.achDataIDField;
            }
            set
            {
                this.achDataIDField = value;
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
