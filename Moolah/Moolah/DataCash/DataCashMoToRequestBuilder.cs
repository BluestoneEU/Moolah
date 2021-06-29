using System;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    /// <summary>
    /// Builds the DataCash MoTo request XML.
    /// </summary>
    internal class DataCashMoToRequestBuilder : DataCashRequestBuilderBase, IDataCashPaymentRequestBuilder
    {
        public DataCashMoToRequestBuilder(DataCashConfiguration configuration)
            : base(configuration)
        {
        }

        public XDocument Build(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress, MCC6012 mcc6012)
        {
            return GetDocument(
                AddCaptureMethod(TxnDetailsElement(merchantReference, amount, currencyCode, mcc6012, card.CardHolder, billingAddress), "cnp"),
                CardTxnElement(card, billingAddress, policy));
        }
    }
}