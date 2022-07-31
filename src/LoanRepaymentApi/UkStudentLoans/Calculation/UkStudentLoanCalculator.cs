namespace LoanRepaymentApi.UkStudentLoans.Calculation;

using NodaTime;

public class UkStudentLoanCalculator : IUkStudentLoanCalculator
{
    private readonly IClock _clock;

    public UkStudentLoanCalculator(IClock clock)
    {
        _clock = clock;
    }
    
    public List<UkStudentLoanResult> Execute(UkStudentLoanCalculatorRequest request)
    {
        var results = new List<UkStudentLoanResult>();
        var period = 1;
        var loansToRepayInThresholdOrder = request.Loans
            .OrderByDescending(x => x.RepaymentThreshold);
        bool loansHaveDebt;
        var now = _clock.GetCurrentInstant().ToDateTimeUtc();

        do
        {
            var periodDate = now.AddMonths(period);
            loansHaveDebt = false;
            var typeResults = new List<UkStudentLoanTypeResult>();

            decimal allocationCarriedOver = 0;
            UkStudentLoan? previousLoanWithABalancePaid = null;
            foreach (var loan in loansToRepayInThresholdOrder)
            {
                var previousPeriodResult = results.SingleOrDefault(x => x.Period == period - 1)?.LoanResults
                    .SingleOrDefault(x => x.LoanType == loan.Type);

                var balanceRemaining = previousPeriodResult?.DebtRemaining ?? loan.BalanceRemaining;

                if (balanceRemaining <= 0)
                {
                    continue;
                }

                // Apply Interest
                // TODO Calculate the interest rate ourselves: https://www.gov.uk/repaying-your-student-loan/what-you-pay
                var interestToApply = balanceRemaining * loan.InterestRate / 12;
                balanceRemaining += interestToApply;

                // Pay Down Balance
                // TODO Use thresholds defined here: https://www.gov.uk/repaying-your-student-loan/what-you-pay
                
                var annualSalaryUsableForLoanRepayment =
                    previousLoanWithABalancePaid?.RepaymentThreshold ?? request.Income.AnnualSalaryBeforeTax;

                var amountAvailableForPayment = (((annualSalaryUsableForLoanRepayment - loan.RepaymentThreshold) * 0.09m) / 12) + allocationCarriedOver;

                decimal amountToPay;
                if (amountAvailableForPayment > balanceRemaining)
                {
                    amountToPay = balanceRemaining;
                    allocationCarriedOver = amountAvailableForPayment - balanceRemaining;
                }
                else
                {
                    amountToPay = amountAvailableForPayment;
                    allocationCarriedOver = 0;
                }

                var debtRemaining = balanceRemaining - amountToPay;

                var result = new UkStudentLoanTypeResult
                {
                    LoanType = loan.Type,
                    Period = period,
                    PeriodDate = periodDate,
                    DebtRemaining = debtRemaining,
                    PaidInPeriod = amountToPay,
                    InterestRate = loan.InterestRate,
                    InterestAppliedInPeriod = interestToApply,
                    TotalPaid = amountToPay + (previousPeriodResult?.TotalPaid ?? 0),
                    TotalInterestPaid = interestToApply + (previousPeriodResult?.TotalInterestPaid ?? 0),
                };

                loansHaveDebt = debtRemaining > 0 || loansHaveDebt;
                previousLoanWithABalancePaid = loan;
                typeResults.Add(result);
            }

            results.Add(new UkStudentLoanResult
            {
                Period = period,
                PeriodDate = periodDate,
                LoanResults = typeResults.OrderBy(x => x.LoanType).ToList()
            });

            period++;
        } while (loansHaveDebt);

        return results.OrderBy(x => x.Period).ToList();
    }
}