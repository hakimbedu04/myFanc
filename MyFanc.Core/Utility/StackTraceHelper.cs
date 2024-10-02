using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.Core.Utility
{
    public static class StackTraceHelper
    {
        public static string GetText(this StackTrace stack)
        {
            StackFrame[] frames = stack.GetFrames();

            StringBuilder stringBuilder = new StringBuilder();

            // Iterate over the frames and print information
            foreach (StackFrame frame in frames)
            {
                stringBuilder.Append($"{frame}");
            }
            return stringBuilder.ToString();
        }
    }
}
