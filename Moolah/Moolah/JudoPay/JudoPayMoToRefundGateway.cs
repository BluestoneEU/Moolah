namespace Moolah.JudoPay
{
    public interface IJudoPayMoToRefundGateway : IRefundGateway
    {
    }

    public class JudoPayMoToRefundGateway : RefundGateway, IJudoPayMoToRefundGateway
    {
        public JudoPayMoToRefundGateway()
            : base(MoolahConfiguration.Current.JudoPayMoTo)
        {
        }
    }    
}