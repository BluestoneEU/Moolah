﻿using System;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    public interface ICancelGateway
    {
        ICancelTransactionResponse Cancel(string originalTransactionReference);
    }

    public class CancelGateway : ICancelGateway
    {
        readonly JudoPayConfiguration _configuration;
        readonly IHttpClient _httpClient;
        readonly IJudoPayCancelTransactionRequestBuilder _requestBuilder;
        readonly ICancelTransactionResponseParser _responseParser;

        public CancelGateway(JudoPayConfiguration configuration)
            : this(configuration, new HttpClient(), new CancelTransactionRequestBuilder(configuration), new CancelTransactionResponseParser())
        {
        }

        internal CancelGateway(JudoPayConfiguration configuration, IHttpClient httpClient, IJudoPayCancelTransactionRequestBuilder requestBuilder, ICancelTransactionResponseParser responseParser)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            if (responseParser == null) throw new ArgumentNullException("responseParser");
            if (requestBuilder == null) throw new ArgumentNullException("requestBuilder");
            _configuration = configuration;
            _httpClient = httpClient;
            _requestBuilder = requestBuilder;
            _responseParser = responseParser;
        }

        public ICancelTransactionResponse Cancel(string originalTransactionReference)
        {
            var requestDocument = _requestBuilder.Build(originalTransactionReference);
            var response = _httpClient.Post(_configuration.Host, requestDocument.ToString(SaveOptions.DisableFormatting));
            return _responseParser.Parse(response);
        }
    }
}