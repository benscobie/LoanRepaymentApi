using System;
using System.Collections.Generic;
using AwesomeAssertions;
using LoanRepaymentApi.UkStudentLoans;
using LoanRepaymentApi.UkStudentLoans.Calculation;
using LoanRepaymentApi.UkStudentLoans.Calculation.Operations.VoluntaryRepayment;
using Xunit;

namespace LoanRepaymentApi.Tests.UkStudentLoans.Calculation.Operations.VoluntaryRepayment;

public class VoluntaryRepaymentOperationTests
{
    [Theory, AutoMoqData]
    public void Execute_WhenNoVoluntaryRepayments_ShouldReturnEmpty(VoluntaryRepaymentOperation sut,
        VoluntaryRepaymentOperationFact fact)
    {
        // Arrange
        fact.Loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1
            }
        };
        fact.LoanBalancesRemaining = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 500
            }
        };
        fact.VoluntaryRepayments = new List<LoanRepaymentApi.UkStudentLoans.Calculation.VoluntaryRepayment>();

        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData(10, 10)]
    [InlineAutoMoqData(1000, 500)]
    public void Execute_WhenOneOffType1VoluntaryRepayment_ShouldReturnCorrectAmount(decimal amount, decimal expected,
        VoluntaryRepaymentOperation sut,
        VoluntaryRepaymentOperationFact fact)
    {
        // Arrange
        fact.Loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1
            },
            new()
            {
                Type = UkStudentLoanType.Type2
            }
        };
        fact.LoanBalancesRemaining = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 500
            },
            {
                UkStudentLoanType.Type2, 1000
            }
        };
        fact.PeriodDate = DateTime.Now;
        fact.VoluntaryRepayments = new List<LoanRepaymentApi.UkStudentLoans.Calculation.VoluntaryRepayment>
        {
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                Date = DateTime.Now,
                VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                Value = amount
            }
        };

        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().ContainSingle(x => x.Key == UkStudentLoanType.Type1 && x.Value == expected);
    }

    [Theory, AutoMoqData]
    public void Execute_WhenOneOffAndRepeatingType1VoluntaryRepayment_ShouldReturnCorrectAmount(
        VoluntaryRepaymentOperation sut,
        VoluntaryRepaymentOperationFact fact)
    {
        // Arrange
        fact.Loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1
            },
            new()
            {
                Type = UkStudentLoanType.Type2
            }
        };
        fact.LoanBalancesRemaining = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 500
            },
            {
                UkStudentLoanType.Type2, 1000
            }
        };
        fact.PeriodDate = DateTime.Now;
        fact.VoluntaryRepayments = new List<LoanRepaymentApi.UkStudentLoans.Calculation.VoluntaryRepayment>
        {
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                Date = DateTime.Now,
                VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                Value = 10
            },
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                Date = DateTime.Now.AddMonths(-7),
                VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                Value = 25
            },
            new()
            {
                LoanType = UkStudentLoanType.Type1,
                Date = DateTime.Now.AddMonths(-3),
                VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                Value = 15
            }
        };

        // Act
        var result = sut.Execute(fact);

        // Assert
        result.Should().ContainSingle(x => x.Key == UkStudentLoanType.Type1 && x.Value == 25);
    }

    [Theory, AutoMoqData]
    public void Execute_WhenOneOffWholeBalanceVoluntaryRepayment_ShouldReturnCorrectAmount(
        VoluntaryRepaymentOperation sut,
        VoluntaryRepaymentOperationFact fact)
    {
        // Arrange
        fact.Loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1
            },
            new()
            {
                Type = UkStudentLoanType.Type2
            },
        };
        fact.LoanBalancesRemaining = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 500
            },
            {
                UkStudentLoanType.Type2, 200
            }
        };
        fact.PeriodDate = DateTime.Now;
        fact.VoluntaryRepayments = new List<LoanRepaymentApi.UkStudentLoans.Calculation.VoluntaryRepayment>
        {
            new()
            {
                LoanType = null,
                Date = DateTime.Now,
                VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                Value = 300
            }
        };
        fact.LoanInterestRates = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 0.1m
            },
            {
                UkStudentLoanType.Type2, 0.2m
            }
        };

        // Act
        var result = sut.Execute(fact);

        var expectedResult = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 100
            },
            {
                UkStudentLoanType.Type2, 200
            }
        };

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory, AutoMoqData]
    public void Execute_WhenOneOffWholeBalanceVoluntaryRepayment_ShouldReturnCorrectAmountAndNotOverpay(
        VoluntaryRepaymentOperation sut,
        VoluntaryRepaymentOperationFact fact)
    {
        // Arrange
        fact.Loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1
            },
            new()
            {
                Type = UkStudentLoanType.Type2
            },
        };
        fact.LoanBalancesRemaining = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 500
            },
            {
                UkStudentLoanType.Type2, 200
            }
        };
        fact.PeriodDate = DateTime.Now;
        fact.VoluntaryRepayments = new List<LoanRepaymentApi.UkStudentLoans.Calculation.VoluntaryRepayment>
        {
            new()
            {
                LoanType = null,
                Date = DateTime.Now,
                VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                Value = 1000
            }
        };
        fact.LoanInterestRates = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 0.1m
            },
            {
                UkStudentLoanType.Type2, 0.2m
            }
        };

        // Act
        var result = sut.Execute(fact);

        var expectedResult = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 500
            },
            {
                UkStudentLoanType.Type2, 200
            }
        };

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory, AutoMoqData]
    public void Execute_WhenOneOffAndRepeatingWholeBalanceVoluntaryRepayment_ShouldReturnCorrectAmount(
        VoluntaryRepaymentOperation sut,
        VoluntaryRepaymentOperationFact fact)
    {
        // Arrange
        fact.Loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1
            },
            new()
            {
                Type = UkStudentLoanType.Type2
            },
        };
        fact.LoanBalancesRemaining = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 500
            },
            {
                UkStudentLoanType.Type2, 200
            }
        };
        fact.PeriodDate = DateTime.Now;
        fact.VoluntaryRepayments = new List<LoanRepaymentApi.UkStudentLoans.Calculation.VoluntaryRepayment>
        {
            new()
            {
                LoanType = null,
                Date = DateTime.Now,
                VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                Value = 300
            },
            new()
            {
                LoanType = null,
                Date = DateTime.Now.AddMonths(-7),
                VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                Value = 25
            },
            new()
            {
                LoanType = null,
                Date = DateTime.Now.AddMonths(-3),
                VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                Value = 15
            }
        };
        fact.LoanInterestRates = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 0.1m
            },
            {
                UkStudentLoanType.Type2, 0.2m
            }
        };

        // Act
        var result = sut.Execute(fact);

        var expectedResult = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 115
            },
            {
                UkStudentLoanType.Type2, 200
            }
        };

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory, AutoMoqData]
    public void Execute_WhenOneOffAndRepeatingType1AndWholeBalanceVoluntaryRepayment_ShouldReturnCorrectAmount(
        VoluntaryRepaymentOperation sut,
        VoluntaryRepaymentOperationFact fact)
    {
        // Arrange
        fact.Loans = new List<UkStudentLoan>
        {
            new()
            {
                Type = UkStudentLoanType.Type1
            },
            new()
            {
                Type = UkStudentLoanType.Type2
            },
        };
        fact.LoanBalancesRemaining = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 500
            },
            {
                UkStudentLoanType.Type2, 200
            }
        };
        fact.PeriodDate = DateTime.Now;
        fact.VoluntaryRepayments = new List<LoanRepaymentApi.UkStudentLoans.Calculation.VoluntaryRepayment>
        {
            new()
            {
                LoanType = null,
                Date = DateTime.Now,
                VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                Value = 300
            },
            new()
            {
                LoanType = UkStudentLoanType.Type2,
                Date = DateTime.Now,
                VoluntaryRepaymentType = VoluntaryRepaymentType.OneOff,
                Value = 25
            },
            new()
            {
                LoanType = null,
                Date = DateTime.Now.AddMonths(-7),
                VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                Value = 25
            },
            new()
            {
                LoanType = null,
                Date = DateTime.Now.AddMonths(-3),
                VoluntaryRepaymentType = VoluntaryRepaymentType.Repeating,
                Value = 15
            }
        };
        fact.LoanInterestRates = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 0.1m
            },
            {
                UkStudentLoanType.Type2, 0.2m
            }
        };

        // Act
        var result = sut.Execute(fact);

        var expectedResult = new Dictionary<UkStudentLoanType, decimal>
        {
            {
                UkStudentLoanType.Type1, 140
            },
            {
                UkStudentLoanType.Type2, 200
            }
        };

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}