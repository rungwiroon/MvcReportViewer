using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcReportViewer
{
    public class HtmlFormFieldBuilder
    {
        private StringBuilder html = new StringBuilder();
        private bool _encryptParameters = false;

        public HtmlFormFieldBuilder(bool encryptParameters)
        {
            _encryptParameters = encryptParameters;
        }

        public void AddField<T>(string name, T value)
        {
            html.Append(CreateHiddenField(name, value));
        }

        private string CreateHiddenField<T>(string name, T value)
        {
            var tag = new TagBuilder("input");
            tag.MergeAttribute("type", "hidden");
            tag.MergeAttribute("name", name);

            var strValue = value.ToString();
            if (_encryptParameters)
            {
                strValue = SecurityUtil.Encrypt(strValue);
            }

            tag.MergeAttribute("value", strValue);

            return tag.ToString();
        }
    }
}
