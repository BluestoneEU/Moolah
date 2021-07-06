namespace Moolah.JudoPay
{
    public enum CardFailureType : short
    {
        None,
        General,
        CardNumber,
        ExpiryDate,        
        StartDate,
        IssueNumber
    }
}