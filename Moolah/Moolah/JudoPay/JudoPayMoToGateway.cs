using System;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    public class JudoPayMoToGateway : IPaymentGateway
    {
        private readonly JudoPayConfiguration _configuration;
        private readonly IHttpClient _httpClient;
        private readonly IJudoPayPaymentRequestBuilder _paymentRequestBuilder;
        private readonly IJudoPayResponseParser _responseParser;
        private readonly IRefundGateway _refundGateway;
        private readonly ICancelGateway _cancelGateway;

        public JudoPayMoToGateway()
            : this(MoolahConfiguration.Current.JudoPayMoTo)
        {
        }

        public JudoPayMoToGateway(JudoPayConfiguration configuration)
            : this(configuration, new HttpClient(), new JudoPayMoToRequestBuilder(configuration), new JudoPayResponseParser(), new RefundGateway(configuration), new CancelGateway(configuration))
        {
        }

        internal JudoPayMoToGateway(
            JudoPayConfiguration configuration, 
            IHttpClient httpClient, 
            IJudoPayPaymentRequestBuilder requestBuilder,
            IJudoPayResponseParser responseParser,
            IRefundGateway refundGateway,
            ICancelGateway cancelGateway)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            if (requestBuilder == null) throw new ArgumentNullException("requestBuilder");
            if (responseParser == null) throw new ArgumentNullException("responseParser");
            if (refundGateway == null) throw new ArgumentNullException("refundGateway");
            if (cancelGateway == null) throw new ArgumentNullException("cancelGateway");
            _configuration = configuration;
            _httpClient = httpClient;
            _paymentRequestBuilder = requestBuilder;
            _responseParser = responseParser;
            _refundGateway = refundGateway;
            _cancelGateway = cancelGateway;
        }

        public ICardPaymentResponse Payment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null)
        {
            var requestDocument = _paymentRequestBuilder.Build(merchantReference, amount, currencyCode, card, policy, billingAddress);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _responseParser.Parse(response);
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