

using PayItGlobal.DTOs;

namespace PayItGlobal.Application.Interfaces
{
    public interface ICreditCardUtility
    {
        bool IsValidNumber(string cardNumber);
        CreditCardTypeType? GetCardTypeFromNumber(string cardNumber);
    }
}
