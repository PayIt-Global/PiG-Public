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

    public partial class TransactionDto
    {
        #region Constructors

        public TransactionDto() {
        }

        public TransactionDto(int transactionID, int? paymentID, System.DateTime transactionDate, string response, string responseText, string authCode, string tDTransactionID, string aVSResponse, string cVVResponse, string responseCode, string transactionType, int? subscriptionID, int? rebateID, string cardType, int? portalID, string payeeID, string last4CC, string name, string iPAddress, decimal? amount, int? employeePaymentID, int? employeePayoutID, bool? sentToQb, PaymentDto payment) {

          this.TransactionID = transactionID;
          this.PaymentID = paymentID;
          this.TransactionDate = transactionDate;
          this.Response = response;
          this.ResponseText = responseText;
          this.AuthCode = authCode;
          this.TDTransactionID = tDTransactionID;
          this.AVSResponse = aVSResponse;
          this.CVVResponse = cVVResponse;
          this.ResponseCode = responseCode;
          this.TransactionType = transactionType;
          this.SubscriptionID = subscriptionID;
          this.RebateID = rebateID;
          this.CardType = cardType;
          this.PortalID = portalID;
          this.PayeeID = payeeID;
          this.Last4CC = last4CC;
          this.Name = name;
          this.IPAddress = iPAddress;
          this.Amount = amount;
          this.EmployeePaymentID = employeePaymentID;
          this.EmployeePayoutID = employeePayoutID;
          this.SentToQb = sentToQb;
          this.Payment = payment;
        }

        #endregion

        #region Properties

        public int TransactionID { get; set; }

        public int? PaymentID { get; set; }

        public System.DateTime TransactionDate { get; set; }

        public string Response { get; set; }

        public string ResponseText { get; set; }

        public string AuthCode { get; set; }

        public string TDTransactionID { get; set; }

        public string AVSResponse { get; set; }

        public string CVVResponse { get; set; }

        public string ResponseCode { get; set; }

        public string TransactionType { get; set; }

        public int? SubscriptionID { get; set; }

        public int? RebateID { get; set; }

        public string CardType { get; set; }

        public int? PortalID { get; set; }

        public string PayeeID { get; set; }

        public string Last4CC { get; set; }

        public string Name { get; set; }

        public string IPAddress { get; set; }

        public decimal? Amount { get; set; }

        public int? EmployeePaymentID { get; set; }

        public int? EmployeePayoutID { get; set; }

        public bool? SentToQb { get; set; }

        #endregion

        #region Navigation Properties

        public PaymentDto Payment { get; set; }

        #endregion
    }

}
