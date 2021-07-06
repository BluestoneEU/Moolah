using System;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    public interface IRefundGateway
    {
        IRefundTransactionResponse Refund(string originalTransactionReference, decimal amount);
    }

    public class RefundGateway : IRefundGateway
    {
        readonly JudoPayConfiguration _configuration;
        readonly IHttpClient _httpClient;
        readonly IJudoPayRefundTransactionRequestBuilder _refundRequestBuilder;
        readonly IRefundTransactionResponseParser _refundResponseParser;

        public RefundGateway(JudoPayConfiguration configuration)
            : this(configuration, new HttpClient(), new RefundTransactionRequestBuilder(configuration), new RefundTransactionResponseParser())
        {
        }

        internal RefundGateway(JudoPayConfiguration configuration, IHttpClient httpClient, IJudoPayRefundTransactionRequestBuilder refundRequestBuilder, IRefundTransactionResponseParser refundResponseParser)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            if (refundResponseParser == null) throw new ArgumentNullException("refundResponseParser");
            if (refundRequestBuilder == null) throw new ArgumentNullException("refundResponseParser");
            _configuration = configuration;
            _httpClient = httpClient;
            _refundRequestBuilder = refundRequestBuilder;
            _refundResponseParser = refundResponseParser;
        }

        public IRefundTransactionResponse Refund(string originalTransactionReference, decimal amount)
        {
            return RefundSingleOrRecurring(originalTransactionReference, amount, null);
        }

        public IRefundTransactionResponse RefundRecurring(string originalTransactionReference, decimal amount, string captureMethod = null)
        {
            return RefundSingleOrRecurring(originalTransactionReference, amount, captureMethod);
        }

        private IRefundTransactionResponse RefundSingleOrRecurring(string originalTransactionReference, decimal amount, string captureMethod)
        {
            var requestDocument = _refundRequestBuilder.Build(originalTransactionReference, amount, captureMethod);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _refundResponseParser.Parse(response);
        }
    }
}