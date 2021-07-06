using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    internal class JudoPayRecurringTransactionBuilder : JudoPayRequestBuilderBase, IJudoPayRecurringTransactionBuilder
    {
        public JudoPayRecurringTransactionBuilder(JudoPayConfiguration configuration)
        : base(configuration)
        {
        }

        public XDocument BuildSetupPaymentRequest(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress)
        {
            return GetDocument(
                TxnDetailsElement(merchantReference, amount, currencyCode, card.CardHolder, billingAddress),
                CardTxnElement(card, billingAddress, policy), ContAuthTxnElement(false));
        }

        public XDocument BuildRepeatPaymentRequest(string merchantReference, string caReference, decimal amount, string currencyCode)
        {
            var txnDetails = TxnDetailsElement(merchantReference, amount, currencyCode, null, null);
            return GetDocument(txnDetails, HistoricTxnElement("auth", caReference), ContAuthTxnElement(true));
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
