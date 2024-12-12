namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public class Plan2LowerAndUpperThresholds : IPlan2LowerAndUpperThresholds
    {
        public Plan2LowerAndUpperThreshold Get(DateTime periodDate)
        {
            if (periodDate >= new DateTime(2025, 04, 06, 0, 0, 0))
            {
                return new Plan2LowerAndUpperThreshold { LowerThreshold = 28470, UpperThreshold = 51245 };
            }

            return new Plan2LowerAndUpperThreshold { LowerThreshold = 27295, UpperThreshold = 49130 };
        }
    }
}
