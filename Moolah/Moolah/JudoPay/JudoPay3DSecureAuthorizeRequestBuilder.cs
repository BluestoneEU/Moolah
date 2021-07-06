using System.Xml.Linq;

namespace Moolah.JudoPay
{
    /// <summary>
    /// Builds DataCash request XML to authorize a pending 3D-Secure payment transaction.
    /// </summary>
    internal class JudoPay3DSecureAuthorizeRequestBuilder : JudoPayRequestBuilderBase, IJudoPayAuthorizeRequestBuilder
    {
        public JudoPay3DSecureAuthorizeRequestBuilder(JudoPayConfiguration configuration)
            : base(configuration)
        {
        }

        public XDocument Build(string transactionReference, string PARes)
        {
            return GetDocument(
                HistoricTxnElement(transactionReference, PARes));
        }
        
        public XDocument BuildResume3DS(string transactionReference, string cvv)
        {
            return GetDocument(
                CardTxnElement(cvv),
                HistoricTxnElement(transactionReference));
        }
        
        public XDocument BuildComplete3DS(string transactionReference, string cvv)
        {
            return GetDocument(
                CardTxnElement(cvv),
                CompleteHistoricTxnElement(transactionReference));
        }

        private XElement HistoricTxnElement(string transactionReference, string PARes)
        {
            return new XElement("HistoricTxn",
                                new XElement("reference", transactionReference),
                                new XElement("method", "threedsecure_authorization_request"),
                                new XElement("pares_message", PARes));
        }
        
        private XElement HistoricTxnElement(string transactionReference)
        {
            return new XElement("HistoricTxn",
                new XElement("reference", transactionReference),
                new XElement("method", "resume3ds"),
                new XElement("methodCompletion", "Yes"));
        }
        
        private XElement CompleteHistoricTxnElement(string transactionReference)
        {
            return new XElement("HistoricTxn",
                new XElement("reference", transactionReference),
                new XElement("method", "complete3ds"));
        }
        
        protected virtual XElement CardTxnElement(string cvv)
        {
            return new XElement("CardTxn",
                new XElement("Card",
                    new XElement("Cv2Avs",
                        new XElement("cv2", cvv))));
        }
    }
}