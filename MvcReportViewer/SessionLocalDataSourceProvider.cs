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
            var sessionDataSource = _session[key] as SessionDataSourceStore;
            sessionDataSource = sessionDataSource ?? new SessionDataSourceStore();
            sessionDataSource.MainReportDataSources.Add(dataSource);

            _session[key] = sessionDataSource;
        }

        public void Add(Guid reportControlId, ISubReportDataSource dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            var key = GetSessionValueKey(reportControlId);
            var sessionDataSource = _session[key] as SessionDataSourceStore;
            sessionDataSource = sessionDataSource ?? new SessionDataSourceStore();
            sessionDataSource.SubReportDataSources.Add(dataSource);

            _session[key] = sessionDataSource;
        }

        public void Add(Guid reportControlId, IEnumerable<ISubReportDataSource> dataSources)
        {
            if (dataSources == null)
            {
                return;
            }

            var key = GetSessionValueKey(reportControlId);
            var sessionDataSource = _session[key] as SessionDataSourceStore;
            sessionDataSource = sessionDataSource ?? new SessionDataSourceStore();
            sessionDataSource.SubReportDataSources.AddRange(dataSources);

            _session[key] = sessionDataSource;
        }

        public IEnumerable<ReportDataSource> Get(Guid reportControlId)
        {
            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as SessionDataSourceStore;
            var dataSourceList = new List<ReportDataSource>();

            foreach(var dataSource in dataSources.MainReportDataSources)
            {
                dataSourceList.Add(dataSource.CreateDataSource());
            }

            return dataSourceList;
        }
        
        public IEnumerable<ISubReportDataSource> GetSubReport(Guid reportControlId, string reportPath)
        {
            var key = GetSessionValueKey(reportControlId);
            var dataSources = _session[key] as SessionDataSourceStore;

            return dataSources.SubReportDataSources.Where(sr => sr.ReportPath == reportPath).ToArray();
        }
    }

    internal class SessionDataSourceStore
    {
        public List<IDataSource> MainReportDataSources { get; private set; }

        public List<ISubReportDataSource> SubReportDataSources { get; private set; }

        public SessionDataSourceStore()
        {
            MainReportDataSources = new List<IDataSource>();
            SubReportDataSources = new List<ISubReportDataSource>();
        }
    }
}
