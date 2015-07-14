using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MvcReportViewer
{
    public interface IDataSource
    {
        ReportDataSource CreateDataSource();
    }

    [Serializable]
    public class GenericDataSource<T> : IDataSource
    {
        public string DataSourceName { get; private set; }
        public T Data { get; private set; }

        public GenericDataSource(string dataSourceName, T data)
        {
            DataSourceName = dataSourceName;
            Data = data;
        }

        public ReportDataSource CreateDataSource()
        {
            return new ReportDataSource(DataSourceName, Data);
        }
    }

    [Serializable]
    public class DataTableDataSource : GenericDataSource<DataTable>
    {
        public DataTableDataSource(string dataSourceName, DataTable data)
            : base(dataSourceName, data)
        {

        }
    }

    [Serializable]
    public class EnumerableDataSource : GenericDataSource<IEnumerable>
    {
        public EnumerableDataSource(string dataSourceName, IEnumerable data)
            : base(dataSourceName, data)
        {

        }
    }

    [Serializable]
    public class GenericEnumerableDataSource<T> : GenericDataSource<IEnumerable<T>>
    {
        public GenericEnumerableDataSource(string dataSourceName, IEnumerable<T> data)
            : base(dataSourceName, data)
        {

        }
    }

    
}
