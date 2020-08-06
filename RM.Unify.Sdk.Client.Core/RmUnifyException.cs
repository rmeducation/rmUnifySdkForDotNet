using System;
using System.Collections.Generic;
using System.Text;

namespace RM.Unify.Sdk.Client
{
    public class RmUnifyException : Exception
    {
        public int ErrorCode { get; private set; }

        public RmUnifyException(int errorCode, string message = "") : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
