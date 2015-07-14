using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcReportViewer
{
    public class ReportLoaderFactory
    {
        public static IReportLoader CreateByRequestData(int reportType, NameValueCollection queryString, bool isEncrypted)
        {
            switch(reportType)
            {
                case 1:
                    return new LocalReportLoader(queryString, isEncrypted);

                case 2:
                    return new LocalReportAssemblyResourceLoader(queryString, isEncrypted);

                case 3:
                    break;

                case 4:
                    break;
            }

            throw new ArgumentException("query string has incorrect report type!");
        }
    }
}
