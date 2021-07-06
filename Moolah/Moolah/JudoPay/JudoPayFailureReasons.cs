using System.Collections.Generic;

namespace Moolah.JudoPay
{
    /// <summary>
    /// See https://testserver.datacash.com/software/returncodes.shtml for full list of DataCash status codes.
    /// </summary>
    public static class JudoPayFailureReasons
    {
        /// <summary>
        /// Failures with messages that can be displayed directly to the customer.
        /// </summary>
        public static readonly Dictionary<int, JudoPayFailureReason> CleanFailures = new Dictionary<int, JudoPayFailureReason>
        {
            { DataCashStatus.NotAuthorised, new JudoPayFailureReason("Transaction was declined by your bank. Please check your details or contact your bank and try again.", CardFailureType.General) },
            { 24, new JudoPayFailureReason("The supplied expiry date is in the past.", CardFailureType.ExpiryDate) },
            { 25, new JudoPayFailureReason("The card number is invalid.", CardFailureType.CardNumber) },
            { 26, new JudoPayFailureReason("The card number does not have the expected number of digits.", CardFailureType.CardNumber) },
            { 27, new JudoPayFailureReason("The card issue number is missing or invalid.", CardFailureType.IssueNumber) },
            { 28, new JudoPayFailureReason("The card start date is missing or invalid.", CardFailureType.StartDate) },
            { 29, new JudoPayFailureReason("The supplied start date is in the future.", CardFailureType.StartDate) },
            { 30, new JudoPayFailureReason("The start date cannot be after the expiry date.", CardFailureType.StartDate) },
            { 53, new JudoPayFailureReason("The transaction was not processed due to a high number of requests.  Please try again.", CardFailureType.General) },
            { 56, new JudoPayFailureReason("The transaction was declined due to the card being used too recently.  Please try again later.", CardFailureType.General) },
            // 3D-Secure related
            { 179, new JudoPayFailureReason("Transaction was declined by your bank. Please check your details or contact your bank and try again.", CardFailureType.General) },
            { 184, new JudoPayFailureReason("The time limit for entering your password expired. You have not been charged. Please try again.", CardFailureType.General) },
            { 185, new JudoPayFailureReason("The time limit for entering your password expired. You have not been charged. Please try again.", CardFailureType.General) }
        };

