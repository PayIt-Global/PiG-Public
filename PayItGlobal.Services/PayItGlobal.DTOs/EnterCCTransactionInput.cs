using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class EnterCCTransactionInput
    {
        #region Internal Field Holder
        private Credentials credentialsField;
        private Customer customerField;
        private string firstNameField;
        private string lastNameField;
        private string companyNameField;
        private string street1Field;
        private string street2Field;
        private string stateCDField;
        private string cityField;
        private string zipField;
        private string phoneField;
        private string ccNumberField;
        private string expMonthField;
        private string expYearField;
        private decimal amountField;
        private string cvvField;
        private string emailField;
        private bool updateExistingCard;
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
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }
            set
            {
                this.firstNameField = value;
            }
        }
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }
        public string CompanyName
        {
            get
            {
                return this.companyNameField;
            }
            set
            {
                this.companyNameField = value;
            }
        }
        public string LastName
        {
            get
            {
                return this.lastNameField;
            }
            set
            {
                this.lastNameField = value;
            }
        }
        public string Street1
        {
            get
            {
                return this.street1Field;
            }
            set
            {
                this.street1Field = value;
            }
        }
        public string Street2
        {
            get
            {
                return this.street2Field;
            }
            set
            {
                this.street2Field = value;
            }
        }
        public string StateCD
        {
            get
            {
                return this.stateCDField;
            }
            set
            {
                this.stateCDField = value;
            }
        }
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }
        public string Zip
        {
            get
            {
                return this.zipField;
            }
            set
            {
                this.zipField = value;
            }
        }
        public string Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }
        public string CCNumber
        {
            get
            {
                return this.ccNumberField;
            }
            set
            {
                this.ccNumberField = value;
            }
        }
        public string ExpMonth
        {
            get
            {
                return this.expMonthField;
            }
            set
            {
                this.expMonthField = value;
            }
        }
        public string ExpYear
        {
            get
            {
                return this.expYearField;
            }
            set
            {
                this.expYearField = value;
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
        public string CVV
        {
            get
            {
                return this.cvvField;
            }
            set
            {
                this.cvvField = value;
            }
        }
        public bool UpdateExistingCard
        {
            get
            {
                return this.updateExistingCard;
            }
            set
            {
                this.updateExistingCard = value;
            }
        }
    }
}
