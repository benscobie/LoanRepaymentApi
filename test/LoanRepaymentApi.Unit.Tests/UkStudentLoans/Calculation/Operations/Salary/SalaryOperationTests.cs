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
            CurrentSalary = 50000,
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
            CurrentSalary = 50000,
            PeriodDate = new DateTimeOffset(2022, 04, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
            SalaryAdjustments = new List<Adjustment>
            {
                new Adjustment
                {
                    Date = new DateTimeOffset(2022, 03, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Value = 0.02m
                },
                new Adjustment
                {
                    Date = new DateTimeOffset(2022, 04, 17, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Value = 0.05m
                },
                new Adjustment
                {
                    Date = new DateTimeOffset(2022, 05, 06, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    Value = 0.04m
                }
            }
        };
        
        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().Be(52500);
    }
}