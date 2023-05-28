#nullable disable warnings

namespace LoanRepaymentApi.UkStudentLoans;

using System.Text.Json.Serialization;

public class UkStudentLoanDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UkStudentLoanType LoanType { get; set; }

    public decimal BalanceRemaining { get; set; }

    /// <summary>
    /// Required for Type 1 and Type 4 loans.
    /// </summary>
    public int? AcademicYearLoanTakenOut { get; set; }

    public bool StudyingPartTime { get; set; }

    /// <summary>
    /// Required for Type 2 loans or when studying part times.
    /// </summary>
    public DateTime? CourseStartDate { get; set; }

    /// <summary>
    /// Required for Non-Type 5 loans.
    /// </summary>
    public DateTime? CourseEndDate { get; set; }
}