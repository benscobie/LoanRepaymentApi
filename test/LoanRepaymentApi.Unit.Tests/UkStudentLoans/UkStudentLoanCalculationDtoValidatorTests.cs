namespace LoanRepaymentApi.Tests.UkStudentLoans;

using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.TestHelper;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
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
                    BirthDate = DateTime.Now.AddYears(-16),
                    Loans = new List<UkStudentLoanDto>
                    {
                        new()
                        {
                            LoanType = UkStudentLoanType.Type1,
                            CourseEndDate = DateTime.Now,
                            BalanceRemaining = 1,
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
                        new()
                        {
                            LoanType = UkStudentLoanType.Type1,
                            BalanceRemaining = 1,
                            AcademicYearLoanTakenOut = 2006,
                            StudyingPartTime = false,
                            CourseEndDate = DateTime.Now
                        }
                    },
                    AnnualSalaryBeforeTax = 1
                }
            },
            new object[]
            {
                new UkStudentLoanCalculationDto
                {
                    BirthDate = DateTime.Now.AddYears(-16),
                    Loans = new List<UkStudentLoanDto>
                    {
                        new()
                        {
                            LoanType = UkStudentLoanType.Type4,
                            BalanceRemaining = 1,
                            AcademicYearLoanTakenOut = 2006,
                            StudyingPartTime = false,
                            CourseEndDate = DateTime.Now
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
                        new()
                        {
                            LoanType = UkStudentLoanType.Type4,
                            BalanceRemaining = 1,
                            AcademicYearLoanTakenOut = 2007,
                            StudyingPartTime = false,
                            CourseEndDate = DateTime.Now
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
                        new()
                        {
                            LoanType = UkStudentLoanType.Type2,
                            BalanceRemaining = 1,
                            CourseStartDate = DateTime.Now,
                            CourseEndDate = DateTime.Now,
                            StudyingPartTime = false
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
                        new()
                        {
                            LoanType = UkStudentLoanType.Postgraduate,
                            BalanceRemaining = 1,
                            StudyingPartTime = false,
                            CourseEndDate = DateTime.Now,
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
                        new()
                        {
                            LoanType = UkStudentLoanType.Type5,
                            BalanceRemaining = 1,
                            StudyingPartTime = false,
                        }
                    },
                    AnnualSalaryBeforeTax = 1
                }
            },
            new object[]
            {
                new UkStudentLoanCalculationDto
                {
                    SalaryAdjustments = new List<Adjustment>
                    {
                        new()
                        {
                            Date = new DateTime(2022, 03, 01, 0, 0, 0),
                            Value = 0.02m
                        },
                        new()
                        {
                            Date = new DateTime(2022, 04, 17, 0, 0, 0),
                            Value = 0.05m
                        },
                    },
                    Loans = new List<UkStudentLoanDto>
                    {
                        new()
                        {
                            LoanType = UkStudentLoanType.Postgraduate,
                            BalanceRemaining = 1,
                            StudyingPartTime = false,
                            CourseEndDate = DateTime.Now,
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
                        new()
                        {
                            LoanType = UkStudentLoanType.Postgraduate,
                            BalanceRemaining = 1,
                            StudyingPartTime = false,
                            CourseEndDate = DateTime.Now,
                        }
                    },
                    AnnualSalaryBeforeTax = 1,
                    VoluntaryRepayments = new List<VoluntaryRepayment>
                    {
                        new()
                        {
                            LoanType = null,
                            VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                            Date = DateTime.Today
                        },
                        new()
                        {
                            LoanType = null,
                            VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                            Date = DateTime.Today
                        },
                        new()
                        {
                            LoanType = UkStudentLoanType.Postgraduate,
                            VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                            Date = DateTime.Today
                        },
                        new()
                        {
                            LoanType = UkStudentLoanType.Postgraduate,
                            VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                            Date = DateTime.Today
                        },
                        new()
                        {
                            LoanType = UkStudentLoanType.Postgraduate,
                            VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                            Date = DateTime.Today.AddMonths(1)
                        },
                        new()
                        {
                            LoanType = UkStudentLoanType.Postgraduate,
                            VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                            Date = DateTime.Today.AddMonths(1)
                        }
                    }
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
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    BalanceRemaining = 0, // Must be greater than 0
                    AcademicYearLoanTakenOut = 2005, // Set for BirthDate validation
                },
                new()
                {
                    LoanType = UkStudentLoanType.Type1, // Can't have more than 1 of the same loan type
                },
                new()
                {
                    LoanType = UkStudentLoanType.NotSet, // Not a valid selection
                },
                new()
                {
                    LoanType = UkStudentLoanType.Type2,
                    CourseStartDate = null, // Must be set for type 2
                    CourseEndDate = null, // Must be set for type 2
                }
            },
            AnnualSalaryBeforeTax = 0,
            BirthDate = DateTime.Now.AddYears(-14),
            SalaryAdjustments = new List<Adjustment>
            {
                new()
                {
                    Date = new DateTime(2050, 01, 01, 0, 0, 0),
                    Value = 0.01m
                },
                new()
                {
                    Date = new DateTime(2050, 01, 25, 0, 0, 0),
                    Value = 0.01m
                }
            },
            VoluntaryRepayments = new List<VoluntaryRepayment>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                    Date = DateTime.Today
                },
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                    Date = DateTime.Today
                }
            }
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Loans);
        result.ShouldHaveValidationErrorFor(x => x.BirthDate);
        result.ShouldHaveValidationErrorFor(x => x.AnnualSalaryBeforeTax);
        result.ShouldHaveValidationErrorFor(x => x.SalaryAdjustments);
        result.ShouldHaveValidationErrorFor(x => x.VoluntaryRepayments);
        result.ShouldHaveValidationErrorFor("Loans[0].BalanceRemaining");
        result.ShouldHaveValidationErrorFor("Loans[2].LoanType");
        result.ShouldHaveValidationErrorFor("Loans[3].CourseStartDate");
        result.ShouldHaveValidationErrorFor("Loans[3].CourseEndDate");

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
                new()
                {
                    LoanType = data.LoanType,
                    BalanceRemaining = 1,
                    StudyingPartTime = data.StudyingPartTime,
                    CourseStartDate = data.CourseStartDate,
                    CourseEndDate = data.CourseEndDate,
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

        public DateTime? CourseStartDate { get; set; }

        public DateTime? CourseEndDate { get; set; }

        public bool StudyingPartTime { get; set; }

        public DateTime? BirthDate { get; set; }

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
                    FieldThatShouldHaveError = "Loans[0].CourseEndDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    FieldThatShouldHaveError = "Loans[0].CourseEndDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type2,
                    FieldThatShouldHaveError = "Loans[0].CourseEndDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type4,
                    FieldThatShouldHaveError = "Loans[0].CourseEndDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type1,
                    StudyingPartTime = true,
                    FieldThatShouldHaveError = "Loans[0].CourseStartDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    StudyingPartTime = true,
                    FieldThatShouldHaveError = "Loans[0].CourseStartDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type2,
                    StudyingPartTime = true,
                    FieldThatShouldHaveError = "Loans[0].CourseStartDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type2,
                    StudyingPartTime = false,
                    FieldThatShouldHaveError = "Loans[0].CourseStartDate"
                }
            },
            new object[]
            {
                new IncorrectLoanDataSuppliedData
                {
                    LoanType = UkStudentLoanType.Type4,
                    StudyingPartTime = true,
                    FieldThatShouldHaveError = "Loans[0].CourseStartDate"
                }
            },
        };
    }
}