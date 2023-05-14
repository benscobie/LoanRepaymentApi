using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.FirstPossibleRepaymentDate;
using Xunit;

namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations.FirstPossibleRepaymentDate;

public class FirstPossibleRepaymentDateOperationTests
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void Execute_WhenCalledWithFact_ShouldReturnExpectedResult(FirstPossibleRepaymentDateOperationFact fact,
        DateTimeOffset? expectedResult)
    {
        // Arrange & Act
        var result = new FirstPossibleRepaymentDateOperation().Execute(fact);

        // Assert
        result.Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> TestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                new FirstPossibleRepaymentDateOperationFact
                {
                    LoanType = UkStudentLoanType.Type5
                },
                new DateTimeOffset(2026, 4, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
            },
            new object[]
            {
                new FirstPossibleRepaymentDateOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    StudyingPartTime = false,
                    CourseEndDate = new DateTimeOffset(2026, 7, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                new DateTimeOffset(2027, 4, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
            },
            new object[]
            {
                new FirstPossibleRepaymentDateOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    StudyingPartTime = false,
                    CourseEndDate = new DateTimeOffset(2026, 2, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                new DateTimeOffset(2026, 4, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
            },
            new object[]
            {
                new FirstPossibleRepaymentDateOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    StudyingPartTime = true,
                    CourseStartDate = new DateTimeOffset(2020, 9, 1, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseEndDate = new DateTimeOffset(2025, 7, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                new DateTimeOffset(2025, 4, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
            },
            new object[]
            {
                new FirstPossibleRepaymentDateOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    StudyingPartTime = true,
                    CourseStartDate = new DateTimeOffset(2020, 9, 1, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    CourseEndDate = new DateTimeOffset(2022, 7, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                new DateTimeOffset(2023, 4, 1, 0, 0, 0, new TimeSpan(0, 0, 0))
            },
        };
    }
}