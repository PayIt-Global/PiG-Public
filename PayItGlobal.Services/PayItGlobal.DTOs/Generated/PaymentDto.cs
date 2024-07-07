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

    public partial class PaymentDto
    {
        #region Constructors

        public PaymentDto() {
        }

        public PaymentDto(int paymentID, System.Guid? userID, string last4CC, System.DateTime createdOn, System.DateTime? modifiedOn, string businessName, string invoiceNumber, string dynamicFieldVal, string dynamicFieldVal2, int portalID, string payerFirstName, string payerLastName, string aCHName, int? aRProfileID, int? subscriptionID, bool? sentToQB, int? loyaltyCampaignID, string lastQBTransferMessage, bool isFromMobile, int? specialTypeID, decimal total, int? specialTypeOptionID, PayOnlinePortalDto portal, UserDto user, List<TransactionDto> transactions) {

          this.PaymentID = paymentID;
          this.UserID = userID;
          this.Last4CC = last4CC;
          this.CreatedOn = createdOn;
          this.ModifiedOn = modifiedOn;
          this.BusinessName = businessName;
          this.InvoiceNumber = invoiceNumber;
          this.DynamicFieldVal = dynamicFieldVal;
          this.DynamicFieldVal2 = dynamicFieldVal2;
          this.PortalID = portalID;
          this.PayerFirstName = payerFirstName;
          this.PayerLastName = payerLastName;
          this.ACHName = aCHName;
          this.ARProfileID = aRProfileID;
          this.SubscriptionID = subscriptionID;
          this.SentToQB = sentToQB;
          this.LoyaltyCampaignID = loyaltyCampaignID;
          this.LastQBTransferMessage = lastQBTransferMessage;
          this.IsFromMobile = isFromMobile;
          this.SpecialTypeID = specialTypeID;
          this.Total = total;
          this.SpecialTypeOptionID = specialTypeOptionID;
          this.Portal = portal;
          this.User = user;
          this.Transactions = transactions;
        }

        #endregion

        #region Properties

        public int PaymentID { get; set; }

        public System.Guid? UserID { get; set; }

        public string Last4CC { get; set; }

        public System.DateTime CreatedOn { get; set; }

        public System.DateTime? ModifiedOn { get; set; }

        public string BusinessName { get; set; }

        public string InvoiceNumber { get; set; }

        public string DynamicFieldVal { get; set; }

        public string DynamicFieldVal2 { get; set; }

        public int PortalID { get; set; }

        public string PayerFirstName { get; set; }

        public string PayerLastName { get; set; }

        public string ACHName { get; set; }

        public int? ARProfileID { get; set; }

        public int? SubscriptionID { get; set; }

        public bool? SentToQB { get; set; }

        public int? LoyaltyCampaignID { get; set; }

        public string LastQBTransferMessage { get; set; }

        public bool IsFromMobile { get; set; }

        public int? SpecialTypeID { get; set; }

        public decimal Total { get; set; }

        public int? SpecialTypeOptionID { get; set; }

        #endregion

        #region Navigation Properties

        public PayOnlinePortalDto Portal { get; set; }

        public UserDto User { get; set; }

        public List<TransactionDto> Transactions { get; set; }

        #endregion
    }

}
