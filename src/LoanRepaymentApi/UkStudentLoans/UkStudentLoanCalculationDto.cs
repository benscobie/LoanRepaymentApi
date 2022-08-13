#nullable disable warnings

namespace LoanRepaymentApi.UkStudentLoans;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanCalculationDto
{
    public int AnnualSalaryBeforeTax { get; set; }

    public List<Adjustment> SalaryAdjustments { get; set; } = new();

    /// <summary>
    /// Required for Type 1 and Type 4 loans when below their respective academic year thresholds.
    /// </summary>
    public DateTimeOffset? BirthDate { get; set; }

    public List<UkStudentLoanDto> Loans { get; set; } = new();
}

public class UkStudentLoanCalculationDtoValidator : AbstractValidator<UkStudentLoanCalculationDto>
{
    public UkStudentLoanCalculationDtoValidator()
    {
        RuleFor(x => x.AnnualSalaryBeforeTax).GreaterThan(0);
        RuleFor(x => x.BirthDate).NotNull().Must(x => x <= DateTimeOffset.Now.AddYears(-15)).When(x =>
            x.Loans.Any(x => x.LoanType == UkStudentLoanType.Type1 && x.AcademicYearLoanTakenOut <= 2005) ||
            x.Loans.Any(x => x.LoanType == UkStudentLoanType.Type4 && x.AcademicYearLoanTakenOut <= 2006));
        RuleFor(x => x.Loans).Must(HaveUniqueLoanTypes).WithMessage("Only one loan of each type allowed.");
        RuleFor(x => x.SalaryAdjustments).Must(HaveMaximumAdjustmentOfOnePerMonth)
            .WithMessage("Only one salary adjustment allowed per month.");
        RuleFor(x => x.Loans).NotEmpty().WithMessage("At least one loan must be supplied.");

        RuleForEach(x => x.Loans)
            .ChildRules(loan =>
            {
                loan.RuleFor(x => x.LoanType).NotEmpty().IsInEnum();
                loan.RuleFor(x => x.BalanceRemaining).GreaterThan(0);
                loan.RuleFor(x => x.CourseStartDate).NotNull()
                    .When(x => x.LoanType == UkStudentLoanType.Type2);
                loan.RuleFor(x => x.CourseEndDate).NotNull()
                    .When(x => x.LoanType == UkStudentLoanType.Type2);
                loan.RuleFor(x => x.StudyingPartTime).NotNull()
                    .When(x => x.LoanType == UkStudentLoanType.Type2);
                loan.RuleFor(x => x.AcademicYearLoanTakenOut).NotNull().When(x =>
                    x.LoanType == UkStudentLoanType.Type1 || x.LoanType == UkStudentLoanType.Type4);
                loan.RuleFor(x => x.FirstRepaymentDate).NotNull().When(x =>
                    (x.LoanType == UkStudentLoanType.Type1 && x.AcademicYearLoanTakenOut >= 2006) ||
                    x.LoanType == UkStudentLoanType.Type4 || x.LoanType == UkStudentLoanType.Postgraduate ||
                    x.LoanType == UkStudentLoanType.Type2);
            });
    }

    private bool HaveMaximumAdjustmentOfOnePerMonth(UkStudentLoanCalculationDto loan, List<Adjustment> adjustments)
    {
        var groupedAdjustments = adjustments.GroupBy(x => new { x.Date.Year, x.Date.Month });

        return groupedAdjustments.Count() == adjustments.Count;
    }

    private bool HaveUniqueLoanTypes(UkStudentLoanCalculationDto loan, List<UkStudentLoanDto> loans)
    {
        return loans.GroupBy(o => o.LoanType).Count() == loans.Count;
    }
}