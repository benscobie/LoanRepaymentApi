using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.FirstPossibleRepaymentDate;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Threshold;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.VoluntaryRepayment;

namespace LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;

public class StandardTypeCalculator : IStandardTypeCalculator
{
    private readonly ICanLoanBeWrittenOffOperation _canLoanBeWrittenOffOperation;
    private readonly IThresholdOperation _thresholdOperation;
    private readonly IInterestRateOperation _interestRateOperation;
    private readonly IFirstPossibleRepaymentDateOperation _firstPossibleRepaymentDateOperation;
    private readonly IVoluntaryRepaymentOperation _voluntaryRepaymentOperation;

    public StandardTypeCalculator(
        ICanLoanBeWrittenOffOperation canLoanBeWrittenOffOperation,
        IThresholdOperation thresholdOperation,
        IInterestRateOperation interestRateOperation,
        IFirstPossibleRepaymentDateOperation firstPossibleRepaymentDateOperation,
        IVoluntaryRepaymentOperation voluntaryRepaymentOperation)
    {
        _canLoanBeWrittenOffOperation = canLoanBeWrittenOffOperation;
        _thresholdOperation = thresholdOperation;
        _interestRateOperation = interestRateOperation;
        _firstPossibleRepaymentDateOperation = firstPossibleRepaymentDateOperation;
        _voluntaryRepaymentOperation = voluntaryRepaymentOperation;
    }

    public List<UkStudentLoanProjection> Run(StandardTypeCalculatorRequest request)
    {
        var results = new List<UkStudentLoanProjection>();

        var loanInterestRates = new Dictionary<UkStudentLoanType, decimal>();
        var balancesRemaining = new Dictionary<UkStudentLoanType, decimal>();
        foreach (var loan in request.Loans)
        {
            var interestRate = _interestRateOperation.Execute(new InterestRateOperationFact
            {
                LoanType = loan.Type,
                PeriodDate = request.PeriodDate,
                CourseStartDate = loan.CourseStartDate,
                CourseEndDate = loan.CourseEndDate,
                StudyingPartTime = loan.StudyingPartTime,
                Salary = request.Salary
            });

            loanInterestRates.Add(loan.Type, interestRate);

            var previousPeriodResult =
                request.PreviousProjections.SingleOrDefault(x =>
                    x.Period == request.Period - 1 && x.LoanType == loan.Type);

            var balanceRemaining = previousPeriodResult?.DebtRemaining ?? loan.BalanceRemaining;
            balancesRemaining.Add(loan.Type, balanceRemaining);
        }

        var voluntaryRepayments = _voluntaryRepaymentOperation.Execute(new VoluntaryRepaymentOperationFact
        {
            PeriodDate = request.PeriodDate,
            Loans = request.Loans,
            VoluntaryRepayments = request.VoluntaryRepayments,
            LoanBalancesRemaining = balancesRemaining,
            LoanInterestRates = loanInterestRates
        });

        var loansToRepayInThresholdOrder = request.Loans
            .OrderByDescending(x => _thresholdOperation.Execute(new ThresholdOperationFact
            {
                LoanType = x.Type,
                PeriodDate = request.PeriodDate,
                Period = request.Period,
                AnnualEarningsGrowth = request.AnnualEarningsGrowth,
                PreviousProjections = request.PreviousProjections
            })).ToList();

        decimal allocationCarriedOver = 0;
        int? previousLoansThreshold = null;
        foreach (var loan in loansToRepayInThresholdOrder)
        {
            var result = new UkStudentLoanProjection
            {
                LoanType = loan.Type,
                Period = request.Period,
                PeriodDate = request.PeriodDate
            };

            var threshold = _thresholdOperation.Execute(new ThresholdOperationFact
            {
                LoanType = loan.Type,
                PeriodDate = request.PeriodDate,
                Period = request.Period,
                PreviousProjections = request.PreviousProjections,
                AnnualEarningsGrowth = request.AnnualEarningsGrowth
            });
            result.Threshold = threshold;

            var previousPeriodResult =
                request.PreviousProjections.SingleOrDefault(x =>
                    x.Period == request.Period - 1 && x.LoanType == loan.Type);
            var balanceRemaining = balancesRemaining[loan.Type];

            if (balanceRemaining <= 0)
            {
                result.RepaymentStatus = previousPeriodResult?.RepaymentStatus ?? UkStudentLoanRepaymentStatus.PaidOff;
                result.TotalPaid = previousPeriodResult?.TotalPaid ?? 0;
                result.TotalInterestApplied = previousPeriodResult?.TotalInterestApplied ?? 0;
                results.Add(result);
                continue;
            }

            var firstPossibleRepaymentDate = _firstPossibleRepaymentDateOperation.Execute(
                new FirstPossibleRepaymentDateOperationFact
                {
                    LoanType = loan.Type,
                    CourseStartDate = loan.CourseStartDate,
                    CourseEndDate = loan.CourseEndDate,
                    StudyingPartTime = loan.StudyingPartTime
                });

            voluntaryRepayments.TryGetValue(loan.Type, out var voluntaryRepayment);
            result.Paid = voluntaryRepayment;
            result.TotalPaid = voluntaryRepayment + (previousPeriodResult?.TotalPaid ?? 0);
            balanceRemaining -= voluntaryRepayment;

            if (_canLoanBeWrittenOffOperation.Execute(
                    new CanLoanBeWrittenOffOperationFact
                    {
                        BirthDate = request.PersonDetails.BirthDate,
                        LoanType = loan.Type,
                        PeriodDate = request.PeriodDate,
                        FirstRepaymentDate = firstPossibleRepaymentDate,
                        AcademicYearLoanTakenOut = loan.AcademicYearLoanTakenOut
                    }))
            {
                result.RepaymentStatus = UkStudentLoanRepaymentStatus.WrittenOff;
                result.TotalInterestApplied = previousPeriodResult?.TotalInterestApplied ?? 0;
                results.Add(result);
                continue;
            }

            var interestRate = loanInterestRates[loan.Type];
            var interestToApply = balanceRemaining * interestRate / 12;
            balanceRemaining += interestToApply;

            var annualSalaryUsableForLoanRepayment = previousLoansThreshold ?? request.Salary;

            if ((firstPossibleRepaymentDate != null && request.PeriodDate < firstPossibleRepaymentDate) ||
                annualSalaryUsableForLoanRepayment < threshold)
            {
                result.RepaymentStatus = UkStudentLoanRepaymentStatus.NotPaying;
                result.DebtRemaining = balanceRemaining;
                result.InterestRate = interestRate;
                result.InterestApplied = interestToApply;
                result.TotalInterestApplied = interestToApply + (previousPeriodResult?.TotalInterestApplied ?? 0);
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
            result.Paid += amountToPay;
            result.InterestRate = interestRate;
            result.InterestApplied = interestToApply;
            result.TotalPaid += amountToPay;
            result.TotalInterestApplied = interestToApply + (previousPeriodResult?.TotalInterestApplied ?? 0);

            results.Add(result);
        }

        return results;
    }
}