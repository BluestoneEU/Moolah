using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moolah.DataCash
{
    public interface IDataCashRecurringPaymentResponseParser
    {
        IRecurringPaymentResponse Parse(string dataCashResponse);
    }

    public class DataCashRecurringPaymentResponseParser : IDataCashRecurringPaymentResponseParser
    {
        //composition-based inheritance
        private IDataCashResponseParser BaseParser { get; set; }
        public DataCashRecurringPaymentResponseParser(IDataCashResponseParser baseParser)
        {
            BaseParser = baseParser;
        }

        public IRecurringPaymentResponse Parse(string dataCashResponse)
        {
            string contAuthReference;
            var document = XDocument.Parse(dataCashResponse);
            var response = new DataCashRecurringPaymentResponse(document);
            BaseParser.SetPaymentValues(document, response);
            if (document.TryGetXPathValue("Response/ContAuthTxn/ca_reference", out contAuthReference))
                response.CAReference = contAuthReference;
            return response;
        }
    }
}
