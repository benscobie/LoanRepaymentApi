namespace LoanRepaymentApi.UkStudentLoans;

using System;
using LoanRepaymentApi.UkStudentLoans.Calculation;

public class PersonDetails
{
    public int AnnualSalaryBeforeTax { get; set; }

    public DateTimeOffset? BirthDate { get; set; }

    public List<Adjustment> SalaryAdjustments { get; init; } = new();
}