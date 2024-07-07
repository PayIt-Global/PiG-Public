using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class EnterRecurringInput
    {
        #region Internal Field Holder
        private Credentials credentialsField;

        private Recurring recurringField;

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

        public Recurring Recurring
        {
            get
            {
                return this.recurringField;
            }
            set
            {
                this.recurringField = value;
            }
        }

    }
}
