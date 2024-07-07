using System.Collections.Generic;

namespace PayItGlobal.DTOs
{

    public partial class PaymentDataCCDto : NotifyPropertyChangeDTO
    {
        #region Constructors
  
        public PaymentDataCCDto() {
        }

        public PaymentDataCCDto(int paymentDataID, string title, global::System.Nullable<int> aRProfileID, global::System.Nullable<System.Guid> userID, string firstName, string lastName, string cCNumber, string expMonth, string expYear, string addr1, string addr2, string city, global::System.Nullable<int> stateProvinceID, string stateName, string zip, string tele, string email, string dynamicFieldVal, string dynamicFieldVal2, string businessName, global::System.DateTime createDate, global::System.Nullable<System.DateTime> editDate, global::System.Guid createdBy, global::System.Nullable<System.Guid> editedBy, global::System.Nullable<System.DateTime> lastAccessDate, string cSC, global::System.Nullable<int> mWProfileID, List<int> mobileWorkerDebitCards, List<int> subscriptions) {

          this.PaymentDataID = paymentDataID;
          this.Title = title;
          this.ARProfileID = aRProfileID;
          this.UserID = userID;
          this.FirstName = firstName;
          this.LastName = lastName;
          this.CCNumber = cCNumber;
          this.ExpMonth = expMonth;
          this.ExpYear = expYear;
          this.Addr1 = addr1;
          this.Addr2 = addr2;
          this.City = city;
          this.StateProvinceID = stateProvinceID;
          this.StateName = stateName;
          this.Zip = zip;
          this.Tele = tele;
          this.Email = email;
          this.DynamicFieldVal = dynamicFieldVal;
          this.DynamicFieldVal2 = dynamicFieldVal2;
          this.BusinessName = businessName;
          this.CreateDate = createDate;
          this.EditDate = editDate;
          this.CreatedBy = createdBy;
          this.EditedBy = editedBy;
          this.LastAccessDate = lastAccessDate;
          this.CSC = cSC;
          this.MWProfileID = mWProfileID;
        }

        #endregion

        #region Properties

        public int PaymentDataID 
        { 
          get { return _paymentDataID; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, int>(ref _paymentDataID, o => o.PaymentDataID, value);
          }
        }
        private int _paymentDataID;

        public string Title 
        { 
          get { return _title; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _title, o => o.Title, value);
          }
        }
        private string _title;

        public global::System.Nullable<int> ARProfileID 
        { 
          get { return _aRProfileID; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.Nullable<int>>(ref _aRProfileID, o => o.ARProfileID, value);
          }
        }
        private global::System.Nullable<int> _aRProfileID;

        public global::System.Nullable<System.Guid> UserID 
        { 
          get { return _userID; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.Nullable<System.Guid>>(ref _userID, o => o.UserID, value);
          }
        }
        private global::System.Nullable<System.Guid> _userID;

        public string FirstName 
        { 
          get { return _firstName; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _firstName, o => o.FirstName, value);
          }
        }
        private string _firstName;

        public string LastName 
        { 
          get { return _lastName; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _lastName, o => o.LastName, value);
          }
        }
        private string _lastName;

        public string CCNumber 
        { 
          get { return _cCNumber; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _cCNumber, o => o.CCNumber, value);
          }
        }
        private string _cCNumber;

        public string ExpMonth 
        { 
          get { return _expMonth; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _expMonth, o => o.ExpMonth, value);
          }
        }
        private string _expMonth;

        public string ExpYear 
        { 
          get { return _expYear; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _expYear, o => o.ExpYear, value);
          }
        }
        private string _expYear;

        public string Addr1 
        { 
          get { return _addr1; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _addr1, o => o.Addr1, value);
          }
        }
        private string _addr1;

        public string Addr2 
        { 
          get { return _addr2; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _addr2, o => o.Addr2, value);
          }
        }
        private string _addr2;

        public string City 
        { 
          get { return _city; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _city, o => o.City, value);
          }
        }
        private string _city;

        public global::System.Nullable<int> StateProvinceID 
        { 
          get { return _stateProvinceID; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.Nullable<int>>(ref _stateProvinceID, o => o.StateProvinceID, value);
          }
        }
        private global::System.Nullable<int> _stateProvinceID;

        public string StateName 
        { 
          get { return _stateName; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _stateName, o => o.StateName, value);
          }
        }
        private string _stateName;

        public string Zip 
        { 
          get { return _zip; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _zip, o => o.Zip, value);
          }
        }
        private string _zip;

        public string Tele 
        { 
          get { return _tele; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _tele, o => o.Tele, value);
          }
        }
        private string _tele;

        public string Email 
        { 
          get { return _email; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _email, o => o.Email, value);
          }
        }
        private string _email;

        public string DynamicFieldVal 
        { 
          get { return _dynamicFieldVal; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _dynamicFieldVal, o => o.DynamicFieldVal, value);
          }
        }
        private string _dynamicFieldVal;

        public string DynamicFieldVal2 
        { 
          get { return _dynamicFieldVal2; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _dynamicFieldVal2, o => o.DynamicFieldVal2, value);
          }
        }
        private string _dynamicFieldVal2;

        public string BusinessName 
        { 
          get { return _businessName; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _businessName, o => o.BusinessName, value);
          }
        }
        private string _businessName;

        public global::System.DateTime CreateDate 
        { 
          get { return _createDate; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.DateTime>(ref _createDate, o => o.CreateDate, value);
          }
        }
        private global::System.DateTime _createDate;

        public global::System.Nullable<System.DateTime> EditDate 
        { 
          get { return _editDate; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.Nullable<System.DateTime>>(ref _editDate, o => o.EditDate, value);
          }
        }
        private global::System.Nullable<System.DateTime> _editDate;

        public global::System.Guid CreatedBy 
        { 
          get { return _createdBy; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.Guid>(ref _createdBy, o => o.CreatedBy, value);
          }
        }
        private global::System.Guid _createdBy;

        public global::System.Nullable<System.Guid> EditedBy 
        { 
          get { return _editedBy; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.Nullable<System.Guid>>(ref _editedBy, o => o.EditedBy, value);
          }
        }
        private global::System.Nullable<System.Guid> _editedBy;

        public global::System.Nullable<System.DateTime> LastAccessDate 
        { 
          get { return _lastAccessDate; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.Nullable<System.DateTime>>(ref _lastAccessDate, o => o.LastAccessDate, value);
          }
        }
        private global::System.Nullable<System.DateTime> _lastAccessDate;

        public string CSC 
        { 
          get { return _cSC; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, string>(ref _cSC, o => o.CSC, value);
          }
        }
        private string _cSC;

        public global::System.Nullable<int> MWProfileID 
        { 
          get { return _mWProfileID; } 
          set
          {
            ApplyPropertyChange<PaymentDataCCDto, global::System.Nullable<int>>(ref _mWProfileID, o => o.MWProfileID, value);
          }
        }
        private global::System.Nullable<int> _mWProfileID;

        #endregion

        #region Navigation Properties


        #endregion
    }

}
