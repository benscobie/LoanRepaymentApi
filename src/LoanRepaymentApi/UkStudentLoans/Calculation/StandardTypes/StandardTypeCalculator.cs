namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

using LoanRepaymentApi.UkStudentLoans.Calculation.Operations;

public class StandardTypeCalculator : IStandardTypeCalculator
{
    private readonly ICanLoanBeWrittenOffOperation _canLoanBeWrittenOffOperation;

    public StandardTypeCalculator(ICanLoanBeWrittenOffOperation canLoanBeWrittenOffOperation)
    {
        _canLoanBeWrittenOffOperation = canLoanBeWrittenOffOperation;
    }

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

            if (_canLoanBeWrittenOffOperation.Execute(new CanLoanBeWrittenOffOperationFact
                {
                    BirthDate = request.PersonDetails.BirthDate,
                    LoanType = loan.Type,
                    PeriodDate = request.PeriodDate,
                    FirstRepaymentDate = loan.FirstRepaymentDate,
                    AcademicYearLoanTakenOut = loan.AcademicYearLoanTakenOut
                }))
            {
                results.Add(new UkStudentLoanTypeResult
                {
                    RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff,
                    LoanType = loan.Type,
                    Period = request.Period,
                    PeriodDate = request.PeriodDate,
                    DebtRemaining = 0,
                    PaidInPeriod = 0,
                    InterestRate = loan.InterestRate,
                    InterestAppliedInPeriod = 0,
                    TotalPaid = previousPeriodResult?.TotalPaid ?? 0,
                    TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0,
                });

                continue;
            }

            // Apply Interest
            // TODO Calculate the interest rate ourselves: https://www.gov.uk/repaying-your-student-loan/what-you-pay
            var interestToApply = balanceRemaining * loan.InterestRate / 12;
            balanceRemaining += interestToApply;

            // Pay Down Balance
            // TODO Use thresholds defined here: https://www.gov.uk/repaying-your-student-loan/what-you-pay

            var annualSalaryUsableForLoanRepayment =
                previousLoanWithABalancePaid?.RepaymentThreshold ?? request.PersonDetails.AnnualSalaryBeforeTax;

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
                RepaymentStatus = debtRemaining == 0 ? UkStudentLoanRepaymentStatus.PaidOff : UkStudentLoanRepaymentStatus.Paying,
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