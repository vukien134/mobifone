using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Common.Extensions
{
    public static class ExtensionMethods
    {
        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            var task = (Task)@this.Invoke(obj, parameters);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }

        public static decimal? GetDefaultNullIfZero(this decimal? value)
        {
            if (value == null) return value;
            return value == 0 ? null : value;
        }

        public static decimal? GetDefaultNullIfZero(this decimal value)
        {
            return value == 0 ? null : value;
        }
    }
}
