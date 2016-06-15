using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data;
using MvcReportViewer.Example.Models;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Collections;
using ReportLibrary.Example;
using System.Linq;

namespace MvcReportViewer.Example.Controllers
{
    public class HomeController : Controller
    {
        private const string RemoteReportName = "/TestReports/TestReport";
        private const string LocalReportName = "App_Data/Reports/Products.rdlc";
        private const string LocalNoDataReportName = "App_Data/Reports/NoDataReport.rdlc";

        //private const string LocalReportAssembly = "ReportLibraryExample";
        private const string LocalEmbeddedReportName = "ReportLibrary.Example.ReportFiles.ProductReport.rdlc";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fluent()
        {
            return View();
        }

        public ActionResult Post()
        {
            return View();
        }

        public ActionResult Multiple()
        {
            return View();
        }

        public ActionResult VisibilityCheck()
        {
            return View();
        }

        public ActionResult NoDataLocalReport()
        {
            return View();
        }

        public ActionResult DownloadPdfLocalNoData()
        {
            return this.Report(
                ReportFormat.PDF,
                new LocalReportLoader(LocalNoDataReportName),
                new { TodayDate = DateTime.Now },
                ProcessingMode.Local);
        }

        public ActionResult DownloadExcel()
        {
            return DownloadReport(ReportFormat.Excel, true);
        }

        public ActionResult DownloadWord()
        {
            return DownloadReportMultipleValues(ReportFormat.Word);
        }

        public ActionResult DownloadPdf()
        {
            return DownloadReport(ReportFormat.PDF, false);
        }

        private ActionResult DownloadReport(ReportFormat format, bool isLocalReport)
        {
            if (isLocalReport)
            {
                return this.Report(
                    format,
                    new LocalReportLoader(LocalReportName),
                    new { Parameter1 = "Test", Parameter2 = 123 },
                    ProcessingMode.Local,
                    new IDataSource[]
                    {
                        new DataTableDataSource("Products", GetProducts()),
                        new DataTableDataSource("Cities", GetCities())
                    });
            }

            return this.Report(
                format,
                new RemoteServerLoader(RemoteReportName),
                new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 },
                ProcessingMode.Remote);
        }

        private ActionResult DownloadReportMultipleValues(ReportFormat format)
        {
            return this.Report(
                format,
                new RemoteServerLoader(RemoteReportName),
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Parameter1", "Value 1"),
                    new KeyValuePair<string, object>("Parameter1", "Value 2"),
                    new KeyValuePair<string, object>("Parameter2", DateTime.Now),
                    new KeyValuePair<string, object>("Parameter2", DateTime.Now.AddYears(10)),
                    new KeyValuePair<string, object>("Parameter3", 12345)
                },
                ProcessingMode.Remote);
        }

        public ActionResult LocalReports()
        {
            var model = new SqlLocalReportsModel
            {
                Products = "select * from dbo.Products",
                Cities = "select * from dbo.Cities",
                FilteredCities = "select * from dbo.Cities where Id < 3"
            };

            return View(model);
        }

        public ActionResult FluentEmbedded()
        {
            var model = new EmbeddedReportModel() 
            {
                Products = GetProducts2()
            };
            
            return View(model);
        }

        public ActionResult FluentEmbeddedSubReport()
        {
            var model = new EmbeddedReportModel()
            {
                Products = GetProducts2(),
                ProductDetails = GetProductDetails()
            };

            return View(model);
        }

        public ActionResult LocalEmbeddedReports()
        {
            return this.Report(
                ReportFormat.Excel,
                new LocalReportAssemblyResourceLoader(
                    "ReportLibraryExample",
                    "ReportLibrary.Example.ReportFiles.ProductReport.rdlc"),
                new { Parameter1 = "Test", Parameter2 = 123 },
                ProcessingMode.Local,
                new IDataSource[]
                {
                    new EnumerableDataSource("Products", GetProducts2())
                });
        }

        public ActionResult LocalReportWithSubReport()
        {
            return this.Report(
                ReportFormat.ExcelOpenXML,

                new LocalReportAssemblyResourceLoader(
                    "ReportLibraryExample",
                    "ReportLibrary.Example.ReportFiles.ProductWithDetail.rdlc",
                    new SubReportResourceName[]
                    {
                        new SubReportResourceName("ProductDetailReport", "ReportLibrary.Example.ReportFiles.ProductDetailReport.rdlc")
                    }),

                new { Parameter1 = "Test", Parameter2 = 123 },
                ProcessingMode.Local,

                new IDataSource[]
                {
                    new EnumerableDataSource("Products", GetProducts2())
                },
                
                new ISubReportDataSource[]
                {
                    new SubReportEnumerableDataSource<ProductDetailModel>(
                        "ProductDetailReport",
                        new GenericEnumerableDataSource<ProductDetailModel>("ProductDetails", GetProductDetails()), 
                            (parameters, data) => data.Where(pd => pd.ProductId == int.Parse((parameters["ProductId"].Values[0])))) 
                });
        }

        #region Helper Methods for SessionLocalDataSourceProvider examples

        private DataTable GetProducts()
        {
            return GetDataTable("select * from dbo.Products");
        }

        private DataTable GetCities()
        {
            return GetDataTable("select * from dbo.Cities");
        }

        private DataTable GetDataTable(string sql)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Products"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        private IEnumerable<ProductModel> GetProducts2()
        {
            return new ProductModel[] 
            {
                new ProductModel()
                {
                    Id = 1,
                    Name = "aaa",
                    ProductTypeName = "111",
                    Description = "description1"
                },

                new ProductModel()
                {
                    Id = 2,
                    Name = "bbb",
                    ProductTypeName = "222",
                    Description = "description2"
                },
            };
        }

        private IEnumerable<ProductDetailModel> GetProductDetails()
        {
            return new ProductDetailModel[]
            {
                new ProductDetailModel()
                {
                    Id = 1,
                    ProductId = 1,
                    Size = "S",
                    Width = 1,
                    Length = 2,
                    Height = 3,
                    Weight = 4,
                },

                new ProductDetailModel()
                {
                    Id = 2,
                    ProductId = 1,
                    Size = "M",
                    Width = 5,
                    Length = 6,
                    Height = 7,
                    Weight = 8,
                },

                new ProductDetailModel()
                {
                    Id = 3,
                    ProductId = 2,
                    Size = "L",
                    Width = 6,
                    Length = 7,
                    Height = 8,
                    Weight = 8,
                }
            };
        }

        #endregion
    }
}
