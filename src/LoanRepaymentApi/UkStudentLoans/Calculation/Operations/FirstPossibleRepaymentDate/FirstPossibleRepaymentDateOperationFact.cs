namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.FirstPossibleRepaymentDate;

public class FirstPossibleRepaymentDateOperationFact
{
    public UkStudentLoanType LoanType { get; set; }

    public bool StudyingPartTime { get; set; }

    public DateTime? CourseStartDate { get; set; }

    public DateTime? CourseEndDate { get; set; }
}