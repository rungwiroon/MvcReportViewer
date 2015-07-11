﻿using System;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Web;

namespace MvcReportViewer
{
    internal class ReportViewerParametersParser
    {
        public ReportViewerParameters Parse(NameValueCollection queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException("queryString");
            }

            var isEncrypted = CheckEncryption(ref queryString);

            var settinsManager = new ControlSettingsManager(isEncrypted);

            var parameters = InitializeDefaults();
            ResetDefaultCredentials(queryString, parameters);
            parameters.ControlSettings = settinsManager.Deserialize(queryString);

            foreach (var key in queryString.AllKeys)
            {
                var urlParam = queryString[key];
                if (key.EqualsIgnoreCase(UriParameters.ReportPath))
                {
                    parameters.ReportPath = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.ReportAssemblyName))
                {
                    parameters.ReportAssembly = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.ReportEmbeddedName))
                {
                    parameters.ReportEmbeddedResource = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.ControlId))
                {
                    var parameter = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                    parameters.ControlId = Guid.Parse(parameter);
                }
                else if (key.EqualsIgnoreCase(UriParameters.ProcessingMode))
                {
                    var parameter = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                    parameters.ProcessingMode = (ProcessingMode)Enum.Parse(typeof(ProcessingMode), parameter);
                }
                else if (key.EqualsIgnoreCase(UriParameters.ReportServerUrl))
                {
                    parameters.ReportServerUrl = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.Username))
                {
                    parameters.Username = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (key.EqualsIgnoreCase(UriParameters.Password))
                {
                    parameters.Password = isEncrypted ? SecurityUtil.Decrypt(urlParam) : urlParam;
                }
                else if (!settinsManager.IsControlSetting(key))
                {
                    var values = queryString.GetValues(key);
                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            var realValue = isEncrypted ? SecurityUtil.Decrypt(value) : value;
                            var parsedKey = ParseKey(key);
                            var realKey = parsedKey.Item1;
                            var isVisible = parsedKey.Item2;

                            if (parameters.ReportParameters.ContainsKey(realKey))
                            {
                                parameters.ReportParameters[realKey].Values.Add(realValue);
                            }
                            else
                            {
                                var reportParameter = new ReportParameter(realKey);
                                reportParameter.Visible = isVisible;
                                reportParameter.Values.Add(realValue);
                                parameters.ReportParameters.Add(realKey, reportParameter);
                            }
                        }
                    }
                }
            }

            if (parameters.ProcessingMode == ProcessingMode.Remote 
                && string.IsNullOrEmpty(parameters.ReportServerUrl))
            {
                throw new MvcReportViewerException("Report Server is not specified.");
            }

            if (string.IsNullOrEmpty(parameters.ReportEmbeddedResource) && string.IsNullOrEmpty(parameters.ReportPath))
            {
                throw new MvcReportViewerException("Report is not specified.");
            }

            return parameters;
        }

        private static Tuple<string, bool> ParseKey(string key)
        {
            if (!key.Contains(MvcReportViewerIframe.VisibilitySeparator))
            {
                return new Tuple<string, bool>(key, true);
            }

            var parts = key.Split(new[] { MvcReportViewerIframe.VisibilitySeparator }, StringSplitOptions.RemoveEmptyEntries);
            bool isVisible;
            if (parts.Length != 2 || !bool.TryParse(parts[1], out isVisible))
            {
                return new Tuple<string, bool>(key, true);
            }

            return new Tuple<string, bool>(parts[0], isVisible);
        }

        private static bool CheckEncryption(ref NameValueCollection source)
        {
            bool isEncrypted;
            var encryptParametesConfig = ConfigurationManager.AppSettings[WebConfigSettings.EncryptParameters];
            if (!bool.TryParse(encryptParametesConfig, out isEncrypted))
            {
                isEncrypted = false;
            }

            // each parameter is encrypted when POST method is used
            if (string.Compare(HttpContext.Current.Request.HttpMethod, "POST", true) == 0)
            {
                return isEncrypted;
            }

            if (!isEncrypted)
            {
                return isEncrypted;
            }

            var encrypted = source[UriParameters.Encrypted];
            var decrypted = SecurityUtil.Decrypt(encrypted);
            isEncrypted = false;
            source = HttpUtility.ParseQueryString(decrypted);
            return isEncrypted;
        }

        private static void ResetDefaultCredentials(NameValueCollection queryString, ReportViewerParameters parameters)
        {
            if (queryString.ContainsKeyIgnoreCase(UriParameters.Username) ||
                queryString.ContainsKeyIgnoreCase(UriParameters.Password))
            {
                parameters.Username = string.Empty;
                parameters.Password = string.Empty;
            }
        }

        private ReportViewerParameters InitializeDefaults()
        {
            var isAzureSSRS = ConfigurationManager.AppSettings[WebConfigSettings.IsAzureSSRS];
            bool isAzureSSRSValue;
            if (string.IsNullOrEmpty(isAzureSSRS) || !bool.TryParse(isAzureSSRS, out isAzureSSRSValue))
            {
                isAzureSSRSValue = false;
            }
            
            var parameters = new ReportViewerParameters
                {
                    ReportServerUrl = ConfigurationManager.AppSettings[WebConfigSettings.Server],
                    Username = ConfigurationManager.AppSettings[WebConfigSettings.Username],
                    Password = ConfigurationManager.AppSettings[WebConfigSettings.Password],
                    IsAzureSSRS = isAzureSSRSValue
                };

            return parameters;
        }
    }
}
