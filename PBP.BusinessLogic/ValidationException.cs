using System;
using System.Collections.Generic;
using System.Text;

namespace PBP.BusinessLogic
{
    class ValidationException: Exception
    {
        public int ErrorCode { get; set; }
        public ValidationException(int errorCode, string errorMessage) : base(errorMessage)
        {
            ErrorCode = errorCode;
        }
    }
}
