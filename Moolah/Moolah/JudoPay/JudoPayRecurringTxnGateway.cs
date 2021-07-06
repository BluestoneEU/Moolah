using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    public class JudoPayRecurringTxnGateway : IRecurringPaymentGateway
    {
        private readonly JudoPayConfiguration _configuration;
        private readonly IHttpClient _httpClient;
        private readonly IJudoPayRecurringTransactionBuilder _paymentRequestBuilder;
        private readonly IJudoPayRecurringPaymentResponseParser _responseParser;
        private readonly IRecurringRefundGateway _refundGateway;
        private readonly ICancelGateway _cancelGateway;

        public JudoPayRecurringTxnGateway()
            : this(MoolahConfiguration.Current.JudoPayMoTo)
        {
        }

        public JudoPayRecurringTxnGateway(JudoPayConfiguration configuration)
            : this(configuration, new HttpClient(), new JudoPayRecurringTransactionBuilder(configuration), new JudoPayRecurringPaymentResponseParser(new JudoPayResponseParser()), new JudoPayRecurringRefundGateway(configuration), new CancelGateway(configuration))
        {
        }

        internal JudoPayRecurringTxnGateway(
            JudoPayConfiguration configuration,
            IHttpClient httpClient,
            IJudoPayRecurringTransactionBuilder requestBuilder,
            IJudoPayRecurringPaymentResponseParser responseParser,
            IRecurringRefundGateway refundGateway,
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

        public ICancelTransactionResponse CancelTransaction(string originalTransactionReference)
        {
           return _cancelGateway.Cancel(originalTransactionReference);
        }

        public IRecurringPaymentResponse SetupPayment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null)
        {
            var requestDocument = _paymentRequestBuilder.BuildSetupPaymentRequest(merchantReference, amount, currencyCode, card, policy, billingAddress);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _responseParser.Parse(response);
        }

        public IRecurringPaymentResponse RepeatPayment(string merchantReference, string contAuthReference, decimal amount, string currencyCode = null)
        {
            var requestDocument = _paymentRequestBuilder.BuildRepeatPaymentRequest(merchantReference,contAuthReference, amount, currencyCode);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _responseParser.Parse(response);
        }

        public IRefundTransactionResponse RefundTransaction(string originalTransactionReference, decimal amount)
        {
            return _refundGateway.Refund(originalTransactionReference, amount);
        }


        public IRefundTransactionResponse RefundRepeatTransaction(string originalTransactionReference, decimal amount)
        {
            return _refundGateway.RefundRecurring(originalTransactionReference, amount);
        }
    }
}
