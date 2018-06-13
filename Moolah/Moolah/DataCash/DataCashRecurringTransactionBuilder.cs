using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    public class DataCashRecurringTransactionBuilder : DataCashRequestBuilderBase, IDataCashRecurringTransactionBuilder
    {
        public DataCashRecurringTransactionBuilder(DataCashConfiguration configuration)
        : base(configuration)
        {
        }

        public XDocument BuildSetup(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress, MCC6012 mcc6012, string captureMethod = "ecomm")
        {
            return GetDocument(
                         AddCaptureMethod(TxnDetailsElement(merchantReference, amount, currencyCode, mcc6012), captureMethod),
                         CardTxnElement(card, billingAddress, policy), ContAuthTxnElement(false));
        }

        public XDocument BuildRepeat(string merchantReference, string transactionReference, decimal amount, string currencyCode, MCC6012 mcc6012, string captureMethod = null)
        {
            var txnDetails = TxnDetailsElement(merchantReference, amount, currencyCode, mcc6012);
            if (!string.IsNullOrWhiteSpace(captureMethod))
                txnDetails = AddCaptureMethod(txnDetails, captureMethod);
            return GetDocument(txnDetails, HistoricTxnElement("auth", transactionReference), ContAuthTxnElement(true));
        }

        private XElement AddCaptureMethod(XElement txnDetails, string captureMethod)
        {
            var cm = new XElement("capturemethod", captureMethod);
            txnDetails.Add(cm);
            return txnDetails;
        }

        private XElement ContAuthTxnElement(bool historic)
        {
            return new XElement("ContAuthTxn", historic ? "historic" : "setup");
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
