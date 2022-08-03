namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class StandardTypeCalculator : IStandardTypeCalculator
{
    private readonly ICanLoanBeWrittenOffOperation _canLoanBeWrittenOffOperation;
    private readonly IThresholdOperation _thresholdOperation;
    private readonly IInterestRateOperation _interestRateOperation;

    public StandardTypeCalculator(
        ICanLoanBeWrittenOffOperation canLoanBeWrittenOffOperation,
        IThresholdOperation thresholdOperation,
        IInterestRateOperation interestRateOperation)
    {
        _canLoanBeWrittenOffOperation = canLoanBeWrittenOffOperation;
        _thresholdOperation = thresholdOperation;
        _interestRateOperation = interestRateOperation;
    }

    public List<UkStudentLoanProjection> Run(StandardTypeCalculatorRequest request)
    {
        var results = new List<UkStudentLoanProjection>();

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
                request.PreviousProjections.SingleOrDefault(x => x.Period == request.Period - 1 && x.LoanType == loan.Type);

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
                results.Add(new UkStudentLoanProjection
                {
                    RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff,
                    LoanType = loan.Type,
                    Period = request.Period,
                    PeriodDate = request.PeriodDate,
                    DebtRemaining = 0,
                    Paid = 0,
                    InterestRate = 0,
                    InterestApplied = 0,
                    TotalPaid = previousPeriodResult?.TotalPaid ?? 0,
                    TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0,
                });

                continue;
            }

            var interestRate = _interestRateOperation.Execute(new InterestRateOperationFact
            {
                LoanType = loan.Type,
                PeriodDate = request.PeriodDate,
                CourseStartDate = loan.CourseStartDate,
                CourseEndDate = loan.CourseEndDate,
                StudyingPartTime = loan.StudyingPartTime,
                Salary = request.Salary
            });
            
            var interestToApply = balanceRemaining * interestRate / 12;
            balanceRemaining += interestToApply;

            var threshold = _thresholdOperation.Execute(new ThresholdOperationFact
            {
                LoanType = loan.Type,
                PeriodDate = request.PeriodDate
            });

            var annualSalaryUsableForLoanRepayment = previousLoansThreshold ?? request.Salary;

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

            var result = new UkStudentLoanProjection
            {
                RepaymentStatus = debtRemaining == 0
                    ? UkStudentLoanRepaymentStatus.PaidOff
                    : UkStudentLoanRepaymentStatus.Paying,
                LoanType = loan.Type,
                Period = request.Period,
                PeriodDate = request.PeriodDate,
                DebtRemaining = debtRemaining,
                Paid = amountToPay,
                InterestRate = interestRate,
                InterestApplied = interestToApply,
                TotalPaid = amountToPay + (previousPeriodResult?.TotalPaid ?? 0),
                TotalInterestPaid = interestToApply + (previousPeriodResult?.TotalInterestPaid ?? 0),
            };

            previousLoansThreshold = threshold;
            results.Add(result);
        }

        return results;
    }
}