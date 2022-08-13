namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations.Salary;

using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;
using Xunit;

public class SalaryOperationTests
{
    [Theory, AutoMoqData]
    public void Execute_WhenCalledWithNoAdjustment_ShouldReturnCurrentSalary(SalaryOperation sut)
    {
        // Arrange
        var fact = new SalaryOperationFact
        {
            PreviousPeriodSalary = 50000,
            PeriodDate = DateTimeOffset.Now,
            SalaryAdjustments = new List<Adjustment>()
        };
        
        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().Be(50000);
    }
    
    [Theory, AutoMoqData]
    public void Execute_WhenCalledWithAnAdjustment_ShouldReturnCorrectSalaryIncrease(SalaryOperation sut)
    {
        // Arrange
        var fact = new SalaryOperationFact
        {
            PreviousPeriodSalary = 50125,
            PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
            SalaryGrowth = 0.1m,
            SalaryAdjustments = new List<Adjustment>
            {
                new()
                {
                    Date = new DateTimeOffset(2022, 03, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Value = 0.02m
                },
                new()
                {
                    Date = new DateTimeOffset(2022, 04, 17, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Value = 0.0135m
                },
                new()
                {
                    Date = new DateTimeOffset(2022, 05, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Value = 0.04m
                }
            }
        };
        
        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().Be(50802);
    }
    
    [Theory, AutoMoqData]
    public void Execute_WhenCalledWithAnAdjustmentAndIsDueAnnualGrowth_ShouldGrowByTheAdjustmentAndNotAnnualGrowth(SalaryOperation sut)
    {
        // Arrange
        var fact = new SalaryOperationFact
        {
            PreviousPeriodSalary = 50125,
            Period = 20,
            PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
            SalaryGrowth = 0.1m,
            Results = new List<UkStudentLoanResult>
            {
                new()
                {
                    Salary = 50125,
                    PeriodDate = new DateTimeOffset(2021, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Period = 8,
                }
            },
            SalaryAdjustments = new List<Adjustment>
            {
                new()
                {
                    Date = new DateTimeOffset(2022, 04, 17, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Value = 0.0135m
                },
            }
        };
        
        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().Be(50802);
    }
    
    [Theory, AutoMoqData]
    public void Execute_WhenCalledWithASalaryThatIsDueAnnualGrowthWithNoPreviousGrowth_ShouldGrowSalaryBySalaryGrowth(SalaryOperation sut)
    {
        // Arrange
        var fact = new SalaryOperationFact
        {
            PreviousPeriodSalary = 50125,
            Period = 13,
            PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
            SalaryGrowth = 0.1m,
            Results = new List<UkStudentLoanResult>
            {
                new()
                {
                    Salary = 50125,
                    PeriodDate = new DateTimeOffset(2021, 03, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Period = 12,
                }
            },
            SalaryAdjustments = new List<Adjustment>()
        };
        
        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().Be(55138);
    }
    
    [Theory, AutoMoqData]
    public void Execute_WhenCalledWithASalaryThatIsDueAnnualGrowthWithPreviousGrowth_ShouldGrowSalaryBySalaryGrowth(SalaryOperation sut)
    {
        // Arrange
        var fact = new SalaryOperationFact
        {
            PreviousPeriodSalary = 50125,
            Period = 20,
            PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
            SalaryGrowth = 0.1m,
            Results = new List<UkStudentLoanResult>
            {
                new()
                {
                    Salary = 50000,
                    PeriodDate = new DateTimeOffset(2021, 03, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Period = 7,
                },
                new()
                {
                    Salary = 50125,
                    PeriodDate = new DateTimeOffset(2021, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Period = 8,
                }
            },
            SalaryAdjustments = new List<Adjustment>()
        };
        
        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().Be(55138);
    }
    
    [Theory, AutoMoqData]
    public void Execute_WhenCalledWithASalaryThatIsNotDueGrowth_ShouldNotGrow(SalaryOperation sut)
    {
        // Arrange
        var fact = new SalaryOperationFact
        {
            PreviousPeriodSalary = 50125,
            Period = 20,
            PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
            SalaryGrowth = 0.1m,
            Results = new List<UkStudentLoanResult>
            {
                new()
                {
                    Salary = 50000,
                    PeriodDate = new DateTimeOffset(2021, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Period = 8,
                },
                new()
                {
                    Salary = 50125,
                    PeriodDate = new DateTimeOffset(2021, 05, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Period = 9,
                }
            },
            SalaryAdjustments = new List<Adjustment>()
        };
        
        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().Be(50125);
    }
}