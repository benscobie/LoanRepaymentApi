namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

public class PostgraduateCalculator : IPostgraduateCalculator
{
    public List<UkStudentLoanTypeResult> Run(PostgraduateCalculatorRequest request)
    {
        var results = new List<UkStudentLoanTypeResult>();

        foreach (var loan in request.Loans)
        {
            var previousPeriodResult =
                request.PreviousPeriods.SingleOrDefault(x => x.Period == request.Period - 1 && x.LoanType == loan.Type);

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

            var annualSalaryUsableForLoanRepayment = request.Income.AnnualSalaryBeforeTax;
            var amountAvailableForPayment =
                ((annualSalaryUsableForLoanRepayment - loan.RepaymentThreshold) * 0.06m) / 12;
            var amountToPay = amountAvailableForPayment > balanceRemaining
                ? balanceRemaining
                : amountAvailableForPayment;
            var debtRemaining = balanceRemaining - amountToPay;

            var result = new UkStudentLoanTypeResult
            {
                LoanType = loan.Type,
                Period = request.Period,
                PeriodDate = request.PeriodDate,
                DebtRemaining = debtRemaining,
                PaidInPeriod = amountToPay,
                InterestRate = loan.InterestRate,
                InterestAppliedInPeriod = interestToApply,
                TotalPaid = amountToPay + (previousPeriodResult?.TotalPaid ?? 0),
                TotalInterestPaid = interestToApply + (previousPeriodResult?.TotalInterestPaid ?? 0),
            };

            results.Add(result);
        }

        return results;
    }
}