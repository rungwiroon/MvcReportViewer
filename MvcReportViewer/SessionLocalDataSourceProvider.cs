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

        public void Add(Guid reportControlId, IDataSource dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<IDataSource>;
            dataSources = dataSources ?? new List<IDataSource>();
            dataSources.Add(dataSource);

            _session[key] = dataSources;
        }

        public IEnumerable<ReportDataSource> Get(Guid reportControlId)
        {
            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as List<IDataSource>;
            var dataSourceList = new List<ReportDataSource>();

            foreach(var dataSource in dataSources)
            {
                dataSourceList.Add(dataSource.CreateDataSource());
            }

            return dataSourceList;
        }
    }
}
