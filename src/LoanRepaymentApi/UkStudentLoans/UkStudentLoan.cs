namespace LoanRepaymentApi.UkStudentLoans;

using System;

public class UkStudentLoan
{
    public UkStudentLoanType Type { get; set; }

    public int? AcademicYearLoanTakenOut { get; set; }

    public decimal BalanceRemaining { get; set; }

    public bool StudyingPartTime { get; set; }

    public DateTime? CourseStartDate { get; set; }

    public DateTime? CourseEndDate { get; set; }
}