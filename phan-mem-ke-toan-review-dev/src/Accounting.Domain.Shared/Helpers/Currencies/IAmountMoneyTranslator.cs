using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Helpers.Currencies
{
    public interface IAmountMoneyTranslator
    {
        string ToWords(double number);
    }
}
