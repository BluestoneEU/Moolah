using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    internal class DataCashRecurringTransactionBuilder : DataCashRequestBuilderBase, IDataCashRecurringTransactionBuilder
    {
        public DataCashRecurringTransactionBuilder(DataCashConfiguration configuration)
        : base(configuration)
        {
        }

        public XDocument BuildSetupPaymentRequest(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress, MCC6012 mcc6012, string captureMethod = "ecomm")
        {
            return GetDocument(
                AddCaptureMethod(TxnDetailsElement(merchantReference, amount, currencyCode, mcc6012, card.CardHolder, billingAddress), captureMethod),
                CardTxnElement(card, billingAddress, policy), ContAuthTxnElement(false));
        }

        public XDocument BuildRepeatPaymentRequest(string merchantReference, string caReference, decimal amount, string currencyCode, MCC6012 mcc6012, string captureMethod = "cont_auth")
        {
            //TODO
            var txnDetails = TxnDetailsElement(merchantReference, amount, currencyCode, mcc6012, null, null);
            if (!string.IsNullOrWhiteSpace(captureMethod))
                txnDetails = AddCaptureMethod(txnDetails, captureMethod);
            return GetDocument(txnDetails, HistoricTxnElement("auth", caReference), ContAuthTxnElement(true));
        }

        private XElement AddCaptureMethod(XElement txnDetails, string captureMethod)
        {
            var cm = new XElement("capturemethod", captureMethod);
            txnDetails.Add(cm);
            return txnDetails;
        }

        private XElement ContAuthTxnElement(bool historic)
        {
            var contAuthTxn = new XElement("ContAuthTxn");
            contAuthTxn.Add(new XAttribute("type", historic ? "historic" : "setup"));
            return contAuthTxn;
        }

        private XElement HistoricTxnElement(string method, string transactionReference)
        {
            var hisTxn = new XElement("HistoricTxn");
            hisTxn.Add(new XElement("method", method));
            hisTxn.Add(new XElement("reference", transactionReference));
            return hisTxn;
        }

    }
}
