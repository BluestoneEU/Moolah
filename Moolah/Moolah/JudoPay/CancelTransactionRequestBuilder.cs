using System.Xml.Linq;

namespace Moolah.JudoPay
{
    internal class CancelTransactionRequestBuilder : JudoPayRequestBuilderBase, IJudoPayCancelTransactionRequestBuilder
    {
        public CancelTransactionRequestBuilder(JudoPayConfiguration configuration)
            : base(configuration)
        {
        }

        public XDocument Build(string originalTransactionReference)
        {
            return GetDocument(
                HistoricTxnElement(originalTransactionReference));
        }

        private XElement HistoricTxnElement(string originalTransactionReference)
        {
            return new XElement("HistoricTxn",
                                new XElement("reference", originalTransactionReference),
                                new XElement("method", "cancel"));
        }
    }
}