using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class EnterCustomerInput
    {
        #region Internal Field Holder
        private Credentials credentialsField;

        private Customer customerField;

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

    }
}
