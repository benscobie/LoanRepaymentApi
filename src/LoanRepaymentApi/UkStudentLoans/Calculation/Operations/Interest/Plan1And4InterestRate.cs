﻿namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public class Plan1And4InterestRate : IPlan1And4InterestRate
    {
        public decimal Get(DateTime periodDate)
        {
            if (periodDate >= new DateTime(2024, 09, 01, 0, 0, 0))
            {
                return 0.043m;
            }

            return 0.0625m;
        }
    }
}
