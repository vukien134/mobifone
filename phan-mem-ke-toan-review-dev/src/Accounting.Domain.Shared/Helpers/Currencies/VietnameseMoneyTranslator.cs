using Accounting.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Accounting.Helpers.Currencies
{
    public class VietnameseMoneyTranslator : IAmountMoneyTranslator
    {
        private readonly string _readCurrency;
        private readonly string _readDecimal;
        private readonly int? _formatTotal;
        private readonly string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private readonly string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ", "nghìn", "triệu" };
        public VietnameseMoneyTranslator(string currencyCode, string readCurrency,
                                string readDecimal, int? formatTotal)
        {
            _readCurrency = readCurrency;
            _readDecimal = readDecimal;
            _formatTotal = formatTotal;
        }
        public string ToWords(double inputNumber)
        {
            if (inputNumber == 0)
            {
                return unitNumbers[0].UpperFirstChar() + " " + _readCurrency;
            }

            var isNegative = false;
            if (inputNumber < 0)
            {
                isNegative = true;
                inputNumber = Math.Abs(inputNumber);
            }

            var numberFormatInfo = GetCurrentNumberFormatInfo();
            var formatDecimal = GetFormatDecimal();
            var formatNumber = string.Format("{0:n" + formatDecimal + "}", inputNumber);
            var partsNumber = formatNumber.Split(numberFormatInfo.CurrencyDecimalSeparator);

            var originNumberByWord = ReadOriginNumber(partsNumber[0]);
            string amountByWords = originNumberByWord.Trim() + " " + _readCurrency;
            string decimalNumberByWord = "";
            if (partsNumber.Length >= 2)
            {
                decimalNumberByWord = ReadDecimalNumber(partsNumber[1]);
            }

            if (!string.IsNullOrEmpty(decimalNumberByWord))
            {
                amountByWords = amountByWords + " và " + decimalNumberByWord.Trim() + " " + _readDecimal;
            }

            if (isNegative)
            {
                amountByWords = "Âm " + amountByWords;
            }

            return amountByWords.Trim().UpperFirstChar();
        }
        private string ReadOriginNumber(string originPart)
        {
            var numberFormatInfo = GetCurrentNumberFormatInfo();
            var amountByWords = "";
            var groupDigits = originPart.Split(numberFormatInfo.CurrencyGroupSeparator);
            int placeValue = groupDigits.Length - 1;

            for (int i = 0; i < groupDigits.Length; i++)
            {
                var result = ReadGroupDigit(groupDigits[i], placeValue);
                amountByWords = amountByWords + result;
                placeValue--;
            }

            return amountByWords.Trim();
        }
        private string ReadDecimalNumber(string decimalPart)
        {
            var number = Convert.ToInt32(decimalPart);
            if (number == 0)
            {
                return "";
            }

            return ReadGroupDigit(decimalPart, 0);
        }
        private string ReadGroupDigit(string groupDigit, int placeValue)
        {
            var result = "";
            int positionDigit = groupDigit.Length;
            if (positionDigit == 0)
            {
                result = unitNumbers[0] + result;
                return result;
            }

            if (Convert.ToInt32(groupDigit) == 0) return result;

            int ones, tens, hundreds;

            while (positionDigit > 0)
            {
                tens = hundreds = -1;
                ones = Convert.ToInt32(groupDigit.Substring(positionDigit - 1, 1));

                positionDigit--;
                if (positionDigit > 0)
                {
                    tens = Convert.ToInt32(groupDigit.Substring(positionDigit - 1, 1));

                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        hundreds = Convert.ToInt32(groupDigit.Substring(positionDigit - 1, 1));
                        positionDigit--;
                    }
                }

                if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                {
                    result = placeValues[placeValue] + result;
                }

                if ((ones == 1) && (tens > 1))
                {
                    result = "mốt " + result;
                }
                else
                {
                    if ((ones == 5) && (tens > 0))
                    {
                        result = "lăm " + result;
                    }
                    else if ((ones == 4) && (tens >= 2))
                    {
                        result = "tư " + result;
                    }
                    else if (ones > 0)
                    {
                        result = unitNumbers[ones] + " " + result;
                    }
                }
                if (tens < 0)
                    break;
                else
                {
                    if ((tens == 0) && (ones > 0)) result = "linh " + result;
                    if (tens == 1) result = "mười " + result;
                    if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                }
                if (hundreds < 0) break;
                else
                {
                    if ((hundreds > 0) || (tens > 0) || (ones > 0))
                        result = unitNumbers[hundreds] + " trăm " + result;
                }
                result = " " + result;
            }

            return result;
        }
        private string GetFormatDecimal()
        {
            if (_formatTotal == null || _formatTotal.Value == 0)
            {
                return "0";
            }

            return _formatTotal.Value.ToString();
        }
        private NumberFormatInfo GetCurrentNumberFormatInfo()
        {
            return Thread.CurrentThread.CurrentCulture.NumberFormat;
        }
    }
}
