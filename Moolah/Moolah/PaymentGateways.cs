﻿using System.Collections.Generic;
using Moolah.DataCash;
using Moolah.PayPal;
using System;

namespace Moolah
{
    public interface IPaymentGateway : ICanRefundTransactions, ICanCancelTransactions
    {
        /// <summary>
        /// Attempts to transact the specified amount using the card details provided.
        /// </summary>
        /// <param name="merchantReference">The merchant's reference for the transaction.</param>
        /// <param name="amount">The amount to transact.</param>
        /// <param name="card">Credit or debit card details.</param>
        /// <param name="billingAddress">The billing address for the card.  If provided, then address verifications checks are run.</param>
        ICardPaymentResponse Payment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null, MCC6012 mcc6012 = null);
    }

    public interface IRecurringPaymentGateway: ICanRefundTransactions, ICanCancelTransactions
    {
        IRecurringPaymentResponse SetupPayment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null, MCC6012 mcc6012 = null, string captureMethod = "ecomm");

        IRecurringPaymentResponse RepeatPayment(string merchantReference, string caReference, decimal amount, string currencyCode = null, MCC6012 mcc6012 = null, string captureMethod = "cont_auth");

        IRefundTransactionResponse RefundRepeatTransaction(string originalTransactionReference, decimal amount, string captureMethod = "ecomm");
    }

    public interface I3DSecurePaymentGateway : ICanRefundTransactions, ICanCancelTransactions
    {
        /// <summary>
        /// Attempts to make a card payment.  If the card is enrolled in 3D-Secure, then the Access Control Server URL (ACSUrl)
        /// and Payer Authentication Request (PARes) are returned for the calling site to redirect or show in an iframe.
        /// This should then be followed by a call to <see cref="Authorise"/> with the resulting PARes returned by the ACS.
        /// </summary>
        /// <param name="merchantReference">The merchant's reference for the transaction.</param>
        /// <param name="amount">The amount to transact.</param>
        /// <param name="card">Card details.</param>
        /// <param name="billingAddress">The billing address for the card.  If provided, then address verifications checks are run.</param>
        I3DSecureResponse Payment(string merchantReference, decimal amount, CardDetails card, BillingAddress billingAddress = null, Cv2AvsPolicy policy = Cv2AvsPolicy.UNSPECIFIED, string currencyCode = null, MCC6012 mcc6012 = null);

        /// <summary>
        /// Attempts to authorise a 3D-Secure transaction previously submitted to the <see cref="Payment"/> method.
        /// </summary>
        /// <param name="transactionReference">Transaction reference returned by the Gateway for the original 3D-Secure payment request.</param>
        /// <param name="PARes">Payer Authentication Response (PARes) returned by the bank in response to the user entering their 3D-Secure credentials.</param>
        I3DSecureResponse Authorise(string transactionReference, string PARes);

        /// <summary>
        /// Merchant vTID number
        /// </summary>
        string MerchantId { get; }
    }

    public interface ICanCancelTransactions
    {
        /// <summary>
        /// Attempts to cancel a historic DataCash transaction.  Note that transactions can only be cancelled before they are settled.
        /// </summary>
        /// <param name="originalTransactionReference">The DataCash reference provided for the original transaction you are cancelling.</param>
        ICancelTransactionResponse CancelTransaction(string originalTransactionReference);
    }

    public interface ICanRefundTransactions
    {
        /// <summary>
        /// Attempts to refund a historic DataCash transaction.
        /// </summary>
        /// <param name="originalTransactionReference">The DataCash reference provided for the original transaction you are refunding.</param>
        /// <param name="amount">The amount you wish to refund, which must be less than or equal to the original transaction amount, less any previous refunds made for that transaction.</param>
        IRefundTransactionResponse RefundTransaction(string originalTransactionReference, decimal amount);   
    }

    public interface IPayPalExpressCheckout
    {
        /// <summary>
        /// Starts a PayPal express checkout session by providing a token and URL
        /// for the user to be redirected to.
        /// </summary>
        /// <param name="amount">Transaction amount.</param>
        /// <param name="cancelUrl">URL to return to if the customer cancels the checkout process.</param>
        /// <param name="confirmationUrl">URL to return to for the customer to confirm payment and place the order.</param>
        /// <returns>
        /// A PayPal token for the express checkout and URL to redirect the customer to.
        /// When the customer navigates to the confirmationUrl, you should then call
        /// <see cref="GetExpressCheckoutDetails"/> to retrieve details about the express checkout.
        /// </returns>
        PayPalExpressCheckoutToken SetExpressCheckout(decimal amount, CurrencyCodeType currencyCodeType, string cancelUrl, string confirmationUrl);

        /// <summary>
        /// Starts a PayPal express checkout session by providing a token and URL
        /// for the user to be redirected to.
        /// </summary>
        /// <param name="orderDetails">Detailed information on the order this checkout is for.</param>
        /// <param name="cancelUrl">URL to return to if the customer cancels the checkout process.</param>
        /// <param name="confirmationUrl">URL to return to for the customer to confirm payment and place the order.</param>
        /// <returns>
        /// A PayPal token for the express checkout and URL to redirect the customer to.
        /// When the customer navigates to the confirmationUrl, you should then call
        /// <see cref="GetExpressCheckoutDetails"/> to retrieve details about the express checkout.
        /// </returns>
        PayPalExpressCheckoutToken SetExpressCheckout(OrderDetails orderDetails, string cancelUrl, string confirmationUrl);

        /// <summary>
        /// Retrieves information about the express checkout required to carry out authorisation of payment.
        /// </summary>
        /// <param name="payPalToken">The PayPal token returned in the initial <see cref="SetExpressCheckout"/> call.</param>
        /// <returns>
        /// Details about the express checkout, such as the customer details, and PayPal PayerId, which 
        /// must be passed to <see cref="DoExpressCheckoutPayment"/> to perform the payment.
        /// </returns>
        PayPalExpressCheckoutDetails GetExpressCheckoutDetails(string payPalToken);

        /// <summary>
        /// Performs the payment.
        /// </summary>
        /// <param name="amount">Transaction amount.  You may have adjusted the amount depending on the delivery options the customer specified in PayPal.</param>
        /// <param name="payPalToken">The PayPal token returned in the initial <see cref="SetExpressCheckout"/> call.</param>
        /// <param name="payPalPayerId">The PayPal PayerID returned in the <see cref="GetExpressCheckoutDetails"/> call.</param>
        IPaymentResponse DoExpressCheckoutPayment(decimal amount, CurrencyCodeType currencyCodeType, string payPalToken, string payPalPayerId);

        /// <summary>
        /// Performs the payment.
        /// </summary>
        /// <param name="orderDetails">Detailed information on the order this checkout is for.</param>
        /// <param name="payPalToken">The PayPal token returned in the initial <see cref="SetExpressCheckout"/> call.</param>
        /// <param name="payPalPayerId">The PayPal PayerID returned in the <see cref="GetExpressCheckoutDetails"/> call.</param>        
        IPaymentResponse DoExpressCheckoutPayment(OrderDetails orderDetails, string payPalToken, string payPalPayerId);

        /// <summary>
        /// Performs a full transaction refund.
        /// </summary>
        /// <param name="transactionId">The PayPal transaction id to be refunded.</param>
        /// <returns>The status of the refund.</returns>
        IPayPalRefundResponse RefundFullTransaction(string transactionId);

        /// <summary>
        /// Performs a partial transaction refund.
        /// </summary>
        /// <param name="transactionId">The transaction Id received from the DoExpressCheckoutPayment call.</param>
        /// <param name="amount">The amount to refund.</param>
        /// <param name="currencyCodeType">The Currency Code for the refund.</param>
        /// <param name="description">A note about the refund. Optional.</param>
        /// <returns>The status of the refund.</returns>
        IPayPalRefundResponse RefundPartialTransaction(string transactionId, decimal amount, CurrencyCodeType currencyCodeType, string description = null);
    }

    public interface IPayPalMassPay
    {
        /// <summary>
        /// Performs PayPal Mass Payment.
        /// <param name="items">List of payments do be done.</param>
        /// <param name="currencyCodeType">The Currency Code for payment. You cannot mix currencies in a single Mass Payment. A single request must include items that are of the same currency.</param>
        /// <param name="receiverType">Indicates how do you identify the recipients of payments in this request. By default it's email address.</param>
        /// <param name="emailSubject">The subject line of the email that PayPal sends when the transaction is completed. The subject line is the same for all recipients. Character length and limitations: 255 single-byte alphanumeric characters. Optional.</param>        
        /// </summary>
        IPaymentResponse DoMassPayment(IEnumerable<PayReceiver> items, CurrencyCodeType currencyCodeType, ReceiverType receiverType = ReceiverType.EmailAddress, string emailSubject = "");
    }
}
