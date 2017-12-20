using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace System.Xml.Linq
{
    public static class ErrorHelper
    {
        public static string GetErrorMessage(this XElement error)
        {
            string[] messages = GetErrorMessages(error);
            if (messages.Length < 2) return messages.FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            foreach (string message in messages)
            {
                sb.AppendLine(message);
            }
            return sb.ToString();
        }

        public static string[] GetErrorMessages(this XElement error)
        {
            if (error.Name != "Error") return new string[0];

            IEnumerable<XElement> xValidationResults = error.Elements("ValidationResult");
            if (xValidationResults.Any())
            {
                List<string> messages = new List<string>();
                foreach (XElement xValidationResult in xValidationResults)
                {
                    messages.Add(xValidationResult.Element("ErrorMessage").Value);
                }
                return messages.ToArray();
            }
            else
            {
                string exceptionMessage = error.Element("ExceptionMessage")?.Value;
                if (exceptionMessage == null)
                {
                    string message = error.Element("Message")?.Value;
                    if (message != null) return new string[] { message };
                    return new string[0];
                }
                else
                {
                    return new string[] { exceptionMessage };
                }
            }
        }


    }
}
