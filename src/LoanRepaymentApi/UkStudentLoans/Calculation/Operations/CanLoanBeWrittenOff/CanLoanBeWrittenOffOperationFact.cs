namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;

public class CanLoanBeWrittenOffOperationFact
{
    public DateTime PeriodDate { get; set; }

    public int? AcademicYearLoanTakenOut { get; set; }

    public DateTime? FirstRepaymentDate { get; set; }

    public DateTime? BirthDate { get; set; }

    public UkStudentLoanType LoanType { get; set; }

}