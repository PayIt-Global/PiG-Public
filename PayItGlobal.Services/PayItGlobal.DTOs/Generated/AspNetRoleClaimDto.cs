﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Devart Entity Developer tool using Data Transfer Object template.
// Code is generated on: 7/7/2024 2:04:14 PM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace PayItGlobal.DTOs.Generated
{

    public partial class AspNetRoleClaimDto
    {
        #region Constructors

        public AspNetRoleClaimDto() {
        }

        public AspNetRoleClaimDto(int id, int? roleId, string claimType, string claimValue, AspNetRoleDto aspNetRole) {

          this.Id = id;
          this.RoleId = roleId;
          this.ClaimType = claimType;
          this.ClaimValue = claimValue;
          this.AspNetRole = aspNetRole;
        }

        #endregion

        #region Properties

        public int Id { get; set; }

        public int? RoleId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        #endregion

        #region Navigation Properties

        public AspNetRoleDto AspNetRole { get; set; }

        #endregion
    }

}
