namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.CanLoanBeWrittenOff;

public class CanLoanBeWrittenOffOperation : ICanLoanBeWrittenOffOperation
{
    public bool Execute(CanLoanBeWrittenOffOperationFact fact)
    {
        if (fact.LoanType == UkStudentLoanType.Postgraduate)
        {
            return fact.PeriodDate >= fact.FirstRepaymentDate!.Value.AddYears(30);
        }

        if (fact.LoanType == UkStudentLoanType.Type2)
        {
            return fact.PeriodDate >= fact.FirstRepaymentDate!.Value.AddYears(30);
        }

        int personAge = 0;
        if (fact.BirthDate.HasValue)
        {
            personAge = fact.PeriodDate.Year - fact.BirthDate.Value.Year;
            if (fact.BirthDate.Value.Date > fact.PeriodDate.AddYears(-personAge)) personAge--;
        }
        
        if (fact.LoanType == UkStudentLoanType.Type1)
        {
            return (fact.AcademicYearLoanTakenOut!.Value <= 2005 && personAge >= 65) ||
                    (fact.AcademicYearLoanTakenOut!.Value >= 2006 &&
                     fact.PeriodDate >=
                     fact.FirstRepaymentDate!.Value.AddYears(25));
        }

        if (fact.LoanType == UkStudentLoanType.Type4)
        {
            return (fact.AcademicYearLoanTakenOut!.Value <= 2006 && (personAge >= 65 || fact.PeriodDate >=
                        fact.FirstRepaymentDate!.Value.AddYears(30))) ||
                    (fact.AcademicYearLoanTakenOut!.Value >= 2007 &&
                     fact.PeriodDate >=
                     fact.FirstRepaymentDate!.Value.AddYears(30));
        }

        throw new InvalidOperationException();
    }
}