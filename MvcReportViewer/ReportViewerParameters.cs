using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System;
using System.Data;
using System.Collections;

namespace MvcReportViewer
{
    public class ReportViewerParameters
    {
        public ReportViewerParameters()
        {
            ReportParameters = new Dictionary<string, ReportParameter>();
            //ReportLoaderParameters = new Dictionary<string, object>();
        }

        public int ReportLoaderType { get; set; }

        //public string ReportServerUrl { get; set; }

        //public string Username { get; set; }

        //public string Password { get; set; }

        //public string ReportPath { get; set; }

        //public string ReportAssemblyName { get; set; }

        //public string MainReportResourceName { get; set; }

        //public string SubReportResourceNames { get; set; }

        //public Dictionary<string, object> ReportLoaderParameters { get; set; }

        public Guid? ControlId { get; set; }

        public ProcessingMode ProcessingMode { get; set; }

        public bool IsAzureSSRS { get; set; }

        public IReportLoader ReportLoader { get; set; }

        public IDictionary<string, ReportParameter> ReportParameters { get; set; }

        public IEnumerable<IDataSource> ReportDataSources { get; set; }

        public IEnumerable<ISubReportDataSource> SubReportDataSources { get; set; }

        public bool IsReportRunnerExecution { get; set; }

        public ControlSettings ControlSettings { get; set; }
    }
}
