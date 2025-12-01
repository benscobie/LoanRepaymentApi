namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest
{
    public class Plan2LowerAndUpperThresholds : IPlan2LowerAndUpperThresholds
    {
        public Plan2LowerAndUpperThreshold Get(DateTime periodDate)
        {
            if (periodDate >= new DateTime(2026, 04, 06, 0, 0, 0))
            {
                return new Plan2LowerAndUpperThreshold { LowerThreshold = 29_385, UpperThreshold = 52_885 };
            }
            else if (periodDate >= new DateTime(2025, 04, 06, 0, 0, 0))
            {
                return new Plan2LowerAndUpperThreshold { LowerThreshold = 28_470, UpperThreshold = 51_245 };
            }

            return new Plan2LowerAndUpperThreshold { LowerThreshold = 27_295, UpperThreshold = 49_130 };
        }
    }
}
