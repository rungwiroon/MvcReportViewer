using ReportLibrary.Example;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcReportViewer.Example.Models
{
    public class EmbeddedReportModel
    {
        public IEnumerable<ProductModel> Products { get; set; }

        public IEnumerable<ProductDetailModel> ProductDetails { get; set; }
    }
}
