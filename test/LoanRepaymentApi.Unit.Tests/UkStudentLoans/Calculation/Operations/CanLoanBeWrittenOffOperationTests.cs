namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations;

using System;
using System.Collections.Generic;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations;
using Xunit;

public class CanLoanBeWrittenOffOperationTests
{
    [Theory, AutoMoqData]
    public void Execute_WhenCalledWithFactWithInvalidLoanType_ShouldThrowException(CanLoanBeWrittenOffOperation sut,
        CanLoanBeWrittenOffOperationFact fact)
    {
        // Arrange
        fact.LoanType = UkStudentLoanType.NotSet;

        // Act
        Action act = () => sut.Execute(fact);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void Execute_WhenCalledWithFact_ShouldReturnExpectedResult(CanLoanBeWrittenOffOperationFact fact,
        bool expectedResult)
    {
        // Arrange

        // Act
        var result = new CanLoanBeWrittenOffOperation().Execute(fact);

        // Assert
        result.Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> TestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                // Postgraduate loans are written off 30 years after the April you were first due to repay - Enough years
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    PeriodDate = new DateTimeOffset(2040, 04, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    FirstRepaymentDate = new DateTimeOffset(2010, 04, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                true
            },
            new object[]
            {
                // Postgraduate loans are written off 30 years after the April you were first due to repay - Not enough years
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    PeriodDate = new DateTimeOffset(2040, 03, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    FirstRepaymentDate = new DateTimeOffset(2010, 04, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                false
            },
            new object[]
            {
                // Plan 2 loans are written off 30 years after the April you were first due to repay - Enough years
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    PeriodDate = new DateTimeOffset(2040, 04, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    FirstRepaymentDate = new DateTimeOffset(2010, 04, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                true
            },
            new object[]
            {
                // Plan 2 loans are written off 30 years after the April you were first due to repay - Not enough years
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type2,
                    PeriodDate = new DateTimeOffset(2040, 03, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    FirstRepaymentDate = new DateTimeOffset(2010, 04, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                false
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2005-2006 or earlier, paid off when 65 - Old enough
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    AcademicYearLoanTakenOut = 2005,
                    BirthDate = new DateTimeOffset(1990, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2055, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    FirstRepaymentDate = new DateTimeOffset(2050, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                true
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2005-2006 or earlier, paid off when 65 - Not old enough
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    AcademicYearLoanTakenOut = 2005,
                    BirthDate = new DateTimeOffset(1990, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2054, 12, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    FirstRepaymentDate = new DateTimeOffset(2050, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                },
                false
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2006-2007 or later, paid off 25 years after the April you were first due to repay - Enough years
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    AcademicYearLoanTakenOut = 2006,
                    FirstRepaymentDate = new DateTimeOffset(2010, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    BirthDate = new DateTimeOffset(1995, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2035, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                true
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2006-2007 or later, paid off 25 years after the April you were first due to repay - Not enough years
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type1,
                    AcademicYearLoanTakenOut = 2006,
                    FirstRepaymentDate = new DateTimeOffset(2010, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    BirthDate = new DateTimeOffset(1995, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2034, 12, 01, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                false
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2007-2008 or later, paid off 30 years after the April you were first due to repay - Enough years
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    AcademicYearLoanTakenOut = 2007,
                    FirstRepaymentDate = new DateTimeOffset(2010, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    BirthDate = new DateTimeOffset(1995, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2040, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                true
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2007-2008 or later, paid off 30 years after the April you were first due to repay - Not enough years
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    AcademicYearLoanTakenOut = 2007,
                    FirstRepaymentDate = new DateTimeOffset(2010, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    BirthDate = new DateTimeOffset(1995, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2039, 12, 01, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                false
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2006-2007 or later, when you’re 65, or 30 years after the April you were first due to repay - whichever comes first - Old enough
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    AcademicYearLoanTakenOut = 2006,
                    FirstRepaymentDate = new DateTimeOffset(2059, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    BirthDate = new DateTimeOffset(1995, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2060, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                true
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2006-2007 or later, when you’re 65, or 30 years after the April you were first due to repay - whichever comes first - Not old enough
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    AcademicYearLoanTakenOut = 2006,
                    FirstRepaymentDate = new DateTimeOffset(2059, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    BirthDate = new DateTimeOffset(1995, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2059, 12, 01, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                false
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2006-2007 or later, when you’re 65, or 30 years after the April you were first due to repay - whichever comes first - Enough years past
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    AcademicYearLoanTakenOut = 2006,
                    FirstRepaymentDate = new DateTimeOffset(2010, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    BirthDate = new DateTimeOffset(1995, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2040, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                true
            },
            new object[]
            {
                // AcademicYearLoanTakenOut 2006-2007 or later, when you’re 65, or 30 years after the April you were first due to repay - whichever comes first - Not enough years past
                new CanLoanBeWrittenOffOperationFact
                {
                    LoanType = UkStudentLoanType.Type4,
                    AcademicYearLoanTakenOut = 2006,
                    FirstRepaymentDate = new DateTimeOffset(2010, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    BirthDate = new DateTimeOffset(1995, 01, 01, 0, 0, 0, new TimeSpan(0, 0, 0)),
                    PeriodDate = new DateTimeOffset(2039, 12, 01, 0, 0, 0, new TimeSpan(0, 0, 0))
                },
                false
            },
        };
    }
}