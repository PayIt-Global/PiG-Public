﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Devart Entity Developer tool using Data Transfer Object template.
// Code is generated on: 7/1/2024 2:59:24 AM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace PayItGlobal.DTOs.Generated
{

    public partial class AspNetUserTokenDto
    {
        #region Constructors

        public AspNetUserTokenDto() {
        }

        public AspNetUserTokenDto(string userId, string loginProvider, string name, string value, AspNetUserDto aspNetUser) {

          this.UserId = userId;
          this.LoginProvider = loginProvider;
          this.Name = name;
          this.Value = value;
          this.AspNetUser = aspNetUser;
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        #endregion

        #region Navigation Properties

        public AspNetUserDto AspNetUser { get; set; }

        #endregion
    }

}
