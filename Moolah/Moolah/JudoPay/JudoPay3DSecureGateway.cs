using System;
using System.IO;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    public class JudoPay3DSecureGateway : I3DSecurePaymentGateway
    {
        private readonly JudoPay3DSecureConfiguration _configuration;
        private readonly IHttpClient _httpClient;
        private readonly IJudoPayPaymentRequestBuilder _paymentPaymentRequestBuilder;
        private readonly IJudoPayAuthorizeRequestBuilder _authorizeRequestBuilder;
        private readonly IJudoPay3DSecureResponseParser _responseParser;
        private readonly IRefundGateway _refundGateway;
        private readonly ICancelGateway _cancelGateway;

        public JudoPay3DSecureGateway(JudoPay3DSecureConfiguration configuration)
            : this(configuration, new HttpClient(), new JudoPay3DSecureRequestBuilder(configuration), new JudoPay3DSecureAuthorizeRequestBuilder(configuration), new JudoPay3DSecureResponseParser(), new RefundGateway(configuration), new CancelGateway(configuration))
        {
        }

        internal JudoPay3DSecureGateway(
            JudoPay3DSecureConfiguration configuration, 
            IHttpClient httpClient, 
            IJudoPayPaymentRequestBuilder paymentRequestBuilder, 
            IJudoPayAuthorizeRequestBuilder authorizeRequestBuilder,
            IJudoPay3DSecureResponseParser responseParser,
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

        public I3DSecureResponse Payment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null)
        {
            if (string.IsNullOrWhiteSpace(merchantReference)) throw new ArgumentNullException("merchantReference");

            var requestDocument = _paymentPaymentRequestBuilder.Build(merchantReference, amount, currencyCode, card, policy, billingAddress);
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

        public I3DSecureResponse Resume3DSecure(string transactionReference, string cvv)
        {
            if (string.IsNullOrWhiteSpace(transactionReference)) throw new ArgumentNullException("transactionReference");

            var requestDocument = _authorizeRequestBuilder.BuildResume3DS(transactionReference, cvv);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _responseParser.Parse(response);
        }
        
        public I3DSecureResponse Complete3DSecure(string transactionReference, string cvv)
        {
            if (string.IsNullOrWhiteSpace(transactionReference)) throw new ArgumentNullException("transactionReference");

            var requestDocument = _authorizeRequestBuilder.BuildComplete3DS(transactionReference, cvv);
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