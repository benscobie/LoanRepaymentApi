namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.Interest;

public class InterestRateOperationFact
{
    public int Salary { get; set; }

    public UkStudentLoanType LoanType { get; set; }

    public DateTime? CourseStartDate { get; set; }

    public DateTime? CourseEndDate { get; set; }

    public DateTime PeriodDate { get; set; }

    public bool StudyingPartTime { get; set; }
}