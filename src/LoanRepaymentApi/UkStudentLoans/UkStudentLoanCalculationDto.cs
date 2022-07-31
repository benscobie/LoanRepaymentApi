#nullable disable warnings

namespace LoanRepaymentApi.UkStudentLoans;

public class UkStudentLoanCalculationDto
{
    public int AnnualSalaryBeforeTax { get; set; }
    
    public List<UkStudentLoanDto> Loans { get; set; }
}