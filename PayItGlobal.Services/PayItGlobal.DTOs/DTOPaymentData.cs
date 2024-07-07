using System;

namespace PayItGlobal.DTOs
{
    [Serializable]
    public class DTOPaymentData
    {
        public int PaymentDataID { get; set; }
        public int? EncKeyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CCNumber { get; set; }
        public string CcnumberEnc { get; set; }
        public string Salt { get; set; }
        public string Tag { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string CSC { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public int? StateProvinceID { get; set; }
        public string StateName { get; set; }
        public string Zip { get; set; }
        public string Tele { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string DynamicFieldVal { get; set; }
        public string DynamicFieldVal2 { get; set; }
        public string BusinessName { get; set; }
    }
    [Serializable]
    public class DTOECheckPaymentData
    {
        public int PaymentDataID { get; set; }
        public string Bank_Acct_Name { get; set; }
        public string Bank_Aba_Code { get; set; }
        public string Bank_Acct_Num { get; set; }
        public string Bank_Acct_Type { get; set; }
        public string Bank_Name { get; set; }
        public string Echeck_Type { get; set; }
        public int? Bank_Check_Number { get; set; }
        public bool Recurring_Billing { get; set; }
        public string Bank_Acct_Type2 { get; set; }
        public string StateProvinceID { get; set; }
        public string StateName { get; set; }
        public string Zip { get; set; }
        public string Tele { get; set; }
        public string Addr1 { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string DynamicFieldVal { get; set; }
        public string DynamicFieldVal2 { get; set; }
        public string BusinessName { get; set; }
        public bool IsCash { get; set; }
    }
}
