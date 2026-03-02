using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    /// <summary>
    /// Тип фінансової транзакції.
    /// кожна транзакція - це або дохід або витрата.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Дохід: зарплата, фріланс, подарунок тощо
        /// </summary>
        Income = 0,

        /// <summary>
        /// Витрата: покупки, оплата послуг, їжа тощо
        /// </summary>
        Expense = 1
    }
}
