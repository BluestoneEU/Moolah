using System;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    internal class RefundTransactionRequestBuilder : JudoPayRequestBuilderBase, IJudoPayRefundTransactionRequestBuilder
    {
        public RefundTransactionRequestBuilder(JudoPayConfiguration configuration)
            : base(configuration)
        {
        }

        public XDocument Build(string originalTransactionReference, decimal amount)
        {
            return GetDocument(
                TxnDetailsElement(null, amount, null, null, null),
                HistoricTxnElement(originalTransactionReference));
        }

        protected override XElement TxnDetailsElement(string merchantReference, decimal amount, string currencyCode, string cardHolder, BillingAddress billingAddress)
        {
            return new XElement("TxnDetails", new XElement("amount", amount.ToString("0.00")));
        }

        private XElement HistoricTxnElement(string originalTransactionReference)
        {
            return new XElement("HistoricTxn",
                                new XElement("reference", originalTransactionReference),
                                new XElement("method", "txn_refund"));
        }
    }
}