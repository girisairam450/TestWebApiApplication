using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ErrorInfo
    {
        /// <summary>
        /// Gives the error message.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Gives the detailed stack trace of the error message.
        /// </summary>
        public string ErrorStackTrace { get; set; }
    }
}