namespace LoanRepaymentApi.UkStudentLoans;

using System;
using LoanRepaymentApi.UkStudentLoans.Calculation;

public class PersonDetails
{
    public int AnnualSalaryBeforeTax { get; set; }

    public DateTime? BirthDate { get; set; }

    public List<Adjustment> SalaryAdjustments { get; init; } = new();
}