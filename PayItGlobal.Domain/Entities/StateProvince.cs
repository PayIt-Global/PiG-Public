﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 9/16/2024 1:40:37 AM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

#nullable enable annotations
#nullable disable warnings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace PayItGlobalApi.Domain.Entities
{
    public partial class StateProvince {

        public StateProvince()
        {
            OnCreated();
        }

        public virtual int StateprovinceId { get; set; }

        public virtual string StateProvinceCode { get; set; }

        public virtual string CountryRegioncode { get; set; }

        public virtual bool IsOnlyStateProvinceFlag { get; set; }

        public virtual string Name { get; set; }

        public virtual int TerritoryId { get; set; }

        public virtual Guid RowGuid { get; set; }

        public virtual DateTime ModifiedDate { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
