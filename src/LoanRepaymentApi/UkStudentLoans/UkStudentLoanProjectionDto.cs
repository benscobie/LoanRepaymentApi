namespace LoanRepaymentApi.UkStudentLoans;

using System.Text.Json.Serialization;

public class UkStudentLoanProjectionDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UkStudentLoanRepaymentStatus RepaymentStatus { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UkStudentLoanType LoanType { get; set; }

    public decimal DebtRemaining { get; set; }

    public decimal InterestRate { get; set; }

    public decimal Paid { get; set; }

    public decimal InterestApplied { get; set; }

    public int Threshold { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal TotalInterestApplied { get; set; }
}