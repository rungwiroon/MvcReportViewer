using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MvcReportViewer
{
    public interface IGenericDataSource
    {
        ReportDataSource CreateDataSource(string key);
    }

    public class GenericDataSource<T> : IGenericDataSource
    {
        public T Data { get; private set; }

        public GenericDataSource(T data)
        {
            Data = data;
        }

        public ReportDataSource CreateDataSource(string key)
        {
            return new ReportDataSource(key, Data);
        }
    }

    public class DataTableDataSource : GenericDataSource<DataTable>
    {
        public DataTableDataSource(DataTable data)
            : base(data)
        {

        }
    }

    public class EnumerableDataSource : GenericDataSource<IEnumerable>
    {
        public EnumerableDataSource(IEnumerable data)
            : base(data)
        {

        }
    }

    public class GenericEnumerableDataSource<T> : GenericDataSource<IEnumerable<T>>
    {
        public GenericEnumerableDataSource(IEnumerable<T> data)
            : base(data)
        {

        }
    }
}
