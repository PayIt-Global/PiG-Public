﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Devart Entity Developer tool using Data Transfer Object template.
// Code is generated on: 7/11/2024 6:53:55 PM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace PayItGlobalApi.DTOs.Generated
{

    public partial class StateProvinceDto
    {
        #region Constructors

        public StateProvinceDto() {
        }

        public StateProvinceDto(int stateprovinceId, string stateProvinceCode, string countryRegioncode, bool isOnlyStateProvinceFlag, string name, int territoryId, System.Guid rowGuid, System.DateTime modifiedDate) {

          this.StateprovinceId = stateprovinceId;
          this.StateProvinceCode = stateProvinceCode;
          this.CountryRegioncode = countryRegioncode;
          this.IsOnlyStateProvinceFlag = isOnlyStateProvinceFlag;
          this.Name = name;
          this.TerritoryId = territoryId;
          this.RowGuid = rowGuid;
          this.ModifiedDate = modifiedDate;
        }

        #endregion

        #region Properties

        public int StateprovinceId { get; set; }

        public string StateProvinceCode { get; set; }

        public string CountryRegioncode { get; set; }

        public bool IsOnlyStateProvinceFlag { get; set; }

        public string Name { get; set; }

        public int TerritoryId { get; set; }

        public System.Guid RowGuid { get; set; }

        public System.DateTime ModifiedDate { get; set; }

        #endregion
    }

}
