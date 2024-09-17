﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 9/16/2024 12:33:24 AM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

#nullable enable annotations
#nullable disable warnings

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace PayItGlobal.Domain.Entities
{
    public partial class AspNetUserLogin : IdentityUserLogin<int>
    {

        public AspNetUserLogin()
        {
            OnCreated();
        }

        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }

        public virtual string ProviderDisplayName { get; set; }

        public virtual int? UserId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        public override bool Equals(object obj)
        {
          AspNetUserLogin toCompare = obj as AspNetUserLogin;
          if (toCompare == null)
          {
            return false;
          }

          if (!Object.Equals(this.LoginProvider, toCompare.LoginProvider))
            return false;
          if (!Object.Equals(this.ProviderKey, toCompare.ProviderKey))
            return false;

          return true;
        }

        public override int GetHashCode()
        {
          int hashCode = 13;
          hashCode = (hashCode * 7) + LoginProvider.GetHashCode();
          hashCode = (hashCode * 7) + ProviderKey.GetHashCode();
          return hashCode;
        }

        #endregion
    }

}
