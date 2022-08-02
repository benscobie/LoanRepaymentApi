namespace LoanRepaymentApi.UkStudentLoans;

using System;

public class UkStudentLoan
{
    public UkStudentLoanType Type { get; set; }
    
    public DateTimeOffset? FirstRepaymentDate { get; set; }
    
    public int? AcademicYearLoanTakenOut { get; set; }
    
    public decimal BalanceRemaining { get; set; }
    
    public decimal InterestRate { get; set; }
}