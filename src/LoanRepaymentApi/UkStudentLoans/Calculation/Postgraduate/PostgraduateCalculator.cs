namespace LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;

using LoanRepaymentApi.UkStudentLoans.Calculation.Operations;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;

public class PostgraduateCalculator : IPostgraduateCalculator
{
    private readonly ICanLoanBeWrittenOffOperation _canLoanBeWrittenOffOperation;
    private readonly IThresholdOperation _thresholdOperation;

    public PostgraduateCalculator(
        ICanLoanBeWrittenOffOperation canLoanBeWrittenOffOperation,
        IThresholdOperation thresholdOperation)
    {
        _canLoanBeWrittenOffOperation = canLoanBeWrittenOffOperation;
        _thresholdOperation = thresholdOperation;
    }
    
    public UkStudentLoanTypeResult? Run(PostgraduateCalculatorRequest request)
    {
        var previousPeriodResult =
            request.PreviousPeriods.SingleOrDefault(x => x.Period == request.Period - 1 && x.LoanType == request.Loan.Type);

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
            return new UkStudentLoanTypeResult
            {
                RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff,
                LoanType = request.Loan.Type,
                Period = request.Period,
                PeriodDate = request.PeriodDate,
                DebtRemaining = 0,
                PaidInPeriod = 0,
                InterestRate = request.Loan.InterestRate,
                InterestAppliedInPeriod = 0,
                TotalPaid = previousPeriodResult?.TotalPaid ?? 0,
                TotalInterestPaid = previousPeriodResult?.TotalInterestPaid ?? 0,
            };
        }

        // Apply Interest
        // TODO Calculate the interest rate ourselves: https://www.gov.uk/repaying-your-student-loan/what-you-pay
        var interestToApply = balanceRemaining * request.Loan.InterestRate / 12;
        balanceRemaining += interestToApply;

        // Pay Down Balance
        var threshold = _thresholdOperation.Execute(new ThresholdOperationFact
        {
            LoanType = request.Loan.Type,
            PeriodDate = request.PeriodDate
        });

        var annualSalaryUsableForLoanRepayment = request.PersonDetails.AnnualSalaryBeforeTax;
        var amountAvailableForPayment =
            ((annualSalaryUsableForLoanRepayment - threshold) * 0.06m) / 12;
        var amountToPay = amountAvailableForPayment > balanceRemaining
            ? balanceRemaining
            : amountAvailableForPayment;
        var debtRemaining = balanceRemaining - amountToPay;

        return new UkStudentLoanTypeResult
        {
            RepaymentStatus = debtRemaining == 0 ? UkStudentLoanRepaymentStatus.PaidOff : UkStudentLoanRepaymentStatus.Paying,
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