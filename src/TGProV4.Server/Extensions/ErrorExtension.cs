namespace TGProV4.Server.Extensions;

public static class ErrorExtension
{
    public static void HandleStackTrace(this ICollection<BaseError> errors, Exception exception)
    {
        var stackTrace = new StackTrace(exception, true);

        for (var i = 0; i < stackTrace.FrameCount; i++)
        {
            var stackFrame = stackTrace.GetFrame(i);

            if (stackFrame is null) continue;

            string? fileName = stackFrame.GetFileName()?.Trim();
            string? method = stackFrame.GetMethod()?.ToString()?.Trim();
            string line = stackFrame.GetFileLineNumber().ToString().Trim();

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(method) && !string.IsNullOrEmpty(line))
            {
                errors.Add(new BaseSystemError
                {
                    FileName = fileName,
                    Method = method,
                    LineNumber = line
                }); 
            }
        }
    }
}
