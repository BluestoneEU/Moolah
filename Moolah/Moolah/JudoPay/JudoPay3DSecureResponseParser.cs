using System.Xml.Linq;
using NLog;

namespace Moolah.JudoPay
{
    internal interface IJudoPay3DSecureResponseParser
    {
        I3DSecureResponse Parse(string dataCashResponse);
    }

    internal class JudoPay3DSecureResponseParser : IJudoPay3DSecureResponseParser
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public I3DSecureResponse Parse(string dataCashResponse)
        {
            var document = XDocument.Parse(dataCashResponse);

            var response = new JudoPay3DSecurePaymentResponse(document);

            string transactionReference;
            if (document.TryGetXPathValue("Response/datacash_reference", out transactionReference))
                response.TransactionReference = transactionReference;

            string avsCv2Result;
            if (document.TryGetXPathValue("Response/CardTxn/Cv2Avs/cv2avs_status", out avsCv2Result))
                response.AvsCv2Result = avsCv2Result;

            string auth_code;
            if (document.TryGetXPathValue("Response/CardTxn/authcode", out auth_code))
                response.AuthCode = auth_code;

            var dataCashStatus = int.Parse(document.XPathValue("Response/status"));
            switch (dataCashStatus)
            {
                case DataCashStatus.Success:
                    response.Status = PaymentStatus.Successful;
                    break;
                case DataCashStatus.RequiresThreeDSecureAuthentication:
                    response.Status = PaymentStatus.Pending;
                    response.Requires3DSecurePayerVerification = true;
                    // response.PAReq = document.XPathValue("Response/CardTxn/ThreeDSecure/pareq_message");
                    // response.ACSUrl = document.XPathValue("Response/CardTxn/ThreeDSecure/acs_url");
                    document.TryGetXPathValue("Response/ThreeDSecure/cReq", out var cReq);
                    document.TryGetXPathValue("Response/ThreeDSecure/methodUrl", out var methodUrl);
                    document.TryGetXPathValue("Response/ThreeDSecure/md", out var md);
                    document.TryGetXPathValue("Response/ThreeDSecure/challengeUrl", out var challengeUrl);
                    // challengeUrl = challengeUrl?.Replace("https:", "http:");
                    // methodUrl = methodUrl?.Replace("https:", "http:");
                    response.PAReq = !string.IsNullOrEmpty(cReq) ? cReq : md;
                    response.MD = md;
                    response.ChallengeUrl = challengeUrl;
                    response.ACSUrl = !string.IsNullOrEmpty(methodUrl) ? methodUrl : challengeUrl;
                    break;
                default:
                    if (DataCashStatus.CanImmediatelyAuthorise(dataCashStatus))
                    {
                        Log.Warn("Response status of '{0}' was returned for DataCash txn reference '{1}'. This txn can be immediately authorised.", dataCashStatus, transactionReference);
                        response.Status = PaymentStatus.Pending;
                    }
                    else
                    {
                        response.Status = PaymentStatus.Failed;
                        string reason;
                        document.TryGetXPathValue("Response/reason", out reason);

                        string information;
                        document.TryGetXPathValue("Response/information", out information);

                        response.IsSystemFailure = DataCashStatus.IsSystemFailure(dataCashStatus);
                        var failureReason = DataCashStatus.FailureReason(dataCashStatus);
                        response.FailureMessage = failureReason.Message;
                        response.FailureMessage = response.FailureMessage + (string.IsNullOrEmpty(information) ? "" : " " + information);
                        response.FailureMessage = response.FailureMessage + (string.IsNullOrEmpty(reason) ? "" : $" [{reason}]");
                        response.FailureType = failureReason.Type;
                    }
                    break;
            }

            return response;
        }
    }
}
