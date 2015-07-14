﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcReportViewer
{
    /// <summary>
    /// HTML helpers for MvcReportViewer.
    /// </summary>
    public static class MvcReportViewerExtensions
    {
        /// <summary>
        /// Returns an HTML <b>iframe</b> rendering ASP.NET ReportViewer control with Remote Processing Mode.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An HTML <b>iframe</b> element.</returns>
        public static MvcReportViewerIframe MvcReportViewer(
            this HtmlHelper helper,
            IReportLoader reportLoader,
            object htmlAttributes)
        {
            return new MvcReportViewerIframe(
                reportLoader, 
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an HTML <b>iframe</b> rendering ASP.NET ReportViewer control with Remote Processing Mode.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An HTML <b>iframe</b> element.</returns>
        public static MvcReportViewerIframe MvcReportViewer(
            this HtmlHelper helper,
            IReportLoader reportLoader,
            object reportParameters, 
            object htmlAttributes)
        {
            return new MvcReportViewerIframe(
                reportLoader,
                HtmlHelper.AnonymousObjectToHtmlAttributes(reportParameters),
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an HTML <b>iframe</b> rendering ASP.NET ReportViewer control with Remote Processing Mode.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <returns>An HTML <b>iframe</b> element.</returns>
        public static MvcReportViewerIframe MvcReportViewer(
            this HtmlHelper helper,
            IReportLoader reportLoader,
            IEnumerable<KeyValuePair<string, object>> reportParameters,
            object htmlAttributes)
        {
            return new MvcReportViewerIframe(
                reportLoader,
                reportParameters,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an HTML <b>iframe</b> rendering ASP.NET ReportViewer control with Remote Processing Mode.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <param name="reportServerUrl">The URL for the report server.</param>
        /// <param name="username">The report server username.</param>
        /// <param name="password">The report server password.</param>
        /// <param name="reportParameters">The report parameter properties for the report.</param>
        /// <param name="controlSettings">The Report Viewer control's UI settings.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
        /// <param name="method">Method for sending parameters to the iframe, either GET or POST.</param>
        /// <returns>An HTML <b>iframe</b> element.</returns>
        public static MvcReportViewerIframe MvcReportViewer(
            this HtmlHelper helper,
            IReportLoader reportLoader,
            IEnumerable<KeyValuePair<string, object>> reportParameters = null,
            ControlSettings controlSettings = null,
            object htmlAttributes = null,
            FormMethod method = FormMethod.Get)
        {
            return new MvcReportViewerIframe(
                reportLoader,
                reportParameters,
                controlSettings,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes),
                method);
        }

        /// <summary>
        /// Returns fluent interface to HTML <b>iframe</b> rendering ASP.NET ReportViewer control.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportLoader">The path to the report on the server.</param>
        /// <returns>Fluent interface HTML <b>iframe</b> element.</returns>
        public static IMvcReportViewerOptions MvcReportViewerFluent(
            this HtmlHelper helper,
            IReportLoader reportLoader)
        {
            return new MvcReportViewerIframe(reportLoader);
        }

        /// <summary>
        /// Returns fluent interface to HTML <b>iframe</b> rendering ASP.NET ReportViewer control. 
        /// IMPORTANT: Unit-tests only!
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="reportPath">The path to the report on the server.</param>
        /// <returns>Fluent interface HTML <b>iframe</b> element.</returns>
        internal static IMvcReportViewerOptions MvcReportViewerFluent(
            this HtmlHelper helper,
            IReportLoader reportLoader,
            Guid controlId)
        {
            var iframe = new MvcReportViewerIframe(reportLoader);
            iframe.ControlId = controlId;
            return iframe;
        }
    }
}
