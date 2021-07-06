using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moolah.JudoPay
{
    public interface IRecurringRefundGateway : IRefundGateway
    {
        IRefundTransactionResponse RefundRecurring(string originalTransactionReference, decimal amount);
    }


    public class JudoPayRecurringRefundGateway : RefundGateway, IRecurringRefundGateway
    {
        public JudoPayRecurringRefundGateway(JudoPayConfiguration configuration):base(configuration)
        {
        }

    }
}
