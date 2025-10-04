using System;
using System.Text;

namespace Plugin.Maui.SmartNavigation.SourceGenerators
{
    public static class Log
    {
        private static StringBuilder _builder;

        public static void Init(StringBuilder builder)
        {
            _builder = builder;
        }

        public static void Write(string entry)
        {
            _builder.Append($"{DateTime.Now.ToString()} - INFO - {entry}");
        }

        public static void WriteLine(string entry)
        {
            _builder.AppendLine($"{DateTime.Now.ToString()} - INFO - {entry}");
        }

        public static void FlushLog()
        {
            System.Diagnostics.Debug.Write(_builder.ToString());

        }
    }
}
