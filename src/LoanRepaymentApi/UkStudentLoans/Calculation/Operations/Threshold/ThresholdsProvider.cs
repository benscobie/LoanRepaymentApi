namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class ThresholdsProvider : IThresholdsProvider
{
    public List<ThresholdBand> Get()
    {
        return new List<ThresholdBand>
        {
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                DateFrom = new DateTime(2023, 04, 06, 0, 0, 0),
                DateTo = new DateTime(2024, 04, 06, 0, 0, 0),
                Threshold = 22015
            },
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                DateFrom = new DateTime(2024, 04, 06, 0, 0, 0),
                DateTo = new DateTime(2025, 04, 06, 0, 0, 0),
                Threshold = 24990
            },
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                DateFrom = new DateTime(2025, 04, 06, 0, 0, 0),
                DateTo = new DateTime(2026, 04, 06, 0, 0, 0),
                Threshold = 26065
            },
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                DateFrom = new DateTime(2026, 04, 06, 0, 0, 0),
                DateTo = null,
                Threshold = 26900
            },
            new()
            {
                LoanType = UkStudentLoanType.Type2,
                DateFrom = new DateTime(2023, 04, 06, 0, 0, 0),
                DateTo = new DateTime(2025, 04, 06, 0, 0, 0),
                Threshold = 27295
            },
            new()
            {
                LoanType = UkStudentLoanType.Type2,
                DateFrom = new DateTime(2025, 04, 06, 0, 0, 0),
                DateTo = new DateTime(2026, 04, 06, 0, 0, 0),
                Threshold = 28470
            },
            new()
            {
                LoanType = UkStudentLoanType.Type4,
                DateFrom = new DateTime(2023, 04, 06, 0, 0, 0),
                DateTo = new DateTime(2024, 04, 06, 0, 0, 0),
                Threshold = 27660
            },
            new()
            {
                LoanType = UkStudentLoanType.Type4,
                DateFrom = new DateTime(2024, 04, 06, 0, 0, 0),
                DateTo = null,
                Threshold = 31395
            },
            new()
            {
                LoanType = UkStudentLoanType.Postgraduate,
                DateFrom = new DateTime(2023, 04, 06, 0, 0, 0),
                DateTo = null,
                Threshold = 21000
            },
            new()
            {
                LoanType = UkStudentLoanType.Type5,
                DateFrom = new DateTime(2023, 04, 06, 0, 0, 0),
                DateTo = null,
                Threshold = 25000
            }
        };
    }
}