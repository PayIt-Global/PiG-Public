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

namespace PayItGlobal.DTOs.Generated
{

    public partial class AspNetUserClaimDto
    {
        #region Constructors

        public AspNetUserClaimDto() {
        }

        public AspNetUserClaimDto(int id, int? userId, string claimType, string claimValue, AspNetUserDto aspNetUser) {

          this.Id = id;
          this.UserId = userId;
          this.ClaimType = claimType;
          this.ClaimValue = claimValue;
          this.AspNetUser = aspNetUser;
        }

        #endregion

        #region Properties

        public int Id { get; set; }

        public int? UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        #endregion

        #region Navigation Properties

        public AspNetUserDto AspNetUser { get; set; }

        #endregion
    }

}
