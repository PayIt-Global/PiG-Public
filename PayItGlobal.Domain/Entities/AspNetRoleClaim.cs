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
    public partial class AspNetRoleClaim : IdentityRoleClaim<string> {

        public AspNetRoleClaim()
        {
            OnCreated();
        }

        public int Id { get; set; }

        public string? RoleId { get; set; }

        public string? ClaimType { get; set; }

        public string? ClaimValue { get; set; }

        public virtual AspNetRole AspNetRole { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
