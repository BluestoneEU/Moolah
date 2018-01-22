using System;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    public class DataCash3DSecureGateway : I3DSecurePaymentGateway
    {
        private readonly DataCash3DSecureConfiguration _configuration;
        private readonly IHttpClient _httpClient;
        private readonly IDataCashPaymentRequestBuilder _paymentPaymentRequestBuilder;
        private readonly IDataCashAuthorizeRequestBuilder _authorizeRequestBuilder;
        private readonly IDataCash3DSecureResponseParser _responseParser;
        private readonly IRefundGateway _refundGateway;
        private readonly ICancelGateway _cancelGateway;

        public DataCash3DSecureGateway(DataCash3DSecureConfiguration configuration, string userAgent, string userAcceptHeader)
            : this(configuration, new HttpClient(), new DataCash3DSecureRequestBuilder(configuration, userAgent, userAcceptHeader), new DataCash3DSecureAuthorizeRequestBuilder(configuration), new DataCash3DSecureResponseParser(), new RefundGateway(configuration), new CancelGateway(configuration))
        {
        }

        /// <summary>
        /// TODO: Make internal and visible to Moolah.Specs
        /// </summary>
        public DataCash3DSecureGateway(
            DataCash3DSecureConfiguration configuration, 
            IHttpClient httpClient, 
            IDataCashPaymentRequestBuilder paymentRequestBuilder, 
            IDataCashAuthorizeRequestBuilder authorizeRequestBuilder,
            IDataCash3DSecureResponseParser responseParser,
            IRefundGateway refundGateway,
            ICancelGateway cancelGateway)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            if (paymentRequestBuilder == null) throw new ArgumentNullException("paymentRequestBuilder");
            if (authorizeRequestBuilder == null) throw new ArgumentNullException("authorizeRequestBuilder");
            if (responseParser == null) throw new ArgumentNullException("responseParser");
            if (refundGateway == null) throw new ArgumentNullException("refundGateway");
            if (cancelGateway == null) throw new ArgumentNullException("cancelGateway");
            _configuration = configuration;
            _httpClient = httpClient;
            _paymentPaymentRequestBuilder = paymentRequestBuilder;
            _authorizeRequestBuilder = authorizeRequestBuilder;
            _responseParser = responseParser;
            _refundGateway = refundGateway;
            _cancelGateway = cancelGateway;
        }

        public I3DSecureResponse Payment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null, MCC6012 mcc6012 = null)
        {
            if (string.IsNullOrWhiteSpace(merchantReference)) throw new ArgumentNullException("merchantReference");

            var requestDocument = _paymentPaymentRequestBuilder.Build(merchantReference, amount, currencyCode, card, policy, billingAddress, mcc6012);
            var requestData = requestDocument.ToString(SaveOptions.None);
            var httpResponse = _httpClient.Post(_configuration.Host, requestData);
            var response = _responseParser.Parse(httpResponse);
            if (response.CanAuthorize())
                response = Authorise(response.TransactionReference, null);
            return response;
        }

        public I3DSecureResponse Authorise(string transactionReference, string PARes)
        {
            if (string.IsNullOrWhiteSpace(transactionReference)) throw new ArgumentNullException("transactionReference");

            var requestDocument = _authorizeRequestBuilder.Build(transactionReference, PARes);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _responseParser.Parse(response);
        }

        /// <summary>
        /// Merchant vTID number
        /// </summary>
        public string MerchantId
        {
            get { return _configuration.MerchantId; }
        }

        public IRefundTransactionResponse RefundTransaction(string originalTransactionReference, decimal amount)
        {
            return _refundGateway.Refund(originalTransactionReference, amount);
        }

        public ICancelTransactionResponse CancelTransaction(string originalTransactionReference)
        {
            return _cancelGateway.Cancel(originalTransactionReference);
        }
    }
}