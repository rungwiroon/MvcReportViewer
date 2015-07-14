using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcReportViewer
{
    public interface ISubReportDataSource 
    {
        ReportDataSource CreateDataSource(ReportParameterInfoCollection reportParameters);

        string ReportPath { get; }
    }

    public class SubReportEnumerableDataSource<T> : ISubReportDataSource
    {
        private GenericEnumerableDataSource<T> _genericDataSource;
        private Func<ReportParameterInfoCollection, IEnumerable<T>, IEnumerable<T>> _dataSourceFunc;

        public string ReportPath { get; private set; }

        public SubReportEnumerableDataSource(string reportPath,
            GenericEnumerableDataSource<T> dataSource,
            Func<ReportParameterInfoCollection,  IEnumerable<T>,  IEnumerable<T>> dataSourceFunc)
        {
            ReportPath = reportPath;

            _genericDataSource = dataSource;
            _dataSourceFunc = dataSourceFunc;
        }

        public ReportDataSource CreateDataSource(ReportParameterInfoCollection reportParameters)
        {
            return new ReportDataSource(_genericDataSource.DataSourceName,
                _dataSourceFunc.Invoke(reportParameters, _genericDataSource.Data));
        }
    }
}
