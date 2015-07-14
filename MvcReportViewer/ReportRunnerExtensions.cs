using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Collections;

namespace MvcReportViewer
{
    public static class ReportRunnerExtensions
    {
        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="mode">Report processing mode: remote or local.</param>
        /// <param name="localReportDataSources">Local report data sources</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller, 
            ReportFormat reportFormat, 
            IReportLoader reportLoader,
            ProcessingMode mode,
            IEnumerable<IDataSource> reportDataSources = null,
            IEnumerable<ISubReportDataSource> subReportDataSources = null)
        {
            var reportRunner = new ReportRunner(
                reportFormat,
                reportLoader,
                mode,
                reportDataSources,
                subReportDataSources);
            return reportRunner.Run();
        }

        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="mode">Report processing mode: remote or local.</param>
        /// <param name="localReportDataSources">Local report data sources</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller,
            ReportFormat reportFormat,
            IReportLoader reportLoader,
            object reportParameters,
            ProcessingMode mode,
            IEnumerable<IDataSource> reportDataSources = null,
            IEnumerable<ISubReportDataSource> subReportDataSources = null)
        {
            var reportRunner = new ReportRunner(
                reportFormat, 
                reportLoader, 
                HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters),
                mode,
                reportDataSources,
                subReportDataSources);

            return reportRunner.Run();
        }

        /// <summary>
        /// Creates a FileContentResult object by using Report Viewer Web Control.
        /// </summary>
        /// <param name="controller">The Controller instance that this method extends.</param>
        /// <param name="reportFormat">Report Viewer Web Control supported format (Excel, Word, PDF or Image)</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="mode">Report processing mode: remote or local.</param>
        /// <param name="localReportDataSources">Local report data sources</param>
        /// <returns>The file-content result object.</returns>
        public static FileStreamResult Report(
            this Controller controller,
            ReportFormat reportFormat,
            IReportLoader reportLoader,
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            ProcessingMode mode,
            IEnumerable<IDataSource> reportDataSources = null,
            IEnumerable<ISubReportDataSource> subReportDataSources = null)
        {
            var reportRunner = new ReportRunner(
                reportFormat,
                reportLoader,
                reportParameters,
                mode,
                reportDataSources,
                subReportDataSources);

            return reportRunner.Run();
        }
    }
}
