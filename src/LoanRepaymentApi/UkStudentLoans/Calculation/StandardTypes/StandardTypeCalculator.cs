namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

public class StandardTypeCalculator : IStandardTypeCalculator
{
    public List<UkStudentLoanTypeResult> Run(StandardTypeCalculatorRequest request)
    {
        var results = new List<UkStudentLoanTypeResult>();
        
        var loansToRepayInThresholdOrder = request.Loans
            .OrderByDescending(x => x.RepaymentThreshold);

        decimal allocationCarriedOver = 0;
        UkStudentLoan? previousLoanWithABalancePaid = null;
        foreach (var loan in loansToRepayInThresholdOrder)
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

            var annualSalaryUsableForLoanRepayment =
                previousLoanWithABalancePaid?.RepaymentThreshold ?? request.Income.AnnualSalaryBeforeTax;

            var amountAvailableForPayment =
                (((annualSalaryUsableForLoanRepayment - loan.RepaymentThreshold) * 0.09m) / 12) + allocationCarriedOver;

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
                Period = request.Period,
                PeriodDate = request.PeriodDate,
                DebtRemaining = debtRemaining,
                PaidInPeriod = amountToPay,
                InterestRate = loan.InterestRate,
                InterestAppliedInPeriod = interestToApply,
                TotalPaid = amountToPay + (previousPeriodResult?.TotalPaid ?? 0),
                TotalInterestPaid = interestToApply + (previousPeriodResult?.TotalInterestPaid ?? 0),
            };
            
            previousLoanWithABalancePaid = loan;
            results.Add(result);
        }

        return results;
    }
}