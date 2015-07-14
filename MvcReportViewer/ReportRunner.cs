using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Collections;

namespace MvcReportViewer
{
    internal class ReportRunner
    {
        private readonly ReportViewerParameters _viewerParameters = new ReportViewerParameters
            {
                ReportServerUrl = ConfigurationManager.AppSettings[WebConfigSettings.Server],
                Username = ConfigurationManager.AppSettings[WebConfigSettings.Username],
                Password = ConfigurationManager.AppSettings[WebConfigSettings.Password],
                IsReportRunnerExecution = true
            };

        private readonly ReportFormat _reportFormat;

        public ReportRunner(
            ReportFormat reportFormat,
            IReportLoader reportLoader,
            ProcessingMode mode,
            IEnumerable<IDataSource> localReportDataSources = null,
            IEnumerable<ISubReportDataSource> subReportDataSources = null)
            : this(reportFormat, 
                reportLoader,
                null, 
                mode, 
                localReportDataSources,
                subReportDataSources)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            IReportLoader reportLoader,
            IDictionary<string, object> reportParameters,
            ProcessingMode mode,
            IEnumerable<IDataSource> localReportDataSources = null,
            IEnumerable<ISubReportDataSource> subReportDataSources = null)
            : this(reportFormat, 
                reportLoader, 
                reportParameters != null ? reportParameters.ToList() : null,
                mode,
                localReportDataSources,
                subReportDataSources)
        {
        }

        public ReportRunner(
            ReportFormat reportFormat,
            IReportLoader reportLoader,
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            ProcessingMode mode,
            IEnumerable<IDataSource> reportDataSources = null,
            IEnumerable<ISubReportDataSource> subReportDataSources = null)
        {
            _reportFormat = reportFormat;

            _viewerParameters.ProcessingMode = mode;
            if (mode == ProcessingMode.Local && reportDataSources != null)
            {
                _viewerParameters.ReportDataSources = reportDataSources;
                _viewerParameters.SubReportDataSources = subReportDataSources;
            }

            _viewerParameters.ReportLoader = reportLoader;

            reportLoader.SetViewerParamerters(_viewerParameters);

            ParseParameters(reportParameters);
        }

        // The property is only used for unit-testing
        internal ReportViewerParameters ViewerParameters
        {
            get { return _viewerParameters; }
        }

        // The property is only used for unit-testing
        internal ReportFormat ReportFormat
        {
            get { return _reportFormat; }
        }

        public FileStreamResult Run()
        {
            Validate();

            ViewerParameters.ControlSettings = new ControlSettings();
            ViewerParameters.ControlSettings.EnableExternalImages = true;

            var reportViewer = new ReportViewer();
            reportViewer.Initialize(_viewerParameters);

            string mimeType;
            Stream output;

            if (_viewerParameters.ProcessingMode == ProcessingMode.Remote)
            {
                string extension;

                output = reportViewer.ServerReport.Render(
                    ReportFormat.ToString(),
                    "<DeviceInfo></DeviceInfo>",
                    null,
                    out mimeType,
                    out extension);
            }
            else
            {
                var localReport = reportViewer.LocalReport;

                localReport.SubreportProcessing += localReport_SubreportProcessing;
                
                if (_viewerParameters.ReportDataSources != null)
                {
                    foreach(var dataSource in _viewerParameters.ReportDataSources)
                    {
                        ReportDataSource reportDataSource = dataSource.CreateDataSource();

                        localReport.DataSources.Add(reportDataSource);
                    }
                }

                Warning[] warnings;
                string[] streamids;
                string encoding;
                string extension;

                var report = localReport.Render(
                    ReportFormat.ToString(), 
                    null,
                    out mimeType,
                    out encoding,
                    out extension,
                    out streamids,
                    out warnings);

                output = new MemoryStream(report);
            }

            return new FileStreamResult(output, mimeType);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var subReportDataSources = _viewerParameters.SubReportDataSources.Where(sr => sr.ReportPath == e.ReportPath);
            
            foreach(var subReportDataSource in subReportDataSources)
            {
                var reportDataSource = subReportDataSource.CreateDataSource(e.Parameters);

                e.DataSources.Add(reportDataSource);
            }
        }

        private void ParseParameters(IEnumerable<KeyValuePair<string, object>> reportParameters)
        {
            if (reportParameters == null)
            {
                return;
            }

            foreach (var reportParameter in reportParameters)
            {
                var parameterName = reportParameter.Key;
                if (_viewerParameters.ReportParameters.ContainsKey(parameterName))
                {
                    _viewerParameters.ReportParameters[parameterName].Values.Add(reportParameter.Value.ToString());
                }
                else
                {
                    var parameter = new ReportParameter(parameterName);
                    parameter.Values.Add(reportParameter.Value.ToString());
                    _viewerParameters.ReportParameters.Add(parameterName, parameter);
                }
            }
        }

        private void Validate()
        {
            if (_viewerParameters.ProcessingMode == ProcessingMode.Remote && string.IsNullOrEmpty(_viewerParameters.ReportServerUrl))
            {
                throw new MvcReportViewerException("Report Server is not specified.");
            }

            if (string.IsNullOrEmpty(_viewerParameters.MainReportResourceName) 
                && string.IsNullOrEmpty(_viewerParameters.ReportPath))
            {
                throw new MvcReportViewerException("Report is not specified.");
            }
        }
    }
}
