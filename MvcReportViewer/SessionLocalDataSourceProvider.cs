using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Microsoft.Reporting.WebForms;
using System.Collections;

namespace MvcReportViewer
{
    public class SessionLocalDataSourceProvider : BaseLocalDataSourceProvider, ILocalReportDataSourceProvider
    {
        private readonly HttpSessionState _session = HttpContext.Current.Session;

        public void Add<T>(Guid reportControlId, string dataSourceName, T dataSource)
        {
            throw new NotSupportedException();
        }

        public void Add(Guid reportControlId, ReportDataSource dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<ReportDataSourceWrapper>;
            dataSources = dataSources ?? new List<ReportDataSourceWrapper>();

            if(dataSource.Value is DataTable)
            {
                dataSources.Add(
                new ReportDataSourceWrapper
                {
                    Name = dataSource.Name,
                    Value = (DataTable)dataSource.Value
                });
            }
            
            else if(dataSource.Value is IEnumerable)
            {
                dataSources.Add(
                new ReportDataSourceWrapper2
                {
                    Name = dataSource.Name,
                    Value = (IEnumerable)dataSource.Value
                });
            }

            _session[key] = dataSources;
        }

        public IEnumerable<ReportDataSource> Get(Guid reportControlId)
        {
            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<ReportDataSourceWrapper>;
            var dataSourceList = new List<ReportDataSource>();

            foreach(var dataSource in dataSources)
            {
                if(dataSource is ReportDataSourceWrapper2)
                    dataSourceList.Add(new ReportDataSource(dataSource.Name, ((ReportDataSourceWrapper2)dataSource).Value));
                else
                    dataSourceList.Add(new ReportDataSource(dataSource.Name, dataSource.Value));
            }

            return dataSourceList;
        }

        [Serializable]
        private class ReportDataSourceWrapper
        {
            public string Name { get; set; }

            public DataTable Value { get; set; }
        }

        [Serializable]
        private class ReportDataSourceWrapper2 : ReportDataSourceWrapper
        {
            public new IEnumerable Value { get; set; }
        }
    }
}
