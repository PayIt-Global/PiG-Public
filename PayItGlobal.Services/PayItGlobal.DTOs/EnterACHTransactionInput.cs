using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class EnterACHTransactionInput
    {
        #region Internal Field Holder
        private Credentials credentialsField;

        private Customer customerField;

        private string nameField;

        private string street1Field;

        private string street2Field;

        private string stateCDField;

        private string cityField;

        private string zipField;

        private string phoneField;

        private string routingNumberField;

        private string accountNumberField;

        private string driverLicenseStateCDField;

        private string driverLicenseNumberField;

        private string customerCDField;

        private string acctTypeCDField;

        private System.Nullable<decimal> checkAmountField;

        private string merchantTransactionIDField;

        private string creditDebitField;

        private string validationTypeField;
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

        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
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

        public string RoutingNumber
        {
            get
            {
                return this.routingNumberField;
            }
            set
            {
                this.routingNumberField = value;
            }
        }

        public string AccountNumber
        {
            get
            {
                return this.accountNumberField;
            }
            set
            {
                this.accountNumberField = value;
            }
        }

        public string DriverLicenseStateCD
        {
            get
            {
                return this.driverLicenseStateCDField;
            }
            set
            {
                this.driverLicenseStateCDField = value;
            }
        }

        public string DriverLicenseNumber
        {
            get
            {
                return this.driverLicenseNumberField;
            }
            set
            {
                this.driverLicenseNumberField = value;
            }
        }

        public string CustomerCD
        {
            get
            {
                return this.customerCDField;
            }
            set
            {
                this.customerCDField = value;
            }
        }

        public string AcctTypeCD
        {
            get
            {
                return this.acctTypeCDField;
            }
            set
            {
                this.acctTypeCDField = value;
            }
        }

        public System.Nullable<decimal> CheckAmount
        {
            get
            {
                return this.checkAmountField;
            }
            set
            {
                this.checkAmountField = value;
            }
        }

        public string MerchantTransactionID
        {
            get
            {
                return this.merchantTransactionIDField;
            }
            set
            {
                this.merchantTransactionIDField = value;
            }
        }

        public string CreditDebit
        {
            get
            {
                return this.creditDebitField;
            }
            set
            {
                this.creditDebitField = value;
            }
        }

        public string ValidationType
        {
            get
            {
                return this.validationTypeField;
            }
            set
            {
                this.validationTypeField = value;
            }
        }

    }
}
