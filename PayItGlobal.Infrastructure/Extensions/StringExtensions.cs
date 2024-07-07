using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static bool TryTrim(this string source, out string result)
        {
            // Implementation of the TryTrim method
            string trimmed = source.Trim();
            if (!object.ReferenceEquals(source, trimmed))
            {
                result = trimmed;
                return true;
            }
            else
            {
                result = source;
                return false;
            }
        }


    }
}
