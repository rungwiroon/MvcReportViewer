﻿using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer
{
    /// <summary>
    /// Interface which stores data sources for Local Report (.rdlc)
    /// </summary>
    public interface ILocalReportDataSourceProvider
    {
        /// <summary>
        /// Saves report custom data source, e.g. SQL query, etc.
        /// </summary>
        /// <param name="reportControlId">Internal MvcReportViewer instance ID.</param>
        /// <param name="dataSourceName">Data Source Name.</param>
        /// <param name="dataSource">Custom Data Source.</param>
        void Add<T>(Guid reportControlId, string dataSourceName, T dataSource);

        /// <summary>
        /// Saves report data source.
        /// </summary>
        /// <param name="reportControlId">Internal MvcReportViewer instance ID.</param>
        /// <param name="dataSource">Report Data Source.</param>
        void Add(Guid reportControlId, IDataSource dataSource);

        void Add(Guid reportControlId, ISubReportDataSource dataSources);

        /// <summary>
        /// Saves report data source.
        /// </summary>
        /// <param name="reportControlId">Internal MvcReportViewer instance ID.</param>
        /// <param name="dataSource">Report Data Source.</param>
        void Add(Guid reportControlId, IEnumerable<ISubReportDataSource> dataSources);

        /// <summary>
        /// Gets data sources used for the report.
        /// </summary>
        /// <param name="reportControlId">Internal MvcReportViewer instance ID.</param>
        /// <returns>The list of data sources.</returns>
        IEnumerable<ReportDataSource> Get(Guid reportControlId);

        IEnumerable<ISubReportDataSource> GetSubReport(Guid reportControlId, string reportPath);
    }
}
