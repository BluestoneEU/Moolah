using System;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    /// <summary>
    /// Builds the DataCash MoTo request XML.
    /// </summary>
    public class DataCashMoToRequestBuilder : DataCashRequestBuilderBase, IDataCashPaymentRequestBuilder
    {
        public DataCashMoToRequestBuilder(DataCashConfiguration configuration)
            : base(configuration)
        {
        }

        public XDocument Build(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress, MCC6012 mcc6012)
        {
            return GetDocument(
                TxnDetailsElement(merchantReference, amount, currencyCode, mcc6012),
                CardTxnElement(card, billingAddress, policy));
        }
    }
}