using System;
using System.Configuration;

namespace Moolah.JudoPay
{
    public class JudoPayConfiguration : ConfigurationElement
    {
        const string TestHost = "https://api-sandbox.judopay.com/bridge/dpg/transactions";
        const string LiveHost = "https://api.judopay.com/bridge/dpg/transactions";

        internal JudoPayConfiguration()
        {
        }

        public JudoPayConfiguration(PaymentEnvironment environment, string merchantId, string password)
        {
            Environment = environment;
            MerchantId = merchantId;
            Password = password;
        }

        static class Attributes
        {
            public const string Environment = "environment";
            public const string MerchantId = "merchantId";
            public const string Password = "password";
        }

        [ConfigurationProperty(Attributes.Environment)]
        public PaymentEnvironment Environment
        {
            get { return (PaymentEnvironment) this[Attributes.Environment]; }
            set { this[Attributes.Environment] = value; }
        }

        [ConfigurationProperty(Attributes.MerchantId)]
        public string MerchantId
        {
            get { return (string)this[Attributes.MerchantId]; }
            set { this[Attributes.MerchantId] = value; }
        }

        [ConfigurationProperty(Attributes.Password)]
        public string Password
        {
            get { return (string) this[Attributes.Password]; }
            set { this[Attributes.Password] = value; }
        }

        public string Host
        {
            get { return Environment == PaymentEnvironment.Live ? LiveHost : TestHost; }
        }
    }

    public class JudoPay3DSecureConfiguration : JudoPayConfiguration
    {
        internal JudoPay3DSecureConfiguration()
        {
        }

        public JudoPay3DSecureConfiguration(PaymentEnvironment environment, string merchantId, string password,
            string merchantUrl, string purchaseDescription, string methodNotificationUrl, string challengeNotificationUrl)
            : base(environment, merchantId, password)
        {
            if (string.IsNullOrWhiteSpace(merchantUrl)) throw new ArgumentNullException("merchantUrl");
            if (string.IsNullOrWhiteSpace(purchaseDescription)) throw new ArgumentNullException("purchaseDescription");
            MerchantUrl = merchantUrl;
            PurchaseDescription = purchaseDescription;
            MethodNotificationUrl = methodNotificationUrl;
            ChallengeNotificationUrl = challengeNotificationUrl;
        }

        static class Attributes
        {
            public const string MerchantUrl = "merchantUrl";
            public const string PurchaseDescription = "purchaseDescription";
            public const string MethodNotificationUrl = "methodNotificationUrl";
            public const string ChallengeNotificationUrl = "challengeNotificationUrl";
        }

        /// <summary>
        /// The URL of the merchant website.  
        /// This is displayed to the customer by the bank during the 3D-Secure authentication process.
        /// </summary>
        [ConfigurationProperty(Attributes.MerchantUrl)]
        public string MerchantUrl
        {
            get { return (string) this[Attributes.MerchantUrl]; }
            set { this[Attributes.MerchantUrl] = value; }
        }

        /// <summary>
        /// The URL for the additional client information request.  
        /// This is used to send a hidden request with client information during the 3D-Secure authentication process.
        /// </summary>
        [ConfigurationProperty(Attributes.MethodNotificationUrl)]
        public string MethodNotificationUrl 
        { 
            get { return (string) this[Attributes.MethodNotificationUrl]; }
            set { this[Attributes.MethodNotificationUrl] = value; } 
        }

        /// <summary>
        /// The URL with the challenge page.  
        /// This is displayed to the customer by the bank during the 3D-Secure authentication process.
        /// </summary>
        [ConfigurationProperty(Attributes.ChallengeNotificationUrl)]
        public string ChallengeNotificationUrl 
        { 
            get { return (string) this[Attributes.ChallengeNotificationUrl]; }
            set { this[Attributes.ChallengeNotificationUrl] = value; } 
        }

        /// <summary>
        /// A short description of items to be purchased.
        /// This is displayed to the customer by the bank during the 3D-Secure authentication process.
        /// </summary>
        [ConfigurationProperty(Attributes.PurchaseDescription)]
        public string PurchaseDescription
        {
            get { return (string) this[Attributes.PurchaseDescription]; }
            set { this[Attributes.PurchaseDescription] = value; }
        }
    }
}