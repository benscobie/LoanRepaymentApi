﻿namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanResult
{
    public int Period { get; set; }

    public DateTime PeriodDate { get; set; }

    public int Salary { get; set; }

    public List<UkStudentLoanProjection> Projections { get; set; } = new();

    public decimal AggregatedDebtRemaining => Projections.Sum(x => x.DebtRemaining);

    public decimal AggregatedPaidInPeriod => Projections.Sum(x => x.Paid);

    public decimal AggregatedInterestAppliedInPeriod => Projections.Sum(x => x.InterestApplied);

    public decimal AggregatedTotalInterestApplied => Projections.Sum(x => x.TotalInterestApplied);

    public decimal AggregatedTotalPaid => Projections.Sum(x => x.TotalPaid);
}