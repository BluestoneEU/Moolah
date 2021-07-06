using System.Configuration;
using Moolah.JudoPay;
using Moolah.PayPal;

namespace Moolah
{
    public class MoolahConfiguration : ConfigurationSection
    {
        /// <summary>
        /// The current configuration from an application config file.
        /// </summary>
        public static MoolahConfiguration Current
        {
            get { return (MoolahConfiguration) ConfigurationManager.GetSection(SectionName); }
        }

        public const string SectionName = "Moolah";

        static class Elements
        {
            public const string DataCashMoTo = "DataCashMoTo";
            public const string DataCash3DSecure = "DataCash3DSecure";
            public const string PayPal = "PayPal";
        }

        private const string XmlNamespaceConfigurationPropertyName = "xmlns";
        [ConfigurationProperty(XmlNamespaceConfigurationPropertyName)]
        public string XmlNamespace
        {
            get { return (string)this[XmlNamespaceConfigurationPropertyName]; }
            set { this[XmlNamespaceConfigurationPropertyName] = value; }
        }

        [ConfigurationProperty(Elements.DataCashMoTo)]
        public JudoPayConfiguration JudoPayMoTo
        {
            get { return (JudoPayConfiguration) this[Elements.DataCashMoTo]; }
            set { this[Elements.DataCashMoTo] = value; }
        }

        [ConfigurationProperty(Elements.DataCash3DSecure)]
        public JudoPay3DSecureConfiguration JudoPay3DSecure
        {
            get { return (JudoPay3DSecureConfiguration)this[Elements.DataCash3DSecure]; }
            set { this[Elements.DataCash3DSecure] = value; }
        }

        [ConfigurationProperty(Elements.PayPal)]
        public PayPalConfiguration PayPal
        {
            get { return (PayPalConfiguration)this[Elements.PayPal]; }
            set { this[Elements.PayPal] = value; }
        }
    }
}