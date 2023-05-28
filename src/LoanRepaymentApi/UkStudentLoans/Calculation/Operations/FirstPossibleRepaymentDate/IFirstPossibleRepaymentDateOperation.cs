using LoanRepaymentApi.Operations;

namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.FirstPossibleRepaymentDate;

public interface IFirstPossibleRepaymentDateOperation : IOperation<DateTime?, FirstPossibleRepaymentDateOperationFact>
{
}