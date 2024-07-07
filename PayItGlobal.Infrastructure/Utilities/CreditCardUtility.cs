using System.Text.RegularExpressions;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.DTOs;

namespace PayItGlobal.Infrastructure.Utilities
{
    public class CreditCardUtility : ICreditCardUtility 
    {
        private const string cardRegex = "^(?:(?<Visa>4\\d{3})|(?<MasterCard>5[1-5]\\d{2})|(?<Discover>6011)|(?<DinersClub>(?:3[68]\\d{2})|(?:30[0-5]\\d))|(?<Amex>3[47]\\d{2}))([ -]?)(?(DinersClub)(?:\\d{6}\\1\\d{4})|(?(Amex)(?:\\d{6}\\1\\d{5})|(?:\\d{4}\\1\\d{4}\\1\\d{4})))$";

        public bool IsValidNumber(string cardNum)
        {
            Regex cardTest = new Regex(cardRegex);
            CreditCardTypeType? cardType = GetCardTypeFromNumber(cardNum);
            if (IsValidNumber(cardNum, cardType))
                return true;
            else
                return false;
        }

        private static bool IsValidNumber(string cardNum, CreditCardTypeType? cardType)
        {
            Regex cardTest = new Regex(cardRegex);
            if (cardTest.Match(cardNum).Groups[cardType.ToString()].Success)
            {
                if (PassesLuhnTest(cardNum))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public CreditCardTypeType? GetCardTypeFromNumber(string cardNum)
        {
            if (string.IsNullOrEmpty(cardNum)) { return null; }
            Regex cardTest = new Regex(cardRegex);
            GroupCollection gc = cardTest.Match(cardNum).Groups;
            if (gc[CreditCardTypeType.Amex.ToString()].Success)
            {
                return CreditCardTypeType.Amex;
            }
            else if (gc[CreditCardTypeType.MasterCard.ToString()].Success)
            {
                return CreditCardTypeType.MasterCard;
            }
            else if (gc[CreditCardTypeType.Visa.ToString()].Success)
            {
                return CreditCardTypeType.Visa;
            }
            else if (gc[CreditCardTypeType.Discover.ToString()].Success)
            {
                return CreditCardTypeType.Discover;
            }
            else
            {
                return null;
            }
        }


        private static bool PassesLuhnTest(string cardNumber)
        {
            cardNumber = cardNumber.Replace("-", "").Replace(" ", "");
            int[] digits = new int[cardNumber.Length];
            for (int len = 0; len < cardNumber.Length; len++)
            {
                digits[len] = Int32.Parse(cardNumber.Substring(len, 1));
            }
            int sum = 0;
            bool alt = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int curDigit = digits[i];
                if (alt)
                {
                    curDigit *= 2;
                    if (curDigit > 9)
                    {
                        curDigit -= 9;
                    }
                }
                sum += curDigit;
                alt = !alt;
            }
            return sum % 10 == 0;
        }
    }


}
