using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.JScript.NET;

namespace XData.Data.Helpers
{
    // Node.js is better
    public class JavaScriptEvaluator
    {
        // var i = 1;
        // var j = 2;
        // i * j
        public object Eval(string expression)
        {
            object obj = new Evaluator().Eval(expression);
            return obj;
        }
    }
}
