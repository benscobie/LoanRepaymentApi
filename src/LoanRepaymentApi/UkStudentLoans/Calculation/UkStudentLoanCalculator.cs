namespace LoanRepaymentApi.UkStudentLoans.Calculation;

using System.Collections.Generic;
using System.Linq;
using LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using NodaTime;

public class UkStudentLoanCalculator : IUkStudentLoanCalculator
{
    private readonly IClock _clock;
    private readonly IStandardTypeCalculator _standardTypeCalculator;
    private readonly IPostgraduateCalculator _postgraduateCalculator;

    public UkStudentLoanCalculator(IClock clock, IStandardTypeCalculator standardTypeCalculator,
        IPostgraduateCalculator postgraduateCalculator)
    {
        _clock = clock;
        _standardTypeCalculator = standardTypeCalculator;
        _postgraduateCalculator = postgraduateCalculator;
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

            var loanTypeResults = new List<UkStudentLoanTypeResult>();

            loanTypeResults.AddRange(_standardTypeCalculator.Run(new StandardTypeCalculatorRequest(request.PersonDetails)
            {
                Loans = request.Loans.Where(x => standardLoanTypes.Contains(x.Type)).ToList(),
                Period = period,
                PeriodDate = periodDate,
                PreviousPeriods = results.SelectMany(x => x.LoanResults).ToList()
            }));

            if (request.Loans.Any(x => x.Type == UkStudentLoanType.Postgraduate))
            {
                var postgraduateResult = _postgraduateCalculator.Run(
                    new PostgraduateCalculatorRequest(request.PersonDetails,
                        request.Loans.Single(x => x.Type == UkStudentLoanType.Postgraduate))
                    {
                        Period = period,
                        PeriodDate = periodDate,
                        PreviousPeriods = results.SelectMany(x => x.LoanResults).ToList()
                    });

                if (postgraduateResult != null)
                {
                    loanTypeResults.Add(postgraduateResult);
                }
            }

            loansHaveDebt = loanTypeResults.Any(x => x.DebtRemaining > 0);

            results.Add(new UkStudentLoanResult
            {
                Period = period,
                PeriodDate = periodDate,
                LoanResults = loanTypeResults.OrderBy(x => x.LoanType).ToList()
            });

            period++;
        } while (loansHaveDebt);

        return results.OrderBy(x => x.Period).ToList();
    }
}