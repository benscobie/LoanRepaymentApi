﻿namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanProjection
{
    public UkStudentLoanRepaymentStatus RepaymentStatus { get; set; }

    public UkStudentLoanType LoanType { get; set; }

    public int Period { get; set; }

    public DateTime PeriodDate { get; set; }

    public decimal DebtRemaining { get; set; }

    public decimal InterestRate { get; set; }

    public decimal Paid { get; set; }

    public decimal InterestApplied { get; set; }

    public int Threshold { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal TotalInterestApplied { get; set; }
}