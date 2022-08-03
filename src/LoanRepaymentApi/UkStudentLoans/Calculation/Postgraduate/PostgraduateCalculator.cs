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
            request.PreviousProjections.SingleOrDefault(x => x.Period == request.Period - 1 && x.LoanType == request.Loan.Type);

        var balanceRemaining = previousPeriodResult?.DebtRemaining ?? request.Loan.BalanceRemaining;

        if (balanceRemaining <= 0)
        {
            return null;
        }

        if (_canLoanBeWrittenOffOperation.Execute(new CanLoanBeWrittenOffOperationFact
            {
                BirthDate = request.PersonDetails.BirthDate,
                LoanType = request.Loan.Type,
                PeriodDate = request.PeriodDate,
                FirstRepaymentDate = request.Loan.FirstRepaymentDate,
                AcademicYearLoanTakenOut = request.Loan.AcademicYearLoanTakenOut
            }))
        {
            return new UkStudentLoanProjection
            {
                RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff,
                LoanType = request.Loan.Type,
                Period = request.Period,
                PeriodDate = request.PeriodDate,
                DebtRemaining = 0,
                Paid = 0,
                InterestRate = 0,
                InterestApplied = 0,
                TotalPaid = previousPeriodResult?.TotalPaid ?? 0,
                TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0,
            };
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

        if (request.Salary < threshold)
        {
            return new UkStudentLoanProjection
            {
                RepaymentStatus = UkStudentLoanRepaymentStatus.NotPaying,
                LoanType = request.Loan.Type,
                Period = request.Period,
                PeriodDate = request.PeriodDate,
                DebtRemaining = balanceRemaining,
                Paid = 0,
                InterestRate = interestRate,
                InterestApplied = interestToApply,
                TotalPaid = previousPeriodResult?.TotalPaid ?? 0,
                TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0
            };
        }

        var amountAvailableForPayment = ((request.Salary - threshold) * 0.06m) / 12;
        var amountToPay = amountAvailableForPayment > balanceRemaining
            ? balanceRemaining
            : amountAvailableForPayment;
        var debtRemaining = balanceRemaining - amountToPay;

        return new UkStudentLoanProjection
        {
            RepaymentStatus = debtRemaining == 0 ? UkStudentLoanRepaymentStatus.PaidOff : UkStudentLoanRepaymentStatus.Paying,
            LoanType = request.Loan.Type,
            Period = request.Period,
            PeriodDate = request.PeriodDate,
            DebtRemaining = debtRemaining,
            Paid = amountToPay,
            InterestRate = interestRate,
            InterestApplied = interestToApply,
            TotalPaid = amountToPay + (previousPeriodResult?.TotalPaid ?? 0),
            TotalInterestPaid = interestToApply + (previousPeriodResult?.TotalInterestPaid ?? 0)
        };
    }
}