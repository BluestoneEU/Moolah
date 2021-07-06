using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    public class MCC6012
    {
        /// <summary>
        /// Date of birth of primary recipient 
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Account number, PAN or partial PAN of recipient account.
        /// As account details can be numeric, alphabetic or mixed, the DPG will accept up to 19 alpha-numeric, characters (0-9, a-z, A-Z, space and hyphen). 
        /// If the account details supplied are 15, 16 or 19 characters and all numeric, Mastercard Payment Gateway Services will assume that this is a PAN and will truncate it to a partial PAN using the first 6 and the last 4 characters. 
        /// In other cases the DPG will truncate or pad the supplied value to 10 characters.
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Postcode or partial Postcode of recipient account. The DPG will accept either a partial post code, for example EH6, or a full post code like EH6 6JH. 
        /// In the latter case, Mastercard Payment Gateway Services will only use the first part.  
        /// Must include a single space between parts if whole.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Supply the full surname as it appears on the card and/or account up to 35 characters. Mastercard Payment Gateway Services will strip out any unsupported characters. 
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// If this is true, DateOfBirth can be null.
        /// </summary>
        public bool AccountHolderIsACompany { get; set; }

        /// <summary>
        /// If true - postal code will no be validated on null or empty string
        /// </summary>
        public bool PostalCodeCanBeEmpty { get; set; }

        internal XElement ToElement()
        {
            if (String.IsNullOrEmpty(AccountNumber))
                throw new ArgumentException($"{nameof(MCC6012)}.{AccountNumber} must not be null.");

            if (!AccountHolderIsACompany && DateOfBirth == null)
                throw new ArgumentException($"{nameof(MCC6012)}.{DateOfBirth} must not be null.");

            if (!PostalCodeCanBeEmpty && String.IsNullOrEmpty(PostalCode))
                throw new ArgumentException($"{nameof(MCC6012)}.{PostalCode} must not be null.");

            if (String.IsNullOrEmpty(Surname))
                throw new ArgumentException($"{nameof(MCC6012)}.{Surname} must not be null.");

            return new XElement("MCC6012",
                new XElement("account_number", AccountNumber),
                new XElement("postal_code", PostalCode),
                new XElement("surname", Surname),
                new XElement("dob", AccountHolderIsACompany ? "00000000" : DateOfBirth?.ToString("yyyyMMdd") ?? "00000000")
            );
        }
    }
}
