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

    public partial class AspNetUserDto
    {
        #region Constructors

        public AspNetUserDto() {
        }

        public AspNetUserDto(string id, string userName, string normalizedUserName, string email, string normalizedEmail, bool? emailConfirmed, string passwordHash, string securityStamp, string concurrencyStamp, string phoneNumber, bool? phoneNumberConfirmed, bool? twoFactorEnabled, System.DateTime? lockoutEnd, bool? lockoutEnabled, int? accessFailedCount, string fullName, List<AspNetUserClaimDto> aspNetUserClaims, List<AspNetUserLoginDto> aspNetUserLogins, List<AspNetUserTokenDto> aspNetUserTokens, List<RefreshTokenDto> refreshTokens, List<AspNetRoleDto> aspNetRoles) {

          this.Id = id;
          this.UserName = userName;
          this.NormalizedUserName = normalizedUserName;
          this.Email = email;
          this.NormalizedEmail = normalizedEmail;
          this.EmailConfirmed = emailConfirmed;
          this.PasswordHash = passwordHash;
          this.SecurityStamp = securityStamp;
          this.ConcurrencyStamp = concurrencyStamp;
          this.PhoneNumber = phoneNumber;
          this.PhoneNumberConfirmed = phoneNumberConfirmed;
          this.TwoFactorEnabled = twoFactorEnabled;
          this.LockoutEnd = lockoutEnd;
          this.LockoutEnabled = lockoutEnabled;
          this.AccessFailedCount = accessFailedCount;
          this.FullName = fullName;
          this.AspNetUserClaims = aspNetUserClaims;
          this.AspNetUserLogins = aspNetUserLogins;
          this.AspNetUserTokens = aspNetUserTokens;
          this.RefreshTokens = refreshTokens;
          this.AspNetRoles = aspNetRoles;
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool? EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string PhoneNumber { get; set; }

        public bool? PhoneNumberConfirmed { get; set; }

        public bool? TwoFactorEnabled { get; set; }

        public System.DateTime? LockoutEnd { get; set; }

        public bool? LockoutEnabled { get; set; }

        public int? AccessFailedCount { get; set; }

        public string FullName { get; set; }

        #endregion

        #region Navigation Properties

        public List<AspNetUserClaimDto> AspNetUserClaims { get; set; }

        public List<AspNetUserLoginDto> AspNetUserLogins { get; set; }

        public List<AspNetUserTokenDto> AspNetUserTokens { get; set; }

        public List<RefreshTokenDto> RefreshTokens { get; set; }

        public List<AspNetRoleDto> AspNetRoles { get; set; }

        #endregion
    }

}
