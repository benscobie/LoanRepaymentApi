namespace LoanRepaymentApi.Tests.UkStudentLoans;

using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.TestHelper;
using LoanRepaymentApi.UkStudentLoans;
using Xunit;

public class UkStudentLoanCalculationDtoValidatorTests
{
    [Theory]
    [MemberData(nameof(ValidLoanDataSupplied))]
    public void Validate_WithValidData_ShouldPassValidation(UkStudentLoanCalculationDto dto)
    {
        // Arrange
        var validator = new UkStudentLoanCalculationDtoValidator();

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> ValidLoanDataSupplied()
    {
        return new List<object[]>
        {
            new object[]
            {
                new UkStudentLoanCalculationDto
                {
                    BirthDate = DateTimeOffset.Now,
                    Loans = new List<UkStudentLoanDto>
                    {
                        new UkStudentLoanDto
                        {
                            LoanType = UkStudentLoanType.Type1,
                            BalanceRemaining = 1,
                            InterestRate = 1,
                            RepaymentThreshold = 1,
                            AcademicYearLoanTakenOut = 2005,
                        }
                    },
                    AnnualSalaryBeforeTax = 1
                }
            },
            new object[]
            {
                new UkStudentLoanCalculationDto
                {
                    Loans = new List<UkStudentLoanDto>
                    {
                        new UkStudentLoanDto
                        {
                            LoanType = UkStudentLoanType.Type1,
                            BalanceRemaining = 1,
                            InterestRate = 1,
                            RepaymentThreshold = 1,
                            AcademicYearLoanTakenOut = 2006,
                            FirstRepaymentDate = DateTimeOffset.Now
                        }
                    },
                    AnnualSalaryBeforeTax = 1
                }
            },
            new object[]
            {
                new UkStudentLoanCalculationDto
                {
                    BirthDate = DateTimeOffset.Now,
                    Loans = new List<UkStudentLoanDto>
                    {
                        new UkStudentLoanDto
                        {
                            LoanType = UkStudentLoanType.Type4,
                            BalanceRemaining = 1,
                            InterestRate = 1,
                            RepaymentThreshold = 1,
                            AcademicYearLoanTakenOut = 2006,
                            FirstRepaymentDate = DateTimeOffset.Now
                        }
                    },
                    AnnualSalaryBeforeTax = 1
                }
            },
            new object[]
            {
                new UkStudentLoanCalculationDto
                {
                    Loans = new List<UkStudentLoanDto>
                    {
                        new UkStudentLoanDto
                        {
                            LoanType = UkStudentLoanType.Type4,
                            BalanceRemaining = 1,
                            InterestRate = 1,
                            RepaymentThreshold = 1,
                            AcademicYearLoanTakenOut = 2007,
                            FirstRepaymentDate = DateTimeOffset.Now
                        }
                    },
                    AnnualSalaryBeforeTax = 1
                }
            },
            new object[]
            {
                new UkStudentLoanCalculationDto
                {
                    Loans = new List<UkStudentLoanDto>
                    {
                        new UkStudentLoanDto
                        {
                            LoanType = UkStudentLoanType.Type2,
                            BalanceRemaining = 1,
                            InterestRate = 1,
                            RepaymentThreshold = 1,
                            FirstRepaymentDate = DateTimeOffset.Now
                        }
                    },
                    AnnualSalaryBeforeTax = 1
                }
            },
            new object[]
            {
                new UkStudentLoanCalculationDto
                {
                    Loans = new List<UkStudentLoanDto>
                    {
                        new UkStudentLoanDto
                        {
                            LoanType = UkStudentLoanType.Postgraduate,
                            BalanceRemaining = 1,
                            InterestRate = 1,
                            RepaymentThreshold = 1,
                            FirstRepaymentDate = DateTimeOffset.Now
                        }
                    },
                    AnnualSalaryBeforeTax = 1
                }
            }
        };
    }

    [Fact]
    public void Validate_WithBasicInvalidData_ShouldFailValidation()
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
                },
                new UkStudentLoanDto
                {
                    LoanType = UkStudentLoanType.NotSet,
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
        result.ShouldHaveValidationErrorFor("Loans");
        result.ShouldHaveValidationErrorFor("Loans[0].BalanceRemaining");
        result.ShouldHaveValidationErrorFor("Loans[0].InterestRate");
        result.ShouldHaveValidationErrorFor("Loans[0].RepaymentThreshold");
        result.ShouldHaveValidationErrorFor("Loans[2].LoanType");
    }

    [Theory]
    [MemberData(nameof(IncorrectLoanDataSupplied))]
    public void Validate_WithALoanRequiringParticularFieldsAndDataNotCorrect_ShouldFailValidation(
        IncorrectLoanDataSuppliedData data)
    {
        // Arrange
        var validator = new UkStudentLoanCalculationDtoValidator();

        var dto = new UkStudentLoanCalculationDto
        {
            Loans = new List<UkStudentLoanDto>
            {
                new UkStudentLoanDto
                {
                    LoanType = data.LoanType,
                    BalanceRemaining = 1,
                    InterestRate = 1,
                    RepaymentThreshold = 1,
                    FirstRepaymentDate = data.FirstRepaymentDate,
                    AcademicYearLoanTakenOut = data.AcademicYearLoanTakenOut
                },
            },
            AnnualSalaryBeforeTax = 1,
            BirthDate = data.BirthDate
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(data.FieldThatShouldHaveError);
    }

    public class IncorrectLoanDataSuppliedData
    {
        public UkStudentLoanType LoanType { get; set; }

        public int? AcademicYearLoanTakenOut { get; set; }

        public DateTimeOffset? FirstRepaymentDate { get; set; }

        public DateTimeOffset? BirthDate { get; set; }

        public string FieldThatShouldHaveError { get; set; } = String.Empty;
    }

    public static IEnumerable<object[]> IncorrectLoanDataSupplied()
    {
        return new List<object[]>
        {
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type1,
                    AcademicYearLoanTakenOut = 2005,
                    FieldThatShouldHaveError = "BirthDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type4,
                    AcademicYearLoanTakenOut = 2006,
                    FieldThatShouldHaveError = "BirthDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type1,
                    FieldThatShouldHaveError = "Loans[0].AcademicYearLoanTakenOut"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type4,
                    FieldThatShouldHaveError = "Loans[0].AcademicYearLoanTakenOut"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type1,
                    AcademicYearLoanTakenOut = 2006,
                    FieldThatShouldHaveError = "Loans[0].FirstRepaymentDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    FieldThatShouldHaveError = "Loans[0].FirstRepaymentDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type2,
                    FieldThatShouldHaveError = "Loans[0].FirstRepaymentDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type4,
                    FieldThatShouldHaveError = "Loans[0].FirstRepaymentDate"
                }
            }
        };
    }
}