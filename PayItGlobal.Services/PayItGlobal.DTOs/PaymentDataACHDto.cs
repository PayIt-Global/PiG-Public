
using System.Collections.Generic;

namespace PayItGlobal.DTOs
{

    public partial class PaymentDataACHDto : NotifyPropertyChangeDTO
    {
        #region Constructors
  
        public PaymentDataACHDto() {
        }

        public PaymentDataACHDto(int paymentDataID, string title, global::System.Nullable<int> aRProfileID, global::System.Nullable<System.Guid> userID, string bank_Acct_Name, string bank_Aba_Code, string bank_Acct_Num, string bank_Acct_Type, string bank_Name, string echeck_Type, string bank_Acct_Type2, int stateProvinceID, string stateName, string zip, string tele, string addr1, string addr2, string city, string email, string dynamicFieldVal, string dynamicFieldVal2, string businessName, global::System.DateTime createDate, global::System.Nullable<System.DateTime> editDate, global::System.Nullable<System.Guid> createdBy, global::System.Nullable<System.Guid> editedBy, global::System.Nullable<System.DateTime> lastAccessDate, global::System.Nullable<int> mwprofileid, List<int> clientEmployees, List<int> subscriptions, List<int> payonlinePaymentsToEmployees) {

          this.PaymentDataID = paymentDataID;
          this.Title = title;
          this.ARProfileID = aRProfileID;
          this.UserID = userID;
          this.Bank_Acct_Name = bank_Acct_Name;
          this.Bank_Aba_Code = bank_Aba_Code;
          this.Bank_Acct_Num = bank_Acct_Num;
          this.Bank_Acct_Type = bank_Acct_Type;
          this.Bank_Name = bank_Name;
          this.Echeck_Type = echeck_Type;
          this.Bank_Acct_Type2 = bank_Acct_Type2;
          this.StateProvinceID = stateProvinceID;
          this.StateName = stateName;
          this.Zip = zip;
          this.Tele = tele;
          this.Addr1 = addr1;
          this.Addr2 = addr2;
          this.City = city;
          this.Email = email;
          this.DynamicFieldVal = dynamicFieldVal;
          this.DynamicFieldVal2 = dynamicFieldVal2;
          this.BusinessName = businessName;
          this.CreateDate = createDate;
          this.EditDate = editDate;
          this.CreatedBy = createdBy;
          this.EditedBy = editedBy;
          this.LastAccessDate = lastAccessDate;
          this.Mwprofileid = mwprofileid;
        }

        #endregion

        #region Properties

        public int PaymentDataID 
        { 
          get { return _paymentDataID; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, int>(ref _paymentDataID, o => o.PaymentDataID, value);
          }
        }
        private int _paymentDataID;

        public string Title 
        { 
          get { return _title; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _title, o => o.Title, value);
          }
        }
        private string _title;

        public global::System.Nullable<int> ARProfileID 
        { 
          get { return _aRProfileID; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, global::System.Nullable<int>>(ref _aRProfileID, o => o.ARProfileID, value);
          }
        }
        private global::System.Nullable<int> _aRProfileID;

        public global::System.Nullable<System.Guid> UserID 
        { 
          get { return _userID; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, global::System.Nullable<System.Guid>>(ref _userID, o => o.UserID, value);
          }
        }
        private global::System.Nullable<System.Guid> _userID;

