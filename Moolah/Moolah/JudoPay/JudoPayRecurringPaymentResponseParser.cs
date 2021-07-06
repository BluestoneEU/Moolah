using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moolah.JudoPay
{
    internal interface IJudoPayRecurringPaymentResponseParser
    {
        IRecurringPaymentResponse Parse(string dataCashResponse);
    }

    internal class JudoPayRecurringPaymentResponseParser : IJudoPayRecurringPaymentResponseParser
    {
        //composition-based inheritance
        private IJudoPayResponseParser BaseParser { get; set; }
        public JudoPayRecurringPaymentResponseParser(IJudoPayResponseParser baseParser)
        {
            BaseParser = baseParser;
        }

        public IRecurringPaymentResponse Parse(string dataCashResponse)
        {
            string contAuthReference;
            var document = XDocument.Parse(dataCashResponse);
            var response = new JudoPayRecurringPaymentResponse(document);
            BaseParser.SetPaymentValues(document, response);
            if (document.TryGetXPathValue("Response/ContAuthTxn/ca_reference", out contAuthReference))
                response.CAReference = contAuthReference;
            return response;
        }
    }
}
