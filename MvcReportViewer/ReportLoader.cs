using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MvcReportViewer
{
    public enum ReportLoaderType
    {
        LocalReportLoader = 1,
        LocalReportAssemblyResourceLoader = 2,
        RemoteServerLoader = 3,
        RemoteAzureLoader = 4
    }

    public interface IReportLoader
    {
        void LoadReportTo(ReportViewer reportViewer);

        void BuildViewerFormFields(HtmlFormFieldBuilder html);

        void BuildViewerUri(NameValueCollection query);
    }

    public class LocalReportLoader : IReportLoader
    {
        public string ReportPath { get; private set; }

        public LocalReportLoader(string reportPath)
        {
            ReportPath = reportPath;
        }

        public LocalReportLoader(NameValueCollection queryString, bool isEncrypted)
        {
            var urlParam = queryString[UriParameters.ReportPath];

            ReportPath = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
        }

        public void LoadReportTo(ReportViewer reportViewer)
        {
            var localReport = reportViewer.LocalReport;
            localReport.ReportPath = ReportPath;
        }

        public void BuildViewerFormFields(HtmlFormFieldBuilder html)
        {
            html.AddField(UriParameters.ReportType, (int)ReportLoaderType.LocalReportLoader);
            html.AddField(UriParameters.ReportPath, ReportPath);
        }

        public void BuildViewerUri(NameValueCollection query)
        {
            query[UriParameters.ReportType] = ((int)ReportLoaderType.LocalReportLoader).ToString();
            query[UriParameters.ReportPath] = ReportPath;
        }
    }

    public class SubReportResourceName
    {
        public string ReportName { get; private set; }
        public string ResourceName { get; private set; }

        public SubReportResourceName(string reportName, string resourceName)
        {
            ReportName = reportName;
            ResourceName = resourceName;
        }
    }

    public class LocalReportAssemblyResourceLoader : IReportLoader
    {
        public string AssemblyName { get; private set; }

        public string MainReportResourceName { get; private set; }

        public IEnumerable<SubReportResourceName> SubReportResourceNames { get; private set; }

        //private string _subReportNames;

        public LocalReportAssemblyResourceLoader(string assemblyName, string mainReportEmbeddedResourceName, IEnumerable<SubReportResourceName> subReportEmbeddedResourceNames = null)
        {
            AssemblyName = assemblyName;
            MainReportResourceName = mainReportEmbeddedResourceName;
            SubReportResourceNames = subReportEmbeddedResourceNames;
        }

        public LocalReportAssemblyResourceLoader(NameValueCollection queryString, bool isEncrypted)
        {
            var urlParam1 = queryString[UriParameters.ReportAssemblyName];
            AssemblyName = isEncrypted ? SecurityUtil.Decrypt(urlParam1) : urlParam1;

            var urlParam2 = queryString[UriParameters.ReportResourceName];
            MainReportResourceName = isEncrypted ? SecurityUtil.Decrypt(urlParam2) : urlParam2;

            var subReportValues = queryString.GetValues(UriParameters.SubReportResourceNames);

            if (subReportValues == null || !subReportValues.Any()) return;

            var subReportList = new List<SubReportResourceName>();

            foreach(var subReportResourceName in subReportValues)
            {
                var sr = subReportResourceName.Split(':');
                var reportName = sr[0];
                var resourceName = sr[1];

                subReportList.Add(new SubReportResourceName(reportName, resourceName));
            }

            SubReportResourceNames = subReportList;
        }

        public void LoadReportTo(ReportViewer reportViewer)
        {
            var localReport = reportViewer.LocalReport;

            Assembly assembly = Assembly.Load(AssemblyName);
            Stream stream = assembly.GetManifestResourceStream(MainReportResourceName);
            localReport.LoadReportDefinition(stream);

            if (SubReportResourceNames == null) return;

            foreach (var subReport in SubReportResourceNames)
            {
                Stream subReportStream = assembly.GetManifestResourceStream(subReport.ResourceName);
                localReport.LoadSubreportDefinition(subReport.ReportName, subReportStream);
            }
        }

        public void BuildViewerFormFields(HtmlFormFieldBuilder html)
        {
            html.AddField(UriParameters.ReportType, (int)ReportLoaderType.LocalReportAssemblyResourceLoader);
            html.AddField(UriParameters.ReportAssemblyName, AssemblyName);
            html.AddField(UriParameters.ReportResourceName, MainReportResourceName);

            if (SubReportResourceNames == null) return;

            foreach (var subReport in SubReportResourceNames)
                html.AddField(UriParameters.SubReportResourceNames, subReport.ReportName + ":" + subReport.ResourceName);
        }

        public void BuildViewerUri(NameValueCollection query)
        {
            query[UriParameters.ReportType] = ((int)ReportLoaderType.LocalReportAssemblyResourceLoader).ToString();
            query[UriParameters.ReportAssemblyName] = AssemblyName;
            query[UriParameters.ReportResourceName] = MainReportResourceName;
            
            if (SubReportResourceNames == null) return;

            foreach (var subReport in SubReportResourceNames)
                query.Add(UriParameters.SubReportResourceNames, subReport.ReportName + ":" + subReport.ResourceName);
        }
    }

    public abstract class RemoteReportLoader : IReportLoader
    {
        public string ReportPath { get; private set; }
        public string ReportServerUrl { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public RemoteReportLoader(string reportPath, string serverUrl, string username, string password)
        {
            ReportServerUrl = serverUrl;
            ReportPath = reportPath;
            Username = username;
            Password = password;
        }

        public RemoteReportLoader(string reportPath)
        {
            ReportPath = reportPath;
            ReportServerUrl = ConfigurationManager.AppSettings[WebConfigSettings.Server];
            Username = ConfigurationManager.AppSettings[WebConfigSettings.Username];
            Password = ConfigurationManager.AppSettings[WebConfigSettings.Password];
        }

        public RemoteReportLoader(NameValueCollection queryString, bool isEncrypted)
        {
            var urlParam1 = queryString[UriParameters.ReportServerUrl];
            ReportServerUrl = isEncrypted ? SecurityUtil.Decrypt(urlParam1) : urlParam1;

            var urlParam2 = queryString[UriParameters.Username];
            Username = isEncrypted ? SecurityUtil.Decrypt(urlParam2) : urlParam2;

            var urlParam3 = queryString[UriParameters.Password];
            Password = isEncrypted ? SecurityUtil.Decrypt(urlParam3) : urlParam3;
        }

        public abstract void LoadReportTo(ReportViewer reportViewer);

        public virtual void BuildViewerFormFields(HtmlFormFieldBuilder html)
        {
            html.AddField(UriParameters.ReportServerUrl, ReportServerUrl);

            if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password))
            {
                html.AddField(UriParameters.Username, Username);
                html.AddField(UriParameters.Password, Password);
            }
        }

        public virtual void BuildViewerUri(NameValueCollection query)
        {
            query[UriParameters.ReportServerUrl] = ReportServerUrl;

            if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password))
            {
                query[UriParameters.Username] = Username;
                query[UriParameters.Password] = Password;
            }
        }        
    }

    public class RemoteServerLoader : RemoteReportLoader
    {
        public RemoteServerLoader(string reportPath, string serverUrl, string username, string password)
            : base(reportPath, serverUrl, username, password)
        {
            
        }

        public RemoteServerLoader(string reportPath)
            : base(reportPath)
        {
            
        }

        public override void LoadReportTo(ReportViewer reportViewer)
        {
            var serverReport = reportViewer.ServerReport;

            serverReport.ReportServerUrl = new Uri(ReportServerUrl);
            serverReport.ReportPath = ReportPath;

            if (!string.IsNullOrEmpty(Username))
            {
                var server = serverReport.ReportServerUrl.Host;
                serverReport.ReportServerCredentials = new AzureReportServerCredentials(
                    Username,
                    Password,
                    server);
            }
        }
    }

    public class RemoteAzureLoader : RemoteReportLoader
    {
        public RemoteAzureLoader(string serverUrl, string reportPath, string username, string password)
            : base(serverUrl, reportPath, username, password)
        {
            
        }

        public RemoteAzureLoader(string reportPath)
            : base(reportPath)
        {
            
        }

        public override void LoadReportTo(ReportViewer reportViewer)
        {
            var serverReport = reportViewer.ServerReport;

            serverReport.ReportServerUrl = new Uri(ReportServerUrl);
            serverReport.ReportPath = ReportPath;

            if (!string.IsNullOrEmpty(Username))
            {
                serverReport.ReportServerCredentials = new ReportServerCredentials(
                    Username,
                    Password);
            }
        }
    }
}
