using System;
using System.Web;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    /// <summary>
    /// Builds the DataCash 3D-Secure payment request XML.
    /// This is the same as MoTo, but with added sections for 3D-Secure and the Browser being used.
    /// </summary>
    internal class JudoPay3DSecureRequestBuilder : JudoPayRequestBuilderBase, IJudoPayPaymentRequestBuilder
    {
        private readonly JudoPay3DSecureConfiguration _configuration;

        public ITimeProvider SystemTime { get; set; }

        public JudoPay3DSecureRequestBuilder(JudoPay3DSecureConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            SystemTime = new TimeProvider();
        }

        public XDocument  Build(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress)
        {
            return GetDocument(
                TxnDetailsElement(merchantReference, amount, currencyCode, card.CardHolder, billingAddress),
                CardTxnElement(card, billingAddress, policy));
        }

        protected override XElement TxnDetailsElement(string merchantReference, decimal amount, string currencyCode, string cardHolder, BillingAddress billingAddress)
        {
            var element = base.TxnDetailsElement(merchantReference, amount, currencyCode, cardHolder, billingAddress);
            element.Add(new XElement("custom_data", merchantReference));
            element.Add(threeDSecureElement());
            element.Add(customerInfoElement(cardHolder, billingAddress));
            return element;
        }

        private XElement customerInfoElement(string cardHolder, BillingAddress billingAddress)
        {
            return new XElement("CustomerInfo",
                new XElement("cardHolderName", cardHolder),
                new XElement("mobileNumber", billingAddress.PhoneNumber.Replace(billingAddress.PhoneCountryCode, string.Empty)),
                new XElement("emailAddress", billingAddress.Email),
                new XElement("phoneCountryCode", billingAddress.PhoneCountryCode));
        }

        private XElement threeDSecureElement()
        {
            return new XElement("ThreeDSecure",
                                new XElement("methodNotificationUrl", _configuration.MethodNotificationUrl),
                                new XElement("challengeNotificationUrl", _configuration.ChallengeNotificationUrl)
                                );
        }
    }
}
