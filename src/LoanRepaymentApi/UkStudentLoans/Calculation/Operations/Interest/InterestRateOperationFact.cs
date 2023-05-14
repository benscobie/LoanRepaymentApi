namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

public class InterestRateOperationFact
{
    public int Salary { get; set; }

    public UkStudentLoanType LoanType { get; set; }

    public DateTimeOffset? CourseStartDate { get; set; }

    public DateTimeOffset? CourseEndDate { get; set; }

    public DateTimeOffset PeriodDate { get; set; }

    public bool StudyingPartTime { get; set; }
}