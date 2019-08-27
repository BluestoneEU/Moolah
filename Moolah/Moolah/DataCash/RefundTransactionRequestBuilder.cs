using System;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    internal class RefundTransactionRequestBuilder : DataCashRequestBuilderBase, IDataCashRefundTransactionRequestBuilder
    {
        public RefundTransactionRequestBuilder(DataCashConfiguration configuration)
            : base(configuration)
        {
        }

        public XDocument Build(string originalTransactionReference, decimal amount, string captureMethod = null)
        {
            return GetDocument(
                AddCaptureMethod(TxnDetailsElement(null, amount, null, null), captureMethod),
                HistoricTxnElement(originalTransactionReference));
        }

        protected override XElement TxnDetailsElement(string merchantReference, decimal amount, string currencyCode, MCC6012 mcc6012)
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