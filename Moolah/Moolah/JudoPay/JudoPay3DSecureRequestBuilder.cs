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
        private readonly string _userAgent;
        private readonly string _acceptHeader;

        public ITimeProvider SystemTime { get; set; }

        public JudoPay3DSecureRequestBuilder(JudoPay3DSecureConfiguration configuration, string userAgent, string acceptHeader)
            : base(configuration)
        {
            _configuration = configuration;
            _userAgent = userAgent;
            _acceptHeader = acceptHeader;
            SystemTime = new TimeProvider();
        }

        public XDocument Build(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress, MCC6012 mcc6012)
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
                new XElement("mobileNumber", billingAddress.PhoneNumber),
                new XElement("emailAddress", billingAddress.Email),
                new XElement("phoneCountryCode", "44"));
        }
        
        private XElement threeDSecureElement()
        {
            return new XElement("ThreeDSecure",
                                // new XElement("verify", "yes"),
                                // new XElement("merchant_url", _configuration.MerchantUrl),
                                // new XElement("purchase_desc", _configuration.PurchaseDescription),
                                // new XElement("purchase_datetime", SystemTime.Now.ToString("yyyyMMdd HH:mm:ss")),
                                new XElement("methodNotificationUrl", _configuration.MethodNotificationUrl),
                                new XElement("challengeNotificationUrl", _configuration.ChallengeNotificationUrl)
                                // new XElement("methodNotificationUrl", "https://api.karatepay.com/order/3ds/methodNotification"),
                                // new XElement("challengeNotificationUrl", "https://api.karatepay.com/order/3ds/challengeNotification")
                                // <methodNotificationUrl>https://api.karatepay.com/order/3ds/methodNotification</methodNotificationUrl>
                                // <challengeNotificationUrl>https://api.karatepay.com/order/3ds/challengeNotification</challengeNotificationUrl>
                                // browserElement()
                                );
        }

        private XElement browserElement()
        {
            return new XElement("Browser",
                                new XElement("device_category", "0"),
                                new XElement("accept_headers", _acceptHeader),
                                new XElement("user_agent", _userAgent));
        }
    }
}