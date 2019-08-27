using System;
using System.Web;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    /// <summary>
    /// Builds the DataCash 3D-Secure payment request XML.
    /// This is the same as MoTo, but with added sections for 3D-Secure and the Browser being used.
    /// </summary>
    internal class DataCash3DSecureRequestBuilder : DataCashRequestBuilderBase, IDataCashPaymentRequestBuilder
    {
        private readonly DataCash3DSecureConfiguration _configuration;
        private readonly string _userAgent;
        private readonly string _acceptHeader;

        public ITimeProvider SystemTime { get; set; }

        public DataCash3DSecureRequestBuilder(DataCash3DSecureConfiguration configuration, string userAgent, string acceptHeader)
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
                AddCaptureMethod(TxnDetailsElement(merchantReference, amount, currencyCode, mcc6012), "ecomm"),
                CardTxnElement(card, billingAddress, policy));
        }

        protected override XElement TxnDetailsElement(string merchantReference, decimal amount, string currencyCode, MCC6012 mcc6012)
        {
            var element = base.TxnDetailsElement(merchantReference, amount, currencyCode, mcc6012);
            element.Add(threeDSecureElement());
            return element;
        }

        private XElement threeDSecureElement()
        {
            return new XElement("ThreeDSecure",
                                new XElement("verify", "yes"),
                                new XElement("merchant_url", _configuration.MerchantUrl),
                                new XElement("purchase_desc", _configuration.PurchaseDescription),
                                new XElement("purchase_datetime", SystemTime.Now.ToString("yyyyMMdd HH:mm:ss")),
                                browserElement());
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