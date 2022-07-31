namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation;

using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Xunit2;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.StandardTypes;
using Moq;
using Xunit;

public class UkStudentLoanCalculatorTests
{
    [Theory, AutoMoqData]
    public void Run_WithLoans_ShouldRunUntilNoDebtRemaining(
        [Frozen]Mock<IStandardTypeCalculator> standardTypeCalculator,
        UkStudentLoanCalculatorRequest request,
        UkStudentLoanCalculator sut)
    {
        // Arrange
        standardTypeCalculator.Setup(x => x.Run(It.Is<StandardTypeCalculatorRequest>(x => x.Period == 1)))
            .Returns(new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    DebtRemaining = 1000
                },
                new()
                {
                    DebtRemaining = 0
                }
            });
        
        standardTypeCalculator.Setup(x => x.Run(It.Is<StandardTypeCalculatorRequest>(x => x.Period == 2)))
            .Returns(new List<UkStudentLoanTypeResult>
            {
                new()
                {
                    DebtRemaining = 0
                },
                new()
                {
                    DebtRemaining = 0
                }
            });
        
        // Act
        sut.Run(request);

        // Assert
        standardTypeCalculator.Verify(x => x.Run(It.IsAny<StandardTypeCalculatorRequest>()), Times.Exactly(2));
    }
}