using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Xml.Linq;
using XData.IO.Compression;

namespace XData.Web.Http.Filters
{
    public class CompressionActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Compress(actionExecutedContext.Response);

            base.OnActionExecuted(actionExecutedContext);
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            Compress(actionExecutedContext.Response);

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        // Compress XElement
        private static void Compress(HttpResponseMessage response)
        {
            if (response.Content is ObjectContent)
            {
                ObjectContent objectContent = response.Content as ObjectContent;
                if (objectContent.Value is XElement)
                {
                    XElement element = objectContent.Value as XElement;
                    byte[] buffer = Encoding.UTF8.GetBytes(element.ToString());

                    int compressionMinSize = 2048;
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("compressionMinSize"))
                    {
                        compressionMinSize = int.Parse(ConfigurationManager.AppSettings["compressionMinSize"]);
                    }
                    if (buffer.Length > compressionMinSize)
                    {
                        byte[] bytes = new GZipCompressor().Compress(buffer);
                        element = new XElement("GZip", Convert.ToBase64String(bytes));
                        objectContent.Value = element;
                    }
                }
            }
        }


    }
}