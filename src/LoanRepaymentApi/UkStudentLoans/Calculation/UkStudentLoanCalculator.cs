namespace LoanRepaymentApi.UkStudentLoans.Calculation;

using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using NodaTime;

public class UkStudentLoanCalculator : IUkStudentLoanCalculator
{
    private readonly IClock _clock;
    private readonly IStandardTypeCalculator _standardTypeCalculator;

    public UkStudentLoanCalculator(IClock clock, IStandardTypeCalculator standardTypeCalculator)
    {
        _clock = clock;
        _standardTypeCalculator = standardTypeCalculator;
    }
    
    public List<UkStudentLoanResult> Run(UkStudentLoanCalculatorRequest request)
    {
        var results = new List<UkStudentLoanResult>();
        var period = 1;
        bool loansHaveDebt;
        var now = _clock.GetCurrentInstant().ToDateTimeUtc();
        
        var standardLoanTypes = new[] { UkStudentLoanType.Type1, UkStudentLoanType.Type2, UkStudentLoanType.Type4 };

        do
        {
            var periodDate = now.AddMonths(period);
            var standardTypeResults = _standardTypeCalculator.Run(new StandardTypeCalculatorRequest(request.Income)
            {
                Loans = request.Loans.Where(x => standardLoanTypes.Contains(x.Type)).ToList(),
                Period = period,
                PeriodDate = periodDate,
                PreviousPeriods = results.SelectMany(x => x.LoanResults).ToList()
            });
            
            // TODO Call post grad calc

            loansHaveDebt = standardTypeResults.Any(x => x.DebtRemaining > 0);

            results.Add(new UkStudentLoanResult
            {
                Period = period,
                PeriodDate = periodDate,
                LoanResults = standardTypeResults.OrderBy(x => x.LoanType).ToList()
            });

            period++;
        } while (loansHaveDebt);

        return results.OrderBy(x => x.Period).ToList();
    }
}