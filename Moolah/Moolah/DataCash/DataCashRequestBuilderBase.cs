using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    internal interface IDataCashPaymentRequestBuilder
    {
        XDocument Build(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress, MCC6012 mcc6012);
    }

    internal interface IDataCashRecurringTransactionBuilder
    {
        XDocument BuildSetupPaymentRequest(string merchantReference, decimal amount, string currencyCode, CardDetails card, Cv2AvsPolicy policy, BillingAddress billingAddress, MCC6012 mcc6012, string captureMethod);

        XDocument BuildRepeatPaymentRequest(string merchantReference, string transactionReference, decimal amount, string currencyCode, MCC6012 mcc6012, string captureMethod = "cont_auth");
    }

    internal interface IDataCashAuthorizeRequestBuilder
    {
        XDocument Build(string transactionReference, string PARes);
    }

    internal interface IDataCashRefundTransactionRequestBuilder
    {
        XDocument Build(string originalTransactionReference, decimal amount, string captureMethod = null);
    }

    internal interface IDataCashCancelTransactionRequestBuilder
    {
        XDocument Build(string originalTransactionReference);
    }

    internal abstract class DataCashRequestBuilderBase
    {
        private readonly DataCashConfiguration _configuration;

        public DataCashRequestBuilderBase(DataCashConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            _configuration = configuration;
        }

        public XDocument GetDocument(params object[] transactionElements)
        {
            return new XDocument(new XDeclaration("1.0", "utf-8", null), requestElement(transactionElements));
        }

        private XElement requestElement(params object[] transactionElements)
        {
            return new XElement("Request",
                authenticationElement(),
                transactionElement(transactionElements));
        }

        private XElement authenticationElement()
        {
            return new XElement("Authentication",
                                new XElement("client", _configuration.MerchantId),
                                new XElement("password", _configuration.Password));
        }

        private XElement transactionElement(params object[] elements)
        {
            return new XElement("Transaction", elements);
        }

        protected virtual XElement AddCaptureMethod(XElement txnDetails, string captureMethod)
        {
            if (!string.IsNullOrWhiteSpace(captureMethod))
            {
                var cm = new XElement("capturemethod", captureMethod);
                txnDetails.Add(cm);
            }
            return txnDetails;
        }

        protected virtual XElement TxnDetailsElement(string merchantReference, decimal amount, string currencyCode, MCC6012 mcc6012)
        {
            var amountElement = new XElement("amount", amount.ToString("0.00"));
            if (!string.IsNullOrWhiteSpace(currencyCode))
                amountElement.Add(new XAttribute("currency", currencyCode));
            
            var root = new XElement("TxnDetails",
                new XElement("merchantreference", merchantReference),
                amountElement);

            if (mcc6012 != null)
                root.Add(mcc6012.ToElement());

            return root;
        }

        protected virtual XElement CardTxnElement(CardDetails card, BillingAddress billingAddress, Cv2AvsPolicy policy)
        {
            return new XElement("CardTxn",
                                new XElement("method", "auth"),
                                CardElement(card, billingAddress, policy));
        }

        protected virtual XElement CardElement(CardDetails card, BillingAddress billingAddress, Cv2AvsPolicy policy)
        {
            return new XElement("Card",
                                new XElement("pan", card.Number),
                                new XElement("expirydate", card.ExpiryDate),
                                new XElement("startdate", card.StartDate),
                                new XElement("issuenumber", card.IssueNumber),
                                Cv2AvsElement(card, billingAddress, policy));
        }

        protected virtual XElement Cv2AvsElement(CardDetails card, BillingAddress billingAddress, Cv2AvsPolicy policy)
        {
            var cv2AvsElements = new List<XElement>();
            if (billingAddress != null)
            {
                var numericAddress = numericPartsOfAddress(billingAddress);
                if (!string.IsNullOrWhiteSpace(numericAddress))
                    cv2AvsElements.Add(new XElement("street_address1", numericAddress));

                var formattedPostcode = formatPostcode(billingAddress.Postcode);
                if (!string.IsNullOrWhiteSpace(formattedPostcode))
                    cv2AvsElements.Add(new XElement("postcode", formattedPostcode));
            }

            // 0 is not a valid per-transaction policy code.
            var cvPolicy = (int)policy;
            if (cvPolicy > 0)
                cv2AvsElements.Add(new XElement("policy", cvPolicy));

            cv2AvsElements.Add(new XElement("cv2", card.Cv2));
            return new XElement("Cv2Avs", cv2AvsElements.ToArray());
        }

        /// <summary>
        /// AVS checks strip out all non-numeric characters from addresses.
        /// The DataCash specification clearly states that we can do this ourselves
        /// before sending the address to them.
        /// </summary>
        static string numericPartsOfAddress(BillingAddress billingAddress)
        {
            var regex = new Regex("[^0-9]");
            var address = string.Join("", new[]
                {
                    billingAddress.StreetAddress1,
                    billingAddress.StreetAddress2,
                    billingAddress.StreetAddress3,
                    billingAddress.StreetAddress4,
                    billingAddress.City,
                    billingAddress.State
                }.Where(x => x != null));
            return regex.Replace(address, string.Empty);
        }

        /// <summary>
        /// Postcodes sent to DataCash must be "A maximum of 9 alphanumeric characters."
        /// </summary>
        static string formatPostcode(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode)) return null;

            var regex = new Regex("[^a-zA-Z0-9]");
            postcode = regex.Replace(postcode, string.Empty);
            return postcode.Length > 9
                ? postcode.Substring(0, 9)
                : postcode;
        }
    }
}