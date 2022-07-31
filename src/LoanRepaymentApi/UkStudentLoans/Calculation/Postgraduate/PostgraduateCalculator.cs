namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

public class PostgraduateCalculator : IPostgraduateCalculator
{
    public UkStudentLoanTypeResult? Run(PostgraduateCalculatorRequest request)
    {
        var previousPeriodResult =
            request.PreviousPeriods.SingleOrDefault(x => x.Period == request.Period - 1 && x.LoanType == request.Loan.Type);

        var balanceRemaining = previousPeriodResult?.DebtRemaining ?? request.Loan.BalanceRemaining;

        if (balanceRemaining <= 0)
        {
            return null;
        }

        // Apply Interest
        // TODO Calculate the interest rate ourselves: https://www.gov.uk/repaying-your-student-loan/what-you-pay
        var interestToApply = balanceRemaining * request.Loan.InterestRate / 12;
        balanceRemaining += interestToApply;

        // Pay Down Balance
        // TODO Use thresholds defined here: https://www.gov.uk/repaying-your-student-loan/what-you-pay

        var annualSalaryUsableForLoanRepayment = request.Income.AnnualSalaryBeforeTax;
        var amountAvailableForPayment =
            ((annualSalaryUsableForLoanRepayment - request.Loan.RepaymentThreshold) * 0.06m) / 12;
        var amountToPay = amountAvailableForPayment > balanceRemaining
            ? balanceRemaining
            : amountAvailableForPayment;
        var debtRemaining = balanceRemaining - amountToPay;

        return new UkStudentLoanTypeResult
        {
            LoanType = request.Loan.Type,
            Period = request.Period,
            PeriodDate = request.PeriodDate,
            DebtRemaining = debtRemaining,
            PaidInPeriod = amountToPay,
            InterestRate = request.Loan.InterestRate,
            InterestAppliedInPeriod = interestToApply,
            TotalPaid = amountToPay + (previousPeriodResult?.TotalPaid ?? 0),
            TotalInterestPaid = interestToApply + (previousPeriodResult?.TotalInterestPaid ?? 0),
        };
    }
}