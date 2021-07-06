namespace Moolah.JudoPay
{
    public interface IDataCash3DSecureRefundGateway : IRefundGateway
    {
    }

    public class JudoPay3DSecureRefundGateway : RefundGateway, IDataCash3DSecureRefundGateway
    {
        public JudoPay3DSecureRefundGateway()
            : base(MoolahConfiguration.Current.JudoPay3DSecure)
        {
        }
    }    
}