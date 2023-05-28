namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.FirstPossibleRepaymentDate;

public class FirstPossibleRepaymentDateOperation : IFirstPossibleRepaymentDateOperation
{
    public DateTime? Execute(FirstPossibleRepaymentDateOperationFact fact)
    {
        if (fact.LoanType == UkStudentLoanType.Type5)
        {
            return new DateTime(2026, 4, 1, 0, 0, 0);
        }

        if (!fact.StudyingPartTime)
        {
            var year = fact.CourseEndDate!.Value.Year;
            if (fact.CourseEndDate!.Value.Month >= 4)
            {
                year += 1;
            }

            return new DateTime(year, 4, 1, 0, 0, 0);
        }

        var fourYearsAfterStart = fact.CourseStartDate!.Value.AddYears(4);
        var startYear = fourYearsAfterStart.Year;
        if (fact.CourseEndDate!.Value.Month >= 4)
        {
            startYear += 1;
        }

        var aprilFourYearsAfterStart = new DateTime(startYear, 4, 1, 0, 0, 0);

        var endYear = fact.CourseEndDate!.Value.Year;
        if (fact.CourseEndDate!.Value.Month >= 4)
        {
            endYear += 1;
        }

        var aprilAfterEndOfCourse = new DateTime(endYear, 4, 1, 0, 0, 0);

        return aprilFourYearsAfterStart < aprilAfterEndOfCourse ? aprilFourYearsAfterStart : aprilAfterEndOfCourse;
    }
}