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

        string ResourceName { get; }
    }

    public class SubReportEnumerableDataSource<T> : ISubReportDataSource
    {
        private string _dataSourceName;
        private GenericEnumerableDataSource<T> _genericDataSource;
        private Func<ReportParameterInfoCollection, IEnumerable<T>, IEnumerable<T>> _dataSourceFunc;

        public string ResourceName { get; private set; }

        public SubReportEnumerableDataSource(string resourceName,
            string dataSourceName,
            GenericEnumerableDataSource<T> dataSource,
            Func<ReportParameterInfoCollection,  IEnumerable<T>,  IEnumerable<T>> dataSourceFunc)
        {
            ResourceName = resourceName;

            _dataSourceName = dataSourceName;
            _genericDataSource = dataSource;
            _dataSourceFunc = dataSourceFunc;
        }

        public ReportDataSource CreateDataSource(ReportParameterInfoCollection reportParameters)
        {
            return new ReportDataSource(_dataSourceName, _dataSourceFunc.Invoke(reportParameters, _genericDataSource.Data));
        }
    }
}
