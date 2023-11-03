using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Logging;

namespace Accounting.Exceptions
{
    [Serializable]
    public class AccountingException : Exception,
    IAccountingException,
    IHasErrorCode,
    IHasErrorDetails,
    IHasLogLevel
    {
        public string Code { get; set; }

        public string Details { get; set; }

        public LogLevel LogLevel { get; set; }

        public AccountingException(
            string code = null,
            string message = null,
            string details = null,
            Exception innerException = null,
            LogLevel logLevel = LogLevel.Warning)
            : base(message, innerException)
        {
            Code = code;
            Details = details;
            LogLevel = logLevel;
        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public AccountingException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        public AccountingException WithData(string name, object value)
        {
            Data[name] = value;            
            return this;
        }
    }
}
