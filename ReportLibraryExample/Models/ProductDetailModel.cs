using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportLibrary.Example
{
    public class ProductDetailModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public String Size { get; set; }

        public float Width { get; set; }

        public float Length { get; set; }

        public float Height { get; set; }

        public float Weight { get; set; }

        public string Description { get; set; }
    }
}
