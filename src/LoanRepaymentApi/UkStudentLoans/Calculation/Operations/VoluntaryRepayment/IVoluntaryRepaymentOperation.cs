using LoanRepaymentApi.Operations;

namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.VoluntaryRepayment;

public interface IVoluntaryRepaymentOperation : IOperation<Dictionary<UkStudentLoanType, decimal>, VoluntaryRepaymentOperationFact>
{
}