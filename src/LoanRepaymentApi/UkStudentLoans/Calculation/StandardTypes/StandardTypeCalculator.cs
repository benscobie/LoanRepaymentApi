namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

using LoanRepaymentApi.UkStudentLoans.Calculation.Operations;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class StandardTypeCalculator : IStandardTypeCalculator
{
    private readonly ICanLoanBeWrittenOffOperation _canLoanBeWrittenOffOperation;
    private readonly IThresholdOperation _thresholdOperation;

    public StandardTypeCalculator(
        ICanLoanBeWrittenOffOperation canLoanBeWrittenOffOperation,
        IThresholdOperation thresholdOperation)
    {
        _canLoanBeWrittenOffOperation = canLoanBeWrittenOffOperation;
        _thresholdOperation = thresholdOperation;
    }

    public List<UkStudentLoanTypeResult> Run(StandardTypeCalculatorRequest request)
    {
        var results = new List<UkStudentLoanTypeResult>();

        var loansToRepayInThresholdOrder = request.Loans
            .OrderByDescending(x => _thresholdOperation.Execute(new ThresholdOperationFact
            {
                LoanType = x.Type,
                PeriodDate = request.PeriodDate
            })).ToList();

        decimal allocationCarriedOver = 0;
        int? previousLoansThreshold = null;
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
            var threshold = _thresholdOperation.Execute(new ThresholdOperationFact
            {
                LoanType = loan.Type,
                PeriodDate = request.PeriodDate
            });

            var annualSalaryUsableForLoanRepayment =
                previousLoansThreshold ?? request.PersonDetails.AnnualSalaryBeforeTax;

            var amountAvailableForPayment =
                (((annualSalaryUsableForLoanRepayment - threshold) * 0.09m) / 12) + allocationCarriedOver;

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
                RepaymentStatus = debtRemaining == 0
                    ? UkStudentLoanRepaymentStatus.PaidOff
                    : UkStudentLoanRepaymentStatus.Paying,
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

            previousLoansThreshold = threshold;
            results.Add(result);
        }

        return results;
    }
}