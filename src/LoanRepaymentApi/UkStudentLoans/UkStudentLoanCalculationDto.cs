#nullable disable warnings

namespace LoanRepaymentApi.UkStudentLoans;

using FluentValidation;

public class UkStudentLoanCalculationDto
{
    public int AnnualSalaryBeforeTax { get; set; }

    public List<UkStudentLoanDto> Loans { get; set; }
}

public class UkStudentLoanCalculationDtoValidator : AbstractValidator<UkStudentLoanCalculationDto>
{
    public UkStudentLoanCalculationDtoValidator()
    {
        RuleFor(x => x.AnnualSalaryBeforeTax).GreaterThan(0);
        RuleFor(x => x.Loans).Must(HaveUniqueLoanTypes).WithMessage("Only one loan of each type allowed.");
        RuleFor(x => x.Loans).NotEmpty().WithMessage("At least one loan must be supplied.");

        RuleForEach(x => x.Loans)
            .ChildRules(loan =>
            {
                loan.RuleFor(x => x.LoanType).NotEmpty().IsInEnum();
                loan.RuleFor(x => x.BalanceRemaining).GreaterThan(0);
                loan.RuleFor(x => x.InterestRate).GreaterThan(0);
                loan.RuleFor(x => x.RepaymentThreshold).GreaterThan(0);
            });
    }

    private bool HaveUniqueLoanTypes(UkStudentLoanCalculationDto loan, List<UkStudentLoanDto> loans)
    {
        return loans.GroupBy(o => o.LoanType).Count() == loans.Count;
    }
}