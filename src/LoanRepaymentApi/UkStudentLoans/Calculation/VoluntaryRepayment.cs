using System.Text.Json.Serialization;

namespace LoanRepaymentApi.UkStudentLoans.Calculation;

public class VoluntaryRepayment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UkStudentLoanType? LoanType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VoluntaryRepaymentType VoluntaryRepaymentType { get; set; }

    public DateTime Date { get; set; }

    public decimal Value { get; set; }
}