using System.Text;

namespace System
{
    public static class ExceptionEx
    {
        public static string GetLogMessage(this Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{exception.GetType().FullName} ({exception.Message})");
            sb.AppendLine("---------------------------------");
            sb.AppendLine(exception.ToString());
            if (exception.InnerException != null)
            {
                sb.Append("InnerException: ");
                sb.Append(exception.InnerException.GetLogMessage());
            }
            else
            {
                sb.AppendLine("=================================");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
