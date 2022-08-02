#nullable disable warnings

namespace LoanRepaymentApi.UkStudentLoans;

using System.Text.Json.Serialization;

public class UkStudentLoanDto
{
    [JsonConverter(typeof(JsonStringEnumConverter ))]
    public UkStudentLoanType LoanType { get; set; }

    public decimal BalanceRemaining { get; set; }

    public DateTimeOffset? FirstRepaymentDate { get; set; }

    /// <summary>
    /// Required for Type 1 and Type 4 loans.
    /// </summary>
    public int? AcademicYearLoanTakenOut { get; set; }

    /// <summary>
    /// Required for Type 2 loans.
    /// </summary>
    public bool? StudyingPartTime { get; set; }

    /// <summary>
    /// Required for Type 2 loans.
    /// </summary>
    public DateTimeOffset? CourseStartDate { get; set; }

    /// <summary>
    /// Required for Type 2 loans.
    /// </summary>
    public DateTimeOffset? CourseEndDate { get; set; }
}