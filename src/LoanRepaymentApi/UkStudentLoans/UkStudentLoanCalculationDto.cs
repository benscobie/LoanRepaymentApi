﻿#nullable disable warnings

namespace LoanRepaymentApi.UkStudentLoans;

using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Calculation;

public class UkStudentLoanCalculationDto
{
    public int AnnualSalaryBeforeTax { get; set; }

    public List<Adjustment> SalaryAdjustments { get; set; } = new();

    public decimal SalaryGrowth { get; set; }

    public decimal AnnualEarningsGrowth { get; set; }

    /// <summary>
    /// Required for Type 1 and Type 4 loans when below their respective academic year thresholds.
    /// </summary>
    public DateTime? BirthDate { get; set; }

    public List<UkStudentLoanDto> Loans { get; set; } = new();

    public List<VoluntaryRepayment> VoluntaryRepayments { get; set; } = new();
}

public class UkStudentLoanCalculationDtoValidator : AbstractValidator<UkStudentLoanCalculationDto>
{
    public UkStudentLoanCalculationDtoValidator()
    {
        RuleFor(x => x.AnnualSalaryBeforeTax).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BirthDate).NotNull().Must(x => x <= DateTimeOffset.Now.AddYears(-15)).When(x =>
            x.Loans.Any(x => x.LoanType == UkStudentLoanType.Type1 && x.AcademicYearLoanTakenOut <= 2005) ||
            x.Loans.Any(x => x.LoanType == UkStudentLoanType.Type4 && x.AcademicYearLoanTakenOut <= 2006));
        RuleFor(x => x.Loans).Must(HaveUniqueLoanTypes).WithMessage("Only one loan of each type allowed.");
        RuleFor(x => x.SalaryGrowth).InclusiveBetween(-1, 1);
        RuleFor(x => x.AnnualEarningsGrowth).InclusiveBetween(-1, 1);
        RuleFor(x => x.SalaryAdjustments).Must(HaveMaximumAdjustmentOfOnePerMonth)
            .WithMessage("Only one salary adjustment allowed per month.");
        RuleFor(x => x.Loans).NotEmpty().WithMessage("At least one loan must be supplied.");
        RuleFor(x => x.VoluntaryRepayments).Must(HaveMaximumVoluntaryRepaymentOfOnePerMonth)
            .WithMessage("Only one voluntary repayment allowed per date, voluntary repayment type and loan type.");

        RuleForEach(x => x.Loans)
            .ChildRules(loan =>
            {
                loan.RuleFor(x => x.LoanType).NotEmpty().IsInEnum();
                loan.RuleFor(x => x.BalanceRemaining).GreaterThan(0);
                loan.RuleFor(x => x.CourseStartDate).NotNull()
                    .When(x => x.LoanType == UkStudentLoanType.Type2 || x.StudyingPartTime);
                loan.RuleFor(x => x.CourseEndDate).NotNull()
                    .When(x => x.LoanType != UkStudentLoanType.Type5);
                loan.RuleFor(x => x.StudyingPartTime).NotNull();
                loan.RuleFor(x => x.AcademicYearLoanTakenOut).NotNull().When(x =>
                    x.LoanType == UkStudentLoanType.Type1 || x.LoanType == UkStudentLoanType.Type4);
            });
    }

    private bool HaveMaximumVoluntaryRepaymentOfOnePerMonth(UkStudentLoanCalculationDto loan, List<VoluntaryRepayment> voluntaryRepayments)
    {
        var groupedVoluntaryRepayments = voluntaryRepayments.GroupBy(x => new { x.Date.Year, x.Date.Month, x.VoluntaryRepaymentType, x.LoanType });

        return groupedVoluntaryRepayments.Count() == voluntaryRepayments.Count;
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