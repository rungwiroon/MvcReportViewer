using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System;
using System.Data;
using System.Collections;

namespace MvcReportViewer
{
    internal class ReportViewerParameters
    {
        public ReportViewerParameters()
        {
            ReportParameters = new Dictionary<string, ReportParameter>();
        }

        public string ReportServerUrl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ReportPath { get; set; }

        public string ReportAssembly { get; set; }

        public string ReportEmbeddedResource { get; set; }

        public Guid? ControlId { get; set; }

        public ProcessingMode ProcessingMode { get; set; }

        public bool IsAzureSSRS { get; set; }

        public IDictionary<string, ReportParameter> ReportParameters { get; set; }

        public IDictionary<string, IGenericDataSource> LocalReportDataSources { get; set; }

        public IDictionary<string, ISubReportDataSource> SubReportDataSources { get; set; }

        public bool IsReportRunnerExecution { get; set; }

        public ControlSettings ControlSettings { get; set; }
    }
}
