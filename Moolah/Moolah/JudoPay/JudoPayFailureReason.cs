namespace Moolah.JudoPay
{
    public class JudoPayFailureReason
    {
        public JudoPayFailureReason(string message, CardFailureType failureType)
        {
            Message = message;
            Type = failureType;
        }

        public string Message { get; protected set; }
        public CardFailureType Type { get; protected set; }
    }
}