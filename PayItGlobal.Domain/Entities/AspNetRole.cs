﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 7/1/2024 2:59:24 AM
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
    public partial class AspNetRole : IdentityRole {

        public AspNetRole()
        {
            this.AspNetRoleClaims = new List<AspNetRoleClaim>();
            this.AspNetUsers = new List<AspNetUser>();
            OnCreated();
        }

        public string Id { get; set; }

        public string? Name { get; set; }

        public string? NormalizedName { get; set; }

        public string? ConcurrencyStamp { get; set; }

        public virtual IList<AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public virtual IList<AspNetUser> AspNetUsers { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
