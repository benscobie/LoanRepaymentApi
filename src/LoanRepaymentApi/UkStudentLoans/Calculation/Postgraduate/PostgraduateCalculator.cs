namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class PostgraduateCalculator : IPostgraduateCalculator
{
    private readonly ICanLoanBeWrittenOffOperation _canLoanBeWrittenOffOperation;
    private readonly IThresholdOperation _thresholdOperation;
    private readonly IInterestRateOperation _interestRateOperation;

    public PostgraduateCalculator(
        ICanLoanBeWrittenOffOperation canLoanBeWrittenOffOperation,
        IThresholdOperation thresholdOperation,
        IInterestRateOperation interestRateOperation)
    {
        _canLoanBeWrittenOffOperation = canLoanBeWrittenOffOperation;
        _thresholdOperation = thresholdOperation;
        _interestRateOperation = interestRateOperation;
    }

    public UkStudentLoanProjection? Run(PostgraduateCalculatorRequest request)
    {
        var previousPeriodResult =
            request.PreviousProjections.SingleOrDefault(x =>
                x.Period == request.Period - 1 && x.LoanType == request.Loan.Type);

        var balanceRemaining = previousPeriodResult?.DebtRemaining ?? request.Loan.BalanceRemaining;

        if (balanceRemaining <= 0)
        {
            return null;
        }

        var result = new UkStudentLoanProjection
        {
            LoanType = request.Loan.Type,
            Period = request.Period,
            PeriodDate = request.PeriodDate
        };

        if (_canLoanBeWrittenOffOperation.Execute(new CanLoanBeWrittenOffOperationFact
            {
                BirthDate = request.PersonDetails.BirthDate,
                LoanType = request.Loan.Type,
                PeriodDate = request.PeriodDate,
                FirstRepaymentDate = request.Loan.FirstRepaymentDate,
                AcademicYearLoanTakenOut = request.Loan.AcademicYearLoanTakenOut
            }))
        {
            result.RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff;
            result.TotalPaid = previousPeriodResult?.TotalPaid ?? 0;
            result.TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0;
            return result;
        }

        var interestRate = _interestRateOperation.Execute(new InterestRateOperationFact
        {
            LoanType = request.Loan.Type,
            PeriodDate = request.PeriodDate,
            CourseStartDate = request.Loan.CourseStartDate,
            CourseEndDate = request.Loan.CourseEndDate,
            StudyingPartTime = request.Loan.StudyingPartTime,
            Salary = request.Salary
        });

        var interestToApply = balanceRemaining * interestRate / 12;
        balanceRemaining += interestToApply;

        var threshold = _thresholdOperation.Execute(new ThresholdOperationFact
        {
            LoanType = request.Loan.Type,
            PeriodDate = request.PeriodDate
        });

        if ((request.Loan.FirstRepaymentDate != null && request.PeriodDate < request.Loan.FirstRepaymentDate) ||
            request.Salary < threshold)
        {
            result.RepaymentStatus = UkStudentLoanRepaymentStatus.NotPaying;
            result.DebtRemaining = balanceRemaining;
            result.InterestRate = interestRate;
            result.InterestApplied = interestToApply;
            result.TotalPaid = previousPeriodResult?.TotalPaid ?? 0;
            result.TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0;
            return result;
        }

        var amountAvailableForPayment = ((request.Salary - threshold) * 0.06m) / 12;
        var amountToPay = amountAvailableForPayment > balanceRemaining
            ? balanceRemaining
            : amountAvailableForPayment;
        var debtRemaining = balanceRemaining - amountToPay;

        result.RepaymentStatus = debtRemaining == 0
            ? UkStudentLoanRepaymentStatus.PaidOff
            : UkStudentLoanRepaymentStatus.Paying;
        result.DebtRemaining = debtRemaining;
        result.Paid = amountToPay;
        result.InterestRate = interestRate;
        result.InterestApplied = interestToApply;
        result.TotalPaid = amountToPay + (previousPeriodResult?.TotalPaid ?? 0);
        result.TotalInterestPaid = interestToApply + (previousPeriodResult?.TotalInterestPaid ?? 0);
        return result;
    }
}