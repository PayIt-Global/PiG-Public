using System.Collections.Generic;

namespace PayItGlobal.DTOs
{

    public partial class PaymentDto : NotifyPropertyChangeDTO
    {
        #region Constructors
  
        public PaymentDto() {
        }

        public PaymentDto(int paymentID, global::System.Nullable<System.Guid> userID, string last4CC, global::System.DateTime 
            createdOn, global::System.Nullable<System.DateTime> modifiedOn, string businessName, 
            string invoiceNumber, string dynamicFieldVal, string dynamicFieldVal2, int portalID, 
            string payerFirstName, string payerLastName, string aCHName, global::System.Nullable<int> aRProfileID, 
            global::System.Nullable<int> subscriptionID, global::System.Nullable<bool> sentToQB, global::System.Nullable<int> loyaltyCampaignID, 
            string lastQBTransferMessage, bool isFromMobile, global::System.Nullable<int> specialTypeID, decimal total, 
            global::System.Nullable<int> specialTypeOptionID) {

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
        }

        #endregion

        #region Properties

        public int PaymentID 
        { 
          get { return _paymentID; } 
          set
          {
            ApplyPropertyChange<PaymentDto, int>(ref _paymentID, o => o.PaymentID, value);
          }
        }
        private int _paymentID;

        public global::System.Nullable<System.Guid> UserID 
        { 
          get { return _userID; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.Nullable<System.Guid>>(ref _userID, o => o.UserID, value);
          }
        }
        private global::System.Nullable<System.Guid> _userID;

        public string Last4CC 
        { 
          get { return _last4CC; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _last4CC, o => o.Last4CC, value);
          }
        }
        private string _last4CC;

        public global::System.DateTime CreatedOn 
        { 
          get { return _createdOn; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.DateTime>(ref _createdOn, o => o.CreatedOn, value);
          }
        }
        private global::System.DateTime _createdOn;

        public global::System.Nullable<System.DateTime> ModifiedOn 
        { 
          get { return _modifiedOn; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.Nullable<System.DateTime>>(ref _modifiedOn, o => o.ModifiedOn, value);
          }
        }
        private global::System.Nullable<System.DateTime> _modifiedOn;

        public string BusinessName 
        { 
          get { return _businessName; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _businessName, o => o.BusinessName, value);
          }
        }
        private string _businessName;

        public string InvoiceNumber 
        { 
          get { return _invoiceNumber; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _invoiceNumber, o => o.InvoiceNumber, value);
          }
        }
        private string _invoiceNumber;

        public string DynamicFieldVal 
        { 
          get { return _dynamicFieldVal; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _dynamicFieldVal, o => o.DynamicFieldVal, value);
          }
        }
        private string _dynamicFieldVal;

        public string DynamicFieldVal2 
        { 
          get { return _dynamicFieldVal2; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _dynamicFieldVal2, o => o.DynamicFieldVal2, value);
          }
        }
        private string _dynamicFieldVal2;

        public int PortalID 
        { 
          get { return _portalID; } 
          set
          {
            ApplyPropertyChange<PaymentDto, int>(ref _portalID, o => o.PortalID, value);
          }
        }
        private int _portalID;

        public string PayerFirstName 
        { 
          get { return _payerFirstName; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _payerFirstName, o => o.PayerFirstName, value);
          }
        }
        private string _payerFirstName;

        public string PayerLastName 
        { 
          get { return _payerLastName; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _payerLastName, o => o.PayerLastName, value);
          }
        }
        private string _payerLastName;

        public string ACHName 
        { 
          get { return _aCHName; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _aCHName, o => o.ACHName, value);
          }
        }
        private string _aCHName;

        public global::System.Nullable<int> ARProfileID 
        { 
          get { return _aRProfileID; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.Nullable<int>>(ref _aRProfileID, o => o.ARProfileID, value);
          }
        }
        private global::System.Nullable<int> _aRProfileID;

        public global::System.Nullable<int> SubscriptionID 
        { 
          get { return _subscriptionID; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.Nullable<int>>(ref _subscriptionID, o => o.SubscriptionID, value);
          }
        }
        private global::System.Nullable<int> _subscriptionID;

        public global::System.Nullable<bool> SentToQB 
        { 
          get { return _sentToQB; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.Nullable<bool>>(ref _sentToQB, o => o.SentToQB, value);
          }
        }
        private global::System.Nullable<bool> _sentToQB;

        public global::System.Nullable<int> LoyaltyCampaignID 
        { 
          get { return _loyaltyCampaignID; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.Nullable<int>>(ref _loyaltyCampaignID, o => o.LoyaltyCampaignID, value);
          }
        }
        private global::System.Nullable<int> _loyaltyCampaignID;

        public string LastQBTransferMessage 
        { 
          get { return _lastQBTransferMessage; } 
          set
          {
            ApplyPropertyChange<PaymentDto, string>(ref _lastQBTransferMessage, o => o.LastQBTransferMessage, value);
          }
        }
        private string _lastQBTransferMessage;

        public bool IsFromMobile 
        { 
          get { return _isFromMobile; } 
          set
          {
            ApplyPropertyChange<PaymentDto, bool>(ref _isFromMobile, o => o.IsFromMobile, value);
          }
        }
        private bool _isFromMobile;

        public global::System.Nullable<int> SpecialTypeID 
        { 
          get { return _specialTypeID; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.Nullable<int>>(ref _specialTypeID, o => o.SpecialTypeID, value);
          }
        }
        private global::System.Nullable<int> _specialTypeID;

        public decimal Total 
        { 
          get { return _total; } 
          set
          {
            ApplyPropertyChange<PaymentDto, decimal>(ref _total, o => o.Total, value);
          }
        }
        private decimal _total;

        public global::System.Nullable<int> SpecialTypeOptionID 
        { 
          get { return _specialTypeOptionID; } 
          set
          {
            ApplyPropertyChange<PaymentDto, global::System.Nullable<int>>(ref _specialTypeOptionID, o => o.SpecialTypeOptionID, value);
          }
        }
        private global::System.Nullable<int> _specialTypeOptionID;

        #endregion

        #region Navigation Properties


        #endregion
    }

}
