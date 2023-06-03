namespace LoanRepaymentApi.UkStudentLoans.Calculation;

using System.Collections.Generic;

public class UkStudentLoanCalculatorRequest
{
    public UkStudentLoanCalculatorRequest(PersonDetails personDetails, List<UkStudentLoan> loans)
    {
        PersonDetails = personDetails;
        Loans = loans;
    }

    public PersonDetails PersonDetails { get; }

    public decimal SalaryGrowth { get; set; }

    public decimal AnnualEarningsGrowth { get; set; }

    public List<UkStudentLoan> Loans { get; }

    public List<VoluntaryRepayment> VoluntaryRepayments { get; set; } = new();
}