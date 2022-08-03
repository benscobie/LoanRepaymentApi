namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Salary;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using Moq;
using Xunit;

public class UkStudentLoanCalculatorTests
{
    [Theory, AutoMoqData]
    public void Run_WithLoans_ShouldRunUntilNoDebtRemaining(
        [Frozen] Mock<IStandardTypeCalculator> standardTypeCalculator,
        [Frozen] Mock<ISalaryOperation> salaryOperation,
        UkStudentLoanCalculatorRequest request,
        UkStudentLoanCalculator sut)
    {
        // Arrange
        request.Loans.AddRange(new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1
            },
            new()
            {
                Type = UkStudentLoanType.Type2
            },
            new()
            {
                Type = UkStudentLoanType.Postgraduate
            }
        });

        request.PersonDetails.AnnualSalaryBeforeTax = 50000;

        salaryOperation.Setup(x =>
                x.Execute(It.Is<SalaryOperationFact>(
                    s => s.CurrentSalary == request.PersonDetails.AnnualSalaryBeforeTax)))
            .Returns(52500);

        standardTypeCalculator.Setup(x =>
                x.Run(It.Is<StandardTypeCalculatorRequest>(x =>
                    x.Period == 1 && x.Loans.Any(x => x.Type != UkStudentLoanType.Postgraduate))))
            .Returns(new List<UkStudentLoanProjection>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    DebtRemaining = 1000
                },
                new()
                {
                    LoanType = UkStudentLoanType.Type2,
                    DebtRemaining = 0
                }
            });

        standardTypeCalculator.Setup(x =>
                x.Run(It.Is<StandardTypeCalculatorRequest>(x =>
                    x.Period == 2 && x.Loans.Any(x => x.Type != UkStudentLoanType.Postgraduate))))
            .Returns(new List<UkStudentLoanProjection>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    DebtRemaining = 500
                }
            });

        standardTypeCalculator.Setup(x =>
                x.Run(It.Is<StandardTypeCalculatorRequest>(x =>
                    x.Period == 3 && x.Loans.Any(x => x.Type != UkStudentLoanType.Postgraduate))))
            .Returns(new List<UkStudentLoanProjection>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    DebtRemaining = 0
                }
            });

        standardTypeCalculator.Setup(x =>
                x.Run(It.Is<StandardTypeCalculatorRequest>(x =>
                    x.Period == 1 && x.Loans.Any(x => x.Type == UkStudentLoanType.Postgraduate))))
            .Returns(new List<UkStudentLoanProjection>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DebtRemaining = 1000
                }
            });

        standardTypeCalculator.Setup(x =>
                x.Run(It.Is<StandardTypeCalculatorRequest>(x =>
                    x.Period == 2 && x.Loans.Any(x => x.Type == UkStudentLoanType.Postgraduate))))
            .Returns(new List<UkStudentLoanProjection>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DebtRemaining = 0
                }
            });

        standardTypeCalculator.Setup(x =>
                x.Run(It.Is<StandardTypeCalculatorRequest>(x =>
                    x.Period == 3 && x.Loans.Any(x => x.Type == UkStudentLoanType.Postgraduate))))
            .Returns(new List<UkStudentLoanProjection>());

        // Act
        var results = sut.Run(request);

        // Assert
        standardTypeCalculator.Verify(
            x => x.Run(It.Is<StandardTypeCalculatorRequest>(x => x.Loans.Any(x => x.Type != UkStudentLoanType.Postgraduate))),
            Times.Exactly(3));
        standardTypeCalculator.Verify(
            x => x.Run(It.Is<StandardTypeCalculatorRequest>(x =>
                x.Loans.Any(x => x.Type == UkStudentLoanType.Postgraduate))), Times.Exactly(3));
        salaryOperation.Verify(
            x => x.Execute(It.Is<SalaryOperationFact>(s =>
                s.CurrentSalary == request.PersonDetails.AnnualSalaryBeforeTax)), Times.Once);
        salaryOperation.Verify(x => x.Execute(It.Is<SalaryOperationFact>(s => s.CurrentSalary == 52500)),
            Times.AtLeastOnce);
        results.Sum(x => x.Projections.Count(l => l.LoanType == UkStudentLoanType.Type1)).Should().Be(3);
        results.Sum(x => x.Projections.Count(l => l.LoanType == UkStudentLoanType.Type2)).Should().Be(1);
        results.Sum(x => x.Projections.Count(l => l.LoanType == UkStudentLoanType.Postgraduate)).Should().Be(2);
    }
}