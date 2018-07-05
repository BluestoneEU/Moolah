using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    public class DataCashRecurringTxnGateway : IRecurringPaymentGateway
    {
        private readonly DataCashConfiguration _configuration;
        private readonly IHttpClient _httpClient;
        private readonly IDataCashRecurringTransactionBuilder _paymentRequestBuilder;
        private readonly IDataCashRecurringPaymentResponseParser _responseParser;
        private readonly IRecurringRefundGateway _refundGateway;
        private readonly ICancelGateway _cancelGateway;

        public DataCashRecurringTxnGateway()
     : this(MoolahConfiguration.Current.DataCashMoTo)
        {
        }

        public DataCashRecurringTxnGateway(DataCashConfiguration configuration)
            : this(configuration, new HttpClient(), new DataCashRecurringTransactionBuilder(configuration), new DataCashRecurringPaymentResponseParser(new DataCashResponseParser()), new DataCashRecurringRefundGateway(configuration), new CancelGateway(configuration))
        {
        }

        /// <summary>
        /// TODO: Make internal and visible to Moolah.Specs
        /// </summary>
        public DataCashRecurringTxnGateway(
            DataCashConfiguration configuration,
            IHttpClient httpClient,
            IDataCashRecurringTransactionBuilder requestBuilder,
            IDataCashRecurringPaymentResponseParser responseParser,
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

        public IRecurringPaymentResponse SetupPayment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null, MCC6012 mcc6012 = null, string captureMethod = "ecomm")
        {
            var requestDocument = _paymentRequestBuilder.BuildSetupPaymentRequest(merchantReference, amount, currencyCode, card, policy, billingAddress, mcc6012, captureMethod);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _responseParser.Parse(response);
        }

        public IRecurringPaymentResponse RepeatPayment(string merchantReference, string contAuthReference, decimal amount, string currencyCode = null, MCC6012 mcc6012 = null, string captureMethod = "cont_auth")
        {
            var requestDocument = _paymentRequestBuilder.BuildRepeatPaymentRequest(merchantReference,contAuthReference, amount, currencyCode, mcc6012, captureMethod);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _responseParser.Parse(response);
        }

        public IRefundTransactionResponse RefundTransaction(string originalTransactionReference, decimal amount)
        {
            return _refundGateway.Refund(originalTransactionReference, amount, null);
        }

        /// <summary>
        /// Refunds a repeat transaction
        /// </summary>
        /// <param name="originalTransactionReference">The original transaction reference</param>
        /// <param name="amount">The amount to refund. Must be less than or equal to the original amount</param>
        /// <param name="refund">When refunding a CA transaction (ie. a repeat payment), you MUST specify a capture method,
        /// as the original capture method for the transaction "cont_auth" is non-refundable.</param>
        /// <returns></returns>
        public IRefundTransactionResponse RefundRepeatTransaction(string originalTransactionReference, decimal amount, string captureMethod = "ecomm")
        {
            return _refundGateway.Refund(originalTransactionReference, amount, captureMethod);
        }
    }
}
