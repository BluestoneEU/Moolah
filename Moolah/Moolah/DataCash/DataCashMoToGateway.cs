﻿using System;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    public class DataCashMoToGateway : IPaymentGateway
    {
        private readonly DataCashConfiguration _configuration;
        private readonly IHttpClient _httpClient;
        private readonly IDataCashPaymentRequestBuilder _paymentRequestBuilder;
        private readonly IDataCashResponseParser _responseParser;
        private readonly IRefundGateway _refundGateway;
        private readonly ICancelGateway _cancelGateway;

        public DataCashMoToGateway()
            : this(MoolahConfiguration.Current.DataCashMoTo)
        {
        }

        public DataCashMoToGateway(DataCashConfiguration configuration)
            : this(configuration, new HttpClient(), new DataCashMoToRequestBuilder(configuration), new DataCashResponseParser(), new RefundGateway(configuration), new CancelGateway(configuration))
        {
        }

        /// <summary>
        /// TODO: Make internal and visible to Moolah.Specs
        /// </summary>
        public DataCashMoToGateway(
            DataCashConfiguration configuration, 
            IHttpClient httpClient, 
            IDataCashPaymentRequestBuilder requestBuilder,
            IDataCashResponseParser responseParser,
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

        public ICardPaymentResponse Payment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null, MCC6012 mcc6012 = null)
        {
            var requestDocument = _paymentRequestBuilder.Build(merchantReference, amount, currencyCode, card, policy, billingAddress, mcc6012);
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