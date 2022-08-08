namespace LoanRepaymentApi.UkStudentLoans;

using AutoMapper;
using LoanRepaymentApi.UkStudentLoans.Calculation;

public class UkStudentLoanMapping : Profile
{
    public UkStudentLoanMapping()
    {
        CreateMap<UkStudentLoanResult, UkStudentLoanResultDto>();
        CreateMap<UkStudentLoanProjection, UkStudentLoanProjectionDto>();
    }
}