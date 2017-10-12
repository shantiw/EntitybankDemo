using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using System.Xml.Linq;
using XData.Data.Diagnostics;

namespace XData.Web.Http.Filters
{
    public class HttpExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Log.Error(actionExecutedContext.Exception);

            //
            if (actionExecutedContext.Exception is ValidationException)
            {
                ValidationException validationException = actionExecutedContext.Exception as ValidationException;
                if (validationException.Value != null && validationException.Value is ICollection<ValidationResult>[])
                {
                    ICollection<ValidationResult>[] validationResultCollections = validationException.Value as ICollection<ValidationResult>[];
                    IEnumerable<ValidationResult> validationResults = GetValidationResults(validationResultCollections);

                    string mediaType = actionExecutedContext.Request.GetResponseMediaType();
                    switch (mediaType)
                    {
                        case "application/json":
                            {
                                // [{"MemberNames":["Path1","Path2"],"ErrorMessage":"ErrorMessage1"},{"MemberNames":["Path3"],"ErrorMessage":"ErrorMessage2"},...]
                                ObjectContent content = new ObjectContent<IEnumerable<ValidationResult>>(validationResults, new JsonMediaTypeFormatter(), "application/json");
                                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = content };
                            }
                            break;
                        case "application/xml":
                            {
                                XElement element = ToXml(validationResults);
                                ObjectContent content = new ObjectContent<XElement>(element, new XmlMediaTypeFormatter(), "application/xml");
                                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = content };
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            //List<ValidationResult> validationResults1 = new List<ValidationResult>();
            //validationResults1.Add(new ValidationResult("Validation1", new string[] { "Path1", "Path2" }));
            //validationResults1.Add(new ValidationResult("Validation2", new string[] { "Path1", "Path2" }));
            //ObjectContent content1 = new ObjectContent<IEnumerable<ValidationResult>>(validationResults1, new JsonMediaTypeFormatter(), "application/json");
            //actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = content1 };

            base.OnException(actionExecutedContext);
        }

        //<Error>
        //  <ValidationResult>
        //    <MemberName>Path1</MemberName>
        //    <MemberName>Path2</MemberName>
        //    ...
        //    <ErrorMessage>ErrorMessage</ErrorMessage>
        //  </ValidationResult>
        //  <ValidationResult>
        //    ...
        //  </ValidationResult>
        //</Error>
        private static XElement ToXml(IEnumerable<ValidationResult> validationResults)
        {
            XElement error = new XElement("Error");
            foreach (ValidationResult validationResult in validationResults)
            {
                XElement xValidationResult = new XElement("ValidationResult");

                foreach (string memberName in validationResult.MemberNames)
                {
                    xValidationResult.Add(new XElement("MemberName", memberName));
                }

                xValidationResult.SetElementValue("ErrorMessage", validationResult.ErrorMessage);

                error.Add(xValidationResult);
            }
            return error;
        }

        private static IEnumerable<ValidationResult> GetValidationResults(ICollection<ValidationResult>[] validationResultCollections)
        {
            List<ValidationResult> list = new List<ValidationResult>();
            for (int i = 0; i < validationResultCollections.Length; i++)
            {
                list.AddRange(validationResultCollections[i]);
            }
            return list;
        }


    }
}
