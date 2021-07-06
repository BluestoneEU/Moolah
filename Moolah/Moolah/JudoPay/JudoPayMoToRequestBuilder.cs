using System;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    /// <summary>
    /// Builds the DataCash MoTo request XML.
    /// </summary>
    internal class JudoPayMoToRequestBuilder : JudoPayRequestBuilderBase, IJudoPayPaymentRequestBuilder
    {
        public JudoPayMoToRequestBuilder(JudoPayConfiguration configuration)
            : base(configuration)
        {
        }

        public XDocument Build(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress)
        {
            return GetDocument(
                TxnDetailsElement(merchantReference, amount, currencyCode, card.CardHolder, billingAddress),
                CardTxnElement(card, billingAddress, policy));
        }
    }
}