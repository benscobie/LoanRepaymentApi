namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.FirstPossibleRepaymentDate;

public class FirstPossibleRepaymentDateOperationFact
{
    public UkStudentLoanType LoanType { get; set; }

    public bool StudyingPartTime { get; set; }

    public DateTimeOffset? CourseStartDate { get; set; }

    public DateTimeOffset? CourseEndDate { get; set; }
}