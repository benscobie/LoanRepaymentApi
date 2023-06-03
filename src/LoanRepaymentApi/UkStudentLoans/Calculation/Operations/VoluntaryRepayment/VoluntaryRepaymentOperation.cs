namespace LoanRepaymentApi.UkStudentLoans.Calculation.Operations.VoluntaryRepayment;

public class VoluntaryRepaymentOperation : IVoluntaryRepaymentOperation
{
    public Dictionary<UkStudentLoanType, decimal> Execute(VoluntaryRepaymentOperationFact fact)
    {
        var repaymentsMade = new Dictionary<UkStudentLoanType, decimal>();
        var sortedLoans = fact.Loans.OrderByDescending(x => fact.LoanInterestRates[x.Type]);
        var oneOffRepayments =
            fact.VoluntaryRepayments.Where(x =>
                x.VoluntaryRepaymentType == VoluntaryRepaymentType.OneOff && x.Date.Year == fact.PeriodDate.Year &&
                x.Date.Month == fact.PeriodDate.Month).ToList();
        var endOfMonth = new DateTime(fact.PeriodDate.Year, fact.PeriodDate.Month,
            DateTime.DaysInMonth(fact.PeriodDate.Year, fact.PeriodDate.Month), 23, 59, 59);
        var repeatingRepayments = fact.VoluntaryRepayments
            .Where(x => x.VoluntaryRepaymentType == VoluntaryRepaymentType.Repeating && x.Date <= endOfMonth)
            .OrderByDescending(x => x.Date).ToList();

        foreach (var loan in fact.Loans)
        {
            var balance = fact.LoanBalancesRemaining[loan.Type];
            if (balance == 0) continue;

            var repeatingRepayment = repeatingRepayments.FirstOrDefault(x => x.LoanType == loan.Type);
            var oneOffRepayment = oneOffRepayments.SingleOrDefault(x => x.LoanType == loan.Type);
            var amount = Math.Min(balance, (repeatingRepayment?.Value ?? 0) + (oneOffRepayment?.Value ?? 0));

            if (amount > 0)
            {
                repaymentsMade[loan.Type] = amount;
            }
        }

        var wholeBalanceVoluntaryRepaymentAmount =
            (oneOffRepayments.SingleOrDefault(x => x.LoanType == null)?.Value ?? 0) +
            (repeatingRepayments.FirstOrDefault(x => x.LoanType == null)?.Value ?? 0);

        if (wholeBalanceVoluntaryRepaymentAmount <= 0) return repaymentsMade;

        foreach (var loan in sortedLoans)
        {
            var balance = fact.LoanBalancesRemaining[loan.Type];
            if (repaymentsMade.TryGetValue(loan.Type, out var repaymentMade))
            {
                balance -= repaymentMade;
            }

            if (balance == 0) continue;

            var amountToPay = Math.Min(balance, wholeBalanceVoluntaryRepaymentAmount);
            wholeBalanceVoluntaryRepaymentAmount -= amountToPay;

            if (repaymentsMade.ContainsKey(loan.Type))
            {
                repaymentsMade[loan.Type] += amountToPay;
            }
            else
            {
                repaymentsMade[loan.Type] = amountToPay;
            }

            if (wholeBalanceVoluntaryRepaymentAmount == 0)
            {
                break;
            }
        }

        return repaymentsMade;
    }
}