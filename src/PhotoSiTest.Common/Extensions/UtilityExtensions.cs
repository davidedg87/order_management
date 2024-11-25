using System.Text;

namespace PhotoSiTest.Common.Extensions
{
    public static class UtilityExtensions
    {
        #region Exception

        private static readonly string ExceptionMessageSeparator = Environment.NewLine;

        public static string GetNestedMessage(this Exception ex, string rootMessage = "")
        {
            return BuildMessage(ex, rootMessage, false);
        }

        public static string GetFullMessage(this Exception ex, string rootMessage = "")
        {
            return BuildMessage(ex, rootMessage, true);
        }

        private static string BuildMessage(this Exception ex, string rootMessage, bool includeStackTrace)
        {
            StringBuilder sb = new();

            try
            {
                if (ex != null)
                {
                    if (!string.IsNullOrEmpty(rootMessage))
                    {
                        sb.Append(rootMessage);
                        sb.Append(ExceptionMessageSeparator);
                    }
                    sb.Append(ex.Message);

                    // I append to the error message the Message of the various nested exceptions and the StackTrace of the original exception
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        sb.Append(ExceptionMessageSeparator);
                        sb.Append(ex.Message);
                    }
                    if (includeStackTrace)
                    {
                        sb.Append(ExceptionMessageSeparator);
                        sb.Append(ex.StackTrace);
                    }
                }
            }
            catch (Exception ex2)
            {
                sb.AppendLine();
                sb.Append("Error in Exception.BuildMessage(). ");
                sb.Append(ex2.Message);
            }

            return sb.ToString();
        }

        #endregion


    }
}
