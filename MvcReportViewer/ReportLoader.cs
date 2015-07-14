using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MvcReportViewer
{
    public interface IReportLoader
    {
        void SetViewerParamerters(ReportViewerParameters viwerParameters);
        void LoadReport(ReportViewer reportViewer);
    }

    public class LocalReportLoader : IReportLoader
    {
        public string ReportPath { get; private set; }

        public LocalReportLoader(string reportPath)
        {
            ReportPath = reportPath;
        }

        public void LoadReport(ReportViewer reportViewer)
        {
            var localReport = reportViewer.LocalReport;
            localReport.ReportPath = ReportPath;
        }

        public void SetViewerParamerters(ReportViewerParameters viewerParameters)
        {
            viewerParameters.ReportPath = ReportPath;
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

        public LocalReportAssemblyResourceLoader(string assemblyName, string mainReportEmbeddedResourceName, IEnumerable<SubReportResourceName> subReportEmbeddedResourceNames = null)
        {
            AssemblyName = assemblyName;
            MainReportResourceName = mainReportEmbeddedResourceName;
            SubReportResourceNames = subReportEmbeddedResourceNames;
        }

        public void LoadReport(ReportViewer reportViewer)
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

        public void SetViewerParamerters(ReportViewerParameters viewerParameters)
        {
            viewerParameters.ReportAssemblyName = AssemblyName;
            viewerParameters.MainReportResourceName = MainReportResourceName;
            viewerParameters.SubReportResourceNames = "Not implement!!!";
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

        public abstract void LoadReport(ReportViewer reportViewer);

        public void SetViewerParamerters(ReportViewerParameters viewerParameters)
        {
            viewerParameters.ReportPath = ReportPath;
            viewerParameters.ReportServerUrl = ReportServerUrl ?? viewerParameters.ReportServerUrl;
            if (Username != null || Password != null)
            {
                viewerParameters.Username = Username;
                viewerParameters.Password = Password;
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

        public override void LoadReport(ReportViewer reportViewer)
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

        public override void LoadReport(ReportViewer reportViewer)
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