        /// <summary>
        /// Failures with messages that should not be displayed to the customer, but may indicate a system failure.
        /// </summary>
        public static readonly Dictionary<int, JudoPayFailureReason> SystemFailures = new Dictionary<int, JudoPayFailureReason>
        {
            { 2, new JudoPayFailureReason("Communication was interrupted. The argument is e.g. 523/555 (523 bytes written but 555 expected).", CardFailureType.General) },
            { 3, new JudoPayFailureReason("A timeout occurred while we were reading the transaction details.", CardFailureType.General) },
            { 5, new JudoPayFailureReason("A field was specified twice, you sent us too much or invalid data, a pre-auth lookup failed during a fulfill transaction, the swipe field was incorrectly specified, or you omitted a field.", CardFailureType.General) },
            { 6, new JudoPayFailureReason("Error in communications link; resend.", CardFailureType.General) },
            { 9, new JudoPayFailureReason("The currency you specified does not exist", CardFailureType.General) },
            { 10, new JudoPayFailureReason("The vTID or password were incorrect", CardFailureType.General) },
            { 12, new JudoPayFailureReason("The authcode you supplied was invalid", CardFailureType.General) },
            { 13, new JudoPayFailureReason("You did not supply a transaction type.", CardFailureType.General) },
            { 14, new JudoPayFailureReason("Transaction details could not be committed to our database.", CardFailureType.General) },
            { 15, new JudoPayFailureReason("You specified an invalid transaction type.", CardFailureType.General) },
            { 19, new JudoPayFailureReason("You attempted to fulfill a transaction that either could not be fulfilled (e.g. auth, refund) or already has been.", CardFailureType.General) },
            { 20, new JudoPayFailureReason("A successful transaction has already been sent using this vTID and reference number.", CardFailureType.General) },
            { 21, new JudoPayFailureReason("This terminal does not accept transactions for this type of card.", CardFailureType.General) },
            { 22, new JudoPayFailureReason("Reference numbers should be 16 digits for fulfill transactions, or between 6 and 30 digits for all others.", CardFailureType.General) },
            { 23, new JudoPayFailureReason("The expiry dates should be specified as MM/YY or MM-YY.", CardFailureType.General) },
            { 34, new JudoPayFailureReason("The amount is missing, is not fully specified to x.xx format", CardFailureType.General) },
            { 51, new JudoPayFailureReason("A transaction with this reference number is already going through the system.", CardFailureType.General) },
            { 59, new JudoPayFailureReason("This combination of currency, card type and environment is not supported by this vTID", CardFailureType.General) },
            { 60, new JudoPayFailureReason("The XML Document is not valid with our Request schema.", CardFailureType.General) },
            { 61, new JudoPayFailureReason("An error in account configuration caused the transaction to fail. Contact DataCash Technical Support", CardFailureType.General) },
            { 63, new JudoPayFailureReason("The transaction type is not supported by the Acquirer", CardFailureType.General) },
            { 104, new JudoPayFailureReason("Error in bank authorization, where APACS30 Response message refers to different TID to that used in APACS30 Request message; resend.", CardFailureType.General) },
            { 105, new JudoPayFailureReason("Error in bank authorization, where APACS30 Response message refers to different message number to that used in APACS30 Request message; resend.", CardFailureType.General) },
            { 106, new JudoPayFailureReason("Error in bank authorization, where APACS30 Response message refers to different amount to that used in APACS30 Request message; resend.", CardFailureType.General) },
            { 190, new JudoPayFailureReason("Your vTID is capable of dealing with transactions from different environments (e.g. MoTo, e-comm), but you have not specified from which environment this transaction has taken place.", CardFailureType.General) },
            { 280, new JudoPayFailureReason("The datacash reference should be a 16 digit number. The first digit (2, 9, 3 or 4) indicates the format used and whether the txn was processed in a live or test environment.", CardFailureType.General) },
            { 281, new JudoPayFailureReason("The new format of datacash reference includes a luhn check digit. The number supplied failed to pass the luhn check.", CardFailureType.General) },
            { 282, new JudoPayFailureReason("The site_id extracted from the datacash reference does not match the current environment", CardFailureType.General) },
            { 283, new JudoPayFailureReason("The mode flag extracted from the datacash reference does not match the current environment", CardFailureType.General) },
            { 440, new JudoPayFailureReason("Out of external connections", CardFailureType.General) },
            { 470, new JudoPayFailureReason("Maestro transactions for Card Holder not present are not supported for the given clearinghouse", CardFailureType.General) },
            { 471, new JudoPayFailureReason("This transaction must be a 3dsecure transaction", CardFailureType.General) },
            { 472, new JudoPayFailureReason("International Maestro is not permitted in a Mail order / telephone order environment.", CardFailureType.General) },
            { 473, new JudoPayFailureReason("Keyed International Maestro transaction not permitted", CardFailureType.General) },
            { 480, new JudoPayFailureReason("The Merchant Id provided is invalid", CardFailureType.General) },
            { 481, new JudoPayFailureReason("The merchant is expected to provide a Merchant Id with each transaction", CardFailureType.General) },
            { 482, new JudoPayFailureReason("The merchant is not set to provide Merchant Id for a transaction", CardFailureType.General) },
            { 510, new JudoPayFailureReason("The merchant attempted to use a GE Capital card with a BIN that does not belong to them", CardFailureType.General) },
            { 1100, new JudoPayFailureReason("No referenced transaction found", CardFailureType.General) },
            { 1101, new JudoPayFailureReason("Only referred transactions can be authorised", CardFailureType.General) },
            { 1102, new JudoPayFailureReason("Only pre or auth transaction can be authorised", CardFailureType.General) },
            { 1103, new JudoPayFailureReason("Must supply updated authcode to authorise transaction", CardFailureType.General) },
            { 1104, new JudoPayFailureReason("Transactions cannot be authorized after time limit expired. The default timeout value is set to 6hours but can be amended per Vtid by contacting DataCash Support.", CardFailureType.General)},
            { 1105, new JudoPayFailureReason("The transaction referenced was both referred by the acquiring bank, and challanged by the Retail Decisions (ReD) Service. This transaction cannot be authorised through the use of an authorize_referral_request or fulfill transaction, nor have the challenge accepted with an accept_fraud transaction. If a new authorisation code is obtained for the acquiring bank, the merchant can continue with the transaction by resubmitting the entrie transaction and providing the new authorisation code.", CardFailureType.General) },
            { 1106, new JudoPayFailureReason("Historic reference already in use", CardFailureType.General) },
            { 12001, new JudoPayFailureReason("There are more than one active passwords already registered against your vTID at the time the txn was received.", CardFailureType.General) },
            { 12002, new JudoPayFailureReason("The IP address of the system submitting the vtidconfiguration request is not registered against your vTID.", CardFailureType.General) },
            // 3D-Secure Related
            { 151, new JudoPayFailureReason("A transaction type other than 'auth' or 'pre' was received in the 3-D-Secure Enrolment Check Request", CardFailureType.General) },
            { 152, new JudoPayFailureReason("An authcode was supplied in the 3-D Secure Authorization Request, this is not allowed", CardFailureType.General) },
            { 153, new JudoPayFailureReason("The mandatory 'verify' element was not supplied in the 3-D Secure Enrolment Check Request", CardFailureType.General) },
            { 154, new JudoPayFailureReason("The mandatory 'verify' element was supplied, but its value was something other than 'yes' or 'no'", CardFailureType.General) },
            { 155, new JudoPayFailureReason("One of the required fields: 'merchant_url', 'purchase_datetime', 'purchase_desc' or 'device_category' was not supplied", CardFailureType.General) },
            { 156, new JudoPayFailureReason("The required field 'device_category' was supplied, but contains a value other than 0 or 1", CardFailureType.General) },
            { 157, new JudoPayFailureReason("The merchant is not setup to do 3-D Secure transactions", CardFailureType.General) },
            { 158, new JudoPayFailureReason("The card scheme is not supported in the 3-D Secure environment", CardFailureType.General) },
            { 159, new JudoPayFailureReason("The enrolment verification with scheme directory server failed", CardFailureType.General) },
            { 160, new JudoPayFailureReason("Received an invalid response from the scheme directory server", CardFailureType.General) },
            { 161, new JudoPayFailureReason("The 3-D Secure Authorization Request was not authorized and returned a referral response", CardFailureType.General) },
            { 163, new JudoPayFailureReason("Not enabled for this card scheme with that acquiring bank", CardFailureType.General) },
            { 164, new JudoPayFailureReason("The acquirer is not supported by 3-D Secure", CardFailureType.General) },
            { 165, new JudoPayFailureReason("Not enabled to do 3-D Secure transactions with this acquirer", CardFailureType.General) },
            { 166, new JudoPayFailureReason("The format of the 'purchase_datetime' field supplied in the 3-D Secure Enrolment Check Request is invalid", CardFailureType.General) },
            { 167, new JudoPayFailureReason("An invalid reference was supplied in the 3-D Secure Authorization Request", CardFailureType.General) },
            { 168, new JudoPayFailureReason("The transaction could not be submitted for authorization as no valid 3-D Secure Enrolment Check Request was found", CardFailureType.General) },
            { 169, new JudoPayFailureReason("A magic card as not been supplied in transaction where 3-D Secure subscription mode is test", CardFailureType.General) },
            { 170, new JudoPayFailureReason("The DataCash MPI software has no directory server URL details for this scheme", CardFailureType.General) },
            { 171, new JudoPayFailureReason("A PARes message has been supplied in 3-D Secure Authorization Request. This is not allowed", CardFailureType.General) },
            { 172, new JudoPayFailureReason("The required PARes message was not supplied in 3-D Secure Authorization Request", CardFailureType.General) },
            { 174, new JudoPayFailureReason("The PARes response message from the Issuer could not be verified", CardFailureType.General) },
            { 175, new JudoPayFailureReason("A PARes message has been received in 3-D Secure Authorization Request, but no Matching PAReq message was found", CardFailureType.General) },
            { 176, new JudoPayFailureReason("The PARes message received in the 3-D Secure Authorization Request is invalid", CardFailureType.General) },
            { 177, new JudoPayFailureReason("The PAReq message received from the 3-D Secure Enrolment Check Request is invalid", CardFailureType.General) },
            { 178, new JudoPayFailureReason("The PAReq and PARes messages do not match on one of these key fields: 'message_id', 'acqBIN', 'merID', 'xid', 'date', 'purchAmount', 'currency' or 'exponent'", CardFailureType.General) },
            { 180, new JudoPayFailureReason("The DataCash MPI does not support recurring transactions", CardFailureType.General) },
            { 181, new JudoPayFailureReason("A 3-D Secure Authorization Request found there was no matching referral to be authorized", CardFailureType.General) },
            { 182, new JudoPayFailureReason("A 3-D Secure Authorization Request found that the timelimit for an authorization on a transaction that had previously received a response had been exceeded", CardFailureType.General) },
            { 183, new JudoPayFailureReason("A 3-D Secure Enrolment Check Request had the verify element set to 'no' meaning no 3-D Secure verification is to be done.", CardFailureType.General) },
            { 186, new JudoPayFailureReason("3DS Invalid VEReq", CardFailureType.General) },
            { 187, new JudoPayFailureReason("Unable to Verify", CardFailureType.General) },
            { 188, new JudoPayFailureReason("Unable to Authenticate", CardFailureType.General) }
        };
    }
}