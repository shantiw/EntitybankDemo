using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace XData.Windows.Components
{
    public class ErrorException :Exception
    {
        public XElement Error;
    }

    public static class ShowMessageBox
    {
        public static void Show(string message, DependencyObject view = null)
        {
            Window window = (view == null) ? null : Window.GetWindow(view);
            if (window == null)
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show(window, message, window.Title);
            }
        }

        public static void Show(Exception exception, DependencyObject view = null)
        {
            string message;
            if (exception is ErrorException)
            {
                ErrorException errorException = exception as ErrorException;
                string exceptionType = errorException.Error.Element("ExceptionType").Value;
                if (exceptionType == "XData.Data.Element.Validation.ElementValidationException")
                {
                    StringBuilder sb = new StringBuilder();
                    XElement validationErrors = errorException.Error.Element("ValidationErrors");                    
                    foreach (XElement validationError in validationErrors.Elements())
                    {
                        sb.AppendLine(validationError.Element("ErrorMessage").Value);
                    }
                    message = sb.ToString();
                }
                else
                {
                    message = errorException.Error.Element("ExceptionMessage").Value;
                }

            }
            else
            {
                message = exception.Message;
            }
            ShowMessageBox.Show(message, view);
        }


    }
}
