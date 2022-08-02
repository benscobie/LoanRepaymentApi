#nullable disable warnings

namespace LoanRepaymentApi.UkStudentLoans;

using System.Text.Json.Serialization;

public class UkStudentLoanDto
{
    [JsonConverter(typeof(JsonStringEnumConverter ))]
    public UkStudentLoanType LoanType { get; set; }
    
    public decimal BalanceRemaining { get; set; }

    public decimal InterestRate { get; set; }

    public DateTimeOffset? FirstRepaymentDate { get; set; }

    public int? AcademicYearLoanTakenOut { get; set; }
}