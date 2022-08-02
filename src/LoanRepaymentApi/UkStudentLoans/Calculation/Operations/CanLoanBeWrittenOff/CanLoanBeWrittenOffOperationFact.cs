namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;

public class CanLoanBeWrittenOffOperationFact
{
    public DateTimeOffset PeriodDate { get; set; }

    public int? AcademicYearLoanTakenOut { get; set; }

    public DateTimeOffset? FirstRepaymentDate { get; set; }

    public DateTimeOffset? BirthDate { get; set; }

    public UkStudentLoanType LoanType { get; set; }

}