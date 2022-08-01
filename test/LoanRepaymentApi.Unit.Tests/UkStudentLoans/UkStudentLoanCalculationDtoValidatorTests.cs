namespace LoanRepaymentApi.Tests.UkStudentLoans;

using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.TestHelper;
using LoanRepaymentApi.UkStudentLoans;
using Xunit;

public class UkStudentLoanCalculationDtoValidatorTests
{
    [Fact]
    public void Validate_WithAllInvalidData_ShouldFailValidation()
    {
        // Arrange
        var validator = new UkStudentLoanCalculationDtoValidator();

        var dto = new UkStudentLoanCalculationDto
        {
            Loans = new List<UkStudentLoanDto>
            {
                new UkStudentLoanDto
                {
                    LoanType = UkStudentLoanType.Type1,
                    BalanceRemaining = 0,
                    InterestRate = 0,
                    RepaymentThreshold = 0
                },
                new UkStudentLoanDto
                {
                    LoanType = UkStudentLoanType.Type1,
                    BalanceRemaining = 1500,
                    InterestRate = 0.1m,
                    RepaymentThreshold = 25000,
                }
            },
            AnnualSalaryBeforeTax = 0
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Loans);
        result.ShouldHaveValidationErrorFor(x => x.AnnualSalaryBeforeTax);
        result.ShouldHaveValidationErrorFor("Loans[0].BalanceRemaining");
        result.ShouldHaveValidationErrorFor("Loans[0].InterestRate");
        result.ShouldHaveValidationErrorFor("Loans[0].RepaymentThreshold");
    }
}