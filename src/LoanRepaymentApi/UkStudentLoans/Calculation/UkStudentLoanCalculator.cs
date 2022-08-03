namespace LoanRepaymentApi.UkStudentLoans.Calculation;

using System.Collections.Generic;
using System.Linq;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;
using LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using NodaTime;

public class UkStudentLoanCalculator : IUkStudentLoanCalculator
{
    private readonly IClock _clock;
    private readonly IStandardTypeCalculator _standardTypeCalculator;
    private readonly IPostgraduateCalculator _postgraduateCalculator;
    private readonly ISalaryOperation _salaryOperation;

    public UkStudentLoanCalculator(IClock clock,
        IStandardTypeCalculator standardTypeCalculator,
        IPostgraduateCalculator postgraduateCalculator,
        ISalaryOperation salaryOperation)
    {
        _clock = clock;
        _standardTypeCalculator = standardTypeCalculator;
        _postgraduateCalculator = postgraduateCalculator;
        _salaryOperation = salaryOperation;
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
            
            var result = new UkStudentLoanResult
            {
                Period = period,
                PeriodDate = periodDate
            };

            var previousResult = results.SingleOrDefault(x => x.Period == period - 1);

            result.Salary = _salaryOperation.Execute(new SalaryOperationFact
            {
                CurrentSalary = previousResult?.Salary ?? request.PersonDetails.AnnualSalaryBeforeTax,
                PeriodDate = periodDate,
                SalaryAdjustments = request.PersonDetails.SalaryAdjustments
            });

            result.Projections.AddRange(_standardTypeCalculator.Run(new StandardTypeCalculatorRequest(request.PersonDetails)
            {
                Loans = request.Loans.Where(x => standardLoanTypes.Contains(x.Type)).ToList(),
                Period = period,
                PeriodDate = periodDate,
                PreviousProjections = results.SelectMany(x => x.Projections).ToList(),
                Salary = result.Salary
            }));

            if (request.Loans.Any(x => x.Type == UkStudentLoanType.Postgraduate))
            {
                var postgraduateResult = _postgraduateCalculator.Run(
                    new PostgraduateCalculatorRequest(request.PersonDetails,
                        request.Loans.Single(x => x.Type == UkStudentLoanType.Postgraduate))
                    {
                        Period = period,
                        PeriodDate = periodDate,
                        PreviousProjections = results.SelectMany(x => x.Projections).ToList(),
                        Salary = result.Salary
                    });

                if (postgraduateResult != null)
                {
                    result.Projections.Add(postgraduateResult);
                }
            }

            loansHaveDebt = result.Projections.Any(x => x.DebtRemaining > 0);
            results.Add(result);
            period++;
        } while (loansHaveDebt);

        return results.OrderBy(x => x.Period).ToList();
    }
}