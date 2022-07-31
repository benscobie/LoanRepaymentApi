namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Postgraduate;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using Moq;
using Xunit;

public class UkStudentLoanCalculatorTests
{
    [Theory, AutoMoqData]
    public void Run_WithLoans_ShouldRunUntilNoDebtRemaining(
        [Frozen]Mock<IStandardTypeCalculator> standardTypeCalculator,
        [Frozen]Mock<IPostgraduateCalculator> postgraduateCalculator,
        UkStudentLoanCalculatorRequest request,
        UkStudentLoanCalculator sut)
    {
        // Arrange
        standardTypeCalculator.Setup(x => x.Run(It.Is<StandardTypeCalculatorRequest>(x => x.Period == 1)))
            .Returns(new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    DebtRemaining = 1000
                },
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    DebtRemaining = 0
                }
            });
        
        standardTypeCalculator.Setup(x => x.Run(It.Is<StandardTypeCalculatorRequest>(x => x.Period == 2)))
            .Returns(new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    DebtRemaining = 0
                },
                new()
                {
                    LoanType = UkStudentLoanType.Type1,
                    DebtRemaining = 0
                }
            });
        
        postgraduateCalculator.Setup(x => x.Run(It.Is<PostgraduateCalculatorRequest>(x => x.Period == 1)))
            .Returns(new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DebtRemaining = 1000
                },
                new()
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DebtRemaining = 0
                }
            });
        
        postgraduateCalculator.Setup(x => x.Run(It.Is<PostgraduateCalculatorRequest>(x => x.Period == 2)))
            .Returns(new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DebtRemaining = 500
                },
                new()
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DebtRemaining = 0
                }
            });
        
        postgraduateCalculator.Setup(x => x.Run(It.Is<PostgraduateCalculatorRequest>(x => x.Period == 3)))
            .Returns(new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DebtRemaining = 0
                },
                new()
                {
                    LoanType = UkStudentLoanType.Postgraduate,
                    DebtRemaining = 0
                }
            });
        
        // Act
        var results = sut.Run(request);

        // Assert
        standardTypeCalculator.Verify(x => x.Run(It.IsAny<StandardTypeCalculatorRequest>()), Times.Exactly(3));
        postgraduateCalculator.Verify(x => x.Run(It.IsAny<PostgraduateCalculatorRequest>()), Times.Exactly(3));
        results.Sum(x => x.LoanResults.Count(l => l.LoanType == UkStudentLoanType.Type1)).Should().Be(4);
        results.Sum(x => x.LoanResults.Count(l => l.LoanType == UkStudentLoanType.Postgraduate)).Should().Be(6);
    }
}