using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moolah.DataCash
{
    public interface IRecurringRefundGateway : IRefundGateway
    {
        IRefundTransactionResponse Refund(string originalTransactionReference, decimal amount, string captureMethod);
    }


    public class DataCashRecurringRefundGateway : RefundGateway, IRecurringRefundGateway
    {
        public DataCashRecurringRefundGateway(DataCashConfiguration configuration):base(configuration)
        {
        }

    }
}
