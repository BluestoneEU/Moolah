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

        // protected virtual XElement CardElement()
        // {
        //     return new XElement("Card",
        //         Cv2AvsElement());
        // }
        //
        // protected virtual XElement Cv2AvsElement()
        // {
        //     if (billingAddress != null)
        //     {
        //         var numericAddress = numericPartsOfAddress(billingAddress);
        //         if (!string.IsNullOrWhiteSpace(numericAddress))
        //             cv2AvsElements.Add(new XElement("street_address1", numericAddress));
        //
        //         var formattedPostcode = formatPostcode(billingAddress.Postcode);
        //         if (!string.IsNullOrWhiteSpace(formattedPostcode))
        //             cv2AvsElements.Add(new XElement("postcode", formattedPostcode));
        //     }
        //
        //     // 0 is not a valid per-transaction policy code.
        //     var cvPolicy = (int)policy;
        //     if (cvPolicy > 0)
        //         cv2AvsElements.Add(new XElement("policy", cvPolicy));
        //
        //     cv2AvsElements.Add(new XElement("cv2", card.Cv2));
        //     return new XElement("Cv2Avs", "452");
        // }
    }
}