        public string Bank_Acct_Name 
        { 
          get { return _bank_Acct_Name; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _bank_Acct_Name, o => o.Bank_Acct_Name, value);
          }
        }
        private string _bank_Acct_Name;

        public string Bank_Aba_Code 
        { 
          get { return _bank_Aba_Code; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _bank_Aba_Code, o => o.Bank_Aba_Code, value);
          }
        }
        private string _bank_Aba_Code;

        public string Bank_Acct_Num 
        { 
          get { return _bank_Acct_Num; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _bank_Acct_Num, o => o.Bank_Acct_Num, value);
          }
        }
        private string _bank_Acct_Num;

        public string Bank_Acct_Type 
        { 
          get { return _bank_Acct_Type; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _bank_Acct_Type, o => o.Bank_Acct_Type, value);
          }
        }
        private string _bank_Acct_Type;

        public string Bank_Name 
        { 
          get { return _bank_Name; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _bank_Name, o => o.Bank_Name, value);
          }
        }
        private string _bank_Name;

        public string Echeck_Type 
        { 
          get { return _echeck_Type; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _echeck_Type, o => o.Echeck_Type, value);
          }
        }
        private string _echeck_Type;

        public string Bank_Acct_Type2 
        { 
          get { return _bank_Acct_Type2; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _bank_Acct_Type2, o => o.Bank_Acct_Type2, value);
          }
        }
        private string _bank_Acct_Type2;

        public int StateProvinceID 
        { 
          get { return _stateProvinceID; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, int>(ref _stateProvinceID, o => o.StateProvinceID, value);
          }
        }
        private int _stateProvinceID;

        public string StateName 
        { 
          get { return _stateName; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _stateName, o => o.StateName, value);
          }
        }
        private string _stateName;

        public string Zip 
        { 
          get { return _zip; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _zip, o => o.Zip, value);
          }
        }
        private string _zip;

        public string Tele 
        { 
          get { return _tele; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _tele, o => o.Tele, value);
          }
        }
        private string _tele;

        public string Addr1 
        { 
          get { return _addr1; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _addr1, o => o.Addr1, value);
          }
        }
        private string _addr1;

        public string Addr2 
        { 
          get { return _addr2; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _addr2, o => o.Addr2, value);
          }
        }
        private string _addr2;

        public string City 
        { 
          get { return _city; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _city, o => o.City, value);
          }
        }
        private string _city;

        public string Email 
        { 
          get { return _email; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _email, o => o.Email, value);
          }
        }
        private string _email;

        public string DynamicFieldVal 
        { 
          get { return _dynamicFieldVal; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _dynamicFieldVal, o => o.DynamicFieldVal, value);
          }
        }
        private string _dynamicFieldVal;

        public string DynamicFieldVal2 
        { 
          get { return _dynamicFieldVal2; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _dynamicFieldVal2, o => o.DynamicFieldVal2, value);
          }
        }
        private string _dynamicFieldVal2;

        public string BusinessName 
        { 
          get { return _businessName; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, string>(ref _businessName, o => o.BusinessName, value);
          }
        }
        private string _businessName;

        public global::System.DateTime CreateDate 
        { 
          get { return _createDate; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, global::System.DateTime>(ref _createDate, o => o.CreateDate, value);
          }
        }
        private global::System.DateTime _createDate;

        public global::System.Nullable<System.DateTime> EditDate 
        { 
          get { return _editDate; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, global::System.Nullable<System.DateTime>>(ref _editDate, o => o.EditDate, value);
          }
        }
        private global::System.Nullable<System.DateTime> _editDate;

        public global::System.Nullable<System.Guid> CreatedBy 
        { 
          get { return _createdBy; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, global::System.Nullable<System.Guid>>(ref _createdBy, o => o.CreatedBy, value);
          }
        }
        private global::System.Nullable<System.Guid> _createdBy;

        public global::System.Nullable<System.Guid> EditedBy 
        { 
          get { return _editedBy; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, global::System.Nullable<System.Guid>>(ref _editedBy, o => o.EditedBy, value);
          }
        }
        private global::System.Nullable<System.Guid> _editedBy;

        public global::System.Nullable<System.DateTime> LastAccessDate 
        { 
          get { return _lastAccessDate; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, global::System.Nullable<System.DateTime>>(ref _lastAccessDate, o => o.LastAccessDate, value);
          }
        }
        private global::System.Nullable<System.DateTime> _lastAccessDate;

        public global::System.Nullable<int> Mwprofileid 
        { 
          get { return _mwprofileid; } 
          set
          {
            ApplyPropertyChange<PaymentDataACHDto, global::System.Nullable<int>>(ref _mwprofileid, o => o.Mwprofileid, value);
          }
        }
        private global::System.Nullable<int> _mwprofileid;

        #endregion

        #region Navigation Properties



        #endregion
    }

}
