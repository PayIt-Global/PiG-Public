using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayItGlobal.DTOs
{
    public class Credentials
    {
        private string usernameField;

        private string passwordField;

        public string Username
        {
            get
            {
                return this.usernameField;
            }
            set
            {
                this.usernameField = value;
            }
        }

        public string Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }

    }
}
