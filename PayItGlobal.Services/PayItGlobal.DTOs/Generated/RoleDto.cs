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

    public partial class RoleDto
    {
        #region Constructors

        public RoleDto() {
        }

        public RoleDto(System.Guid roleID, string roleName, string loweredRoleName, string description, List<UserDto> users) {

          this.RoleID = roleID;
          this.RoleName = roleName;
          this.LoweredRoleName = loweredRoleName;
          this.Description = description;
          this.Users = users;
        }

        #endregion

        #region Properties

        public System.Guid RoleID { get; set; }

        public string RoleName { get; set; }

        public string LoweredRoleName { get; set; }

        public string Description { get; set; }

        #endregion

        #region Navigation Properties

        public List<UserDto> Users { get; set; }

        #endregion
    }

}
