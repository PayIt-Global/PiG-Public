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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace PayItGlobal.Domain.Entities
{
    public partial class Client {

        public Client()
        {
            this.UsesSlCreatePublicUser = false;
            this.InfoFromClientNeeded = false;
            this.PreInvoiceMonthlyRecur = false;
            this.UsesSfCreatePublicUser = false;
            this.Portals = new List<PayOnlinePortal>();
            this.Users_ClientID = new List<User>();
            OnCreated();
        }

        public int ClientID { get; set; }

        public int? FranchiseID { get; set; }

        public string Name { get; set; }

        public Guid AddedByID { get; set; }

        public DateTime AddedDate { get; set; }

        public string AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string City { get; set; }

        public int StateProvinceID { get; set; }

        public string Phone { get; set; }

        public string? Fax { get; set; }

        public string PostalCode { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? Email { get; set; }

        public string? ContactFname { get; set; }

        public int MvcCmsPortalID { get; set; }

        public string? ContactLname { get; set; }

        public bool SetupIsComplete { get; set; }

        public bool Active { get; set; }

        public bool UsesSalesReceipts { get; set; }

        public bool UsesQB { get; set; }

        public bool UsesQBOE { get; set; }

        public string? ConnectionTicket { get; set; }

        public bool UsesUnitQuantity { get; set; }

        public bool UsesQBOEOld { get; set; }

        public int? InvoicesToRemember { get; set; }

        public bool QBInvoiceEachJob { get; set; }

        public bool QBNameLastFirst { get; set; }

        public bool QBSkippBillingAddrLine1 { get; set; }

        public bool QBDefaultOnDemand { get; set; }

        public bool QBDefaultInvoiceStyleOnly { get; set; }

        public bool InitialSyncIsDone { get; set; }

        public bool QBUsesClasses { get; set; }

        public bool UsesOnlineRegistration { get; set; }

        public bool UsesCardSwipe { get; set; }

        public bool UsesWebGenPOS { get; set; }

        public bool UsesQBNonInvoiced { get; set; }

        public bool SkipCustomersFromQB { get; set; }

        public bool QBLoadInitialPrice { get; set; }

        public bool UsesConvenienceFee { get; set; }

        public bool SendFailMailOnlyToClient { get; set; }

        public string? Ccforallreceipts { get; set; }

        public bool Usesemployeepay { get; set; }

        public decimal? Srpriceperhour { get; set; }

        public decimal? Srpricepershift { get; set; }

        public decimal? ConvenienceFeeAmount { get; set; }

        public bool QBCreateInvoiceOnFail { get; set; }

        public bool CombinedSettlement { get; set; }

        public bool UsesClientCustomerID { get; set; }

        public decimal? ConvenienceFeeMaxCCAmount { get; set; }

        public bool UsesZeeWiseImport { get; set; }

        public string? NameOnApplication { get; set; }

        public bool UsesFromExternalCheckout { get; set; }

        public bool ConvenienceFeeIsPercent { get; set; }

        public string? QboeOauthAccessToken { get; set; }

        public string? QboeOauthAccessTokenSecret { get; set; }

        public bool UsesSlCreatePublicUser { get; set; }

        public string? QboeOauthRealmId { get; set; }

        public int? QboeOauthDataSource { get; set; }

        public bool PauseCardVerify { get; set; }

        public bool QBCreateInvoiceOnRecurFail { get; set; }

        public bool InfoFromClientNeeded { get; set; }

        public bool PreInvoiceMonthlyRecur { get; set; }

        public bool UsesSchoolLeader { get; set; }

        public bool UsesSalesForce { get; set; }

        public bool UsesSfCreatePublicUser { get; set; }

        public bool UsesAmexBlue { get; set; }

        public bool UsesAdobeSign { get; set; }

        public bool ProcessSptAsSingleTransaction { get; set; }

        public bool UsesQbLocations { get; set; }

        public bool UsesSchoolLeaderArOnly { get; set; }

        public string? QboeOauthVersion { get; set; }

        public DateTime? QboeOauthLastConnectTime { get; set; }

        public DateTime? QboeOauthLastRefreshTime { get; set; }

        public string? QboeOauth2AccessToken { get; set; }

        public DateTime? QboeOauth2LastConnectTime { get; set; }

        public bool UsesDocusign { get; set; }

        public int? EncKeyID { get; set; }

        public int? AffiliateID { get; set; }

        public bool UsesProcare { get; set; }

        public decimal? AchConvenienceFeeAmount { get; set; }

        public decimal? LateFeeCc { get; set; }

        public decimal? LateFeeAch { get; set; }

        public virtual User PayEzCreator { get; set; }

        public virtual IList<PayOnlinePortal> Portals { get; set; }

        public virtual IList<User> Users_ClientID { get; set; }

        public virtual StateProvince StateProvince { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
