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
    public partial class Payment {

        public Payment()
        {
            this.Total = 0m;
            this.Transactions = new List<Transaction>();
            OnCreated();
        }

        public int PaymentID { get; set; }

        public Guid? UserID { get; set; }

        public string? Last4CC { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? BusinessName { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? DynamicFieldVal { get; set; }

        public string? DynamicFieldVal2 { get; set; }

        public int PortalID { get; set; }

        public string? PayerFirstName { get; set; }

        public string? PayerLastName { get; set; }

        public string? ACHName { get; set; }

        public int? ARProfileID { get; set; }

        public int? SubscriptionID { get; set; }

        public bool? SentToQB { get; set; }

        public int? LoyaltyCampaignID { get; set; }

        public string? LastQBTransferMessage { get; set; }

        public bool IsFromMobile { get; set; }

        public int? SpecialTypeID { get; set; }

        public decimal Total { get; set; }

        public int? SpecialTypeOptionID { get; set; }

        public virtual PayOnlinePortal Portal { get; set; }

        public virtual User User { get; set; }

        public virtual IList<Transaction> Transactions { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
