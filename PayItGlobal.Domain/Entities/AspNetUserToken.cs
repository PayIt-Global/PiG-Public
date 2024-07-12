﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 7/11/2024 6:53:54 PM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
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
    public partial class AspNetUserToken : IdentityUserToken<int> {

        public AspNetUserToken()
        {
            OnCreated();
        }

        public virtual int UserId { get; set; }

        public virtual string LoginProvider { get; set; }

        public virtual string Name { get; set; }

        public virtual string Value { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        public override bool Equals(object obj)
        {
          AspNetUserToken toCompare = obj as AspNetUserToken;
          if (toCompare == null)
          {
            return false;
          }

          if (!Object.Equals(this.UserId, toCompare.UserId))
            return false;
          if (!Object.Equals(this.LoginProvider, toCompare.LoginProvider))
            return false;
          if (!Object.Equals(this.Name, toCompare.Name))
            return false;

          return true;
        }

        public override int GetHashCode()
        {
          int hashCode = 13;
          hashCode = (hashCode * 7) + UserId.GetHashCode();
          hashCode = (hashCode * 7) + LoginProvider.GetHashCode();
          hashCode = (hashCode * 7) + Name.GetHashCode();
          return hashCode;
        }

        #endregion
    }

}
