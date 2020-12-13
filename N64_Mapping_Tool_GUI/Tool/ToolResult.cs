using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Application.Tool
{

    public enum ToolResultEnum
    {
        DefaultSuccess,
        DefaultError
    }

    /// <summary>
    /// Store a Message and a boolean indicating if the process succeded
    /// </summary>
    public class ToolResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        /// <summary>
        /// If it should be a warning instead of a success
        /// </summary>
        public bool Warn { get; set; }

        public ToolResult() : this(String.Empty, false, false) { }
        public ToolResult(string message, bool success) : this(message, success, false) { }
        public ToolResult(string message, bool success, bool warn)
        {
            Message = message;
            Success = success;
            Warn = warn;
        }
        public ToolResult(ToolResultEnum toolResultEnum)
        {
            if (toolResultEnum == ToolResultEnum.DefaultSuccess)
            {
                Message = Utils.MessageBoxConstants.MessageSuccessExecution;
                Success = true;
                Warn = false;
            }
            else if (toolResultEnum == ToolResultEnum.DefaultError)
            {
                Message = Utils.MessageBoxConstants.MessageErrorExecution;
                Success = false;
                Warn = false;
            }
        }

    }
}
