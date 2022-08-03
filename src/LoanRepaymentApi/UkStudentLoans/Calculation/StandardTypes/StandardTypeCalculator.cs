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
                request.PreviousProjections.SingleOrDefault(x =>
                    x.Period == request.Period - 1 && x.LoanType == loan.Type);

            var balanceRemaining = previousPeriodResult?.DebtRemaining ?? loan.BalanceRemaining;

            if (balanceRemaining <= 0)
            {
                continue;
            }

            var result = new UkStudentLoanProjection
            {
                LoanType = loan.Type,
                Period = request.Period,
                PeriodDate = request.PeriodDate
            };

            if (_canLoanBeWrittenOffOperation.Execute(new CanLoanBeWrittenOffOperationFact
                {
                    BirthDate = request.PersonDetails.BirthDate,
                    LoanType = loan.Type,
                    PeriodDate = request.PeriodDate,
                    FirstRepaymentDate = loan.FirstRepaymentDate,
                    AcademicYearLoanTakenOut = loan.AcademicYearLoanTakenOut
                }))
            {
                result.RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff;
                result.TotalPaid = previousPeriodResult?.TotalPaid ?? 0;
                result.TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0;
                results.Add(result);
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

            if ((loan.FirstRepaymentDate != null && request.PeriodDate < loan.FirstRepaymentDate) ||
                annualSalaryUsableForLoanRepayment < threshold)
            {
                result.RepaymentStatus = UkStudentLoanRepaymentStatus.NotPaying;
                result.DebtRemaining = balanceRemaining;
                result.InterestRate = interestRate;
                result.InterestApplied = interestToApply;
                result.TotalPaid = previousPeriodResult?.TotalPaid ?? 0;
                result.TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0;
                results.Add(result);
                continue;
            }
            
            var percentageOfSalary = loan.Type == UkStudentLoanType.Postgraduate ? 0.06m : 0.09m;
            var amountAvailableForPayment =
                (((annualSalaryUsableForLoanRepayment - threshold) * percentageOfSalary) / 12) + allocationCarriedOver;

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
            previousLoansThreshold = threshold;

            result.RepaymentStatus = debtRemaining == 0
                ? UkStudentLoanRepaymentStatus.PaidOff
                : UkStudentLoanRepaymentStatus.Paying;
            result.DebtRemaining = debtRemaining;
            result.Paid = amountToPay;
            result.InterestRate = interestRate;
            result.InterestApplied = interestToApply;
            result.TotalPaid = amountToPay + (previousPeriodResult?.TotalPaid ?? 0);
            result.TotalInterestPaid = interestToApply + (previousPeriodResult?.TotalInterestPaid ?? 0);

            results.Add(result);
        }

        return results;
    }
}