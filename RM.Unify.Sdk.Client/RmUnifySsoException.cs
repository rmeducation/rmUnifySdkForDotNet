//-------------------------------------------------
// <copyright file="RmUnifySsoException.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;

namespace RM.Unify.Sdk.Client
{
    public class RmUnifySsoException : RmUnifyException
    {
            // Standard scenarios
        public const int ERRORCODES_VERIFICATIONFAILED = 1;
        public const int ERRORCODES_UNKNOWNACCOUNT = 2;
        public const int ERRORCODES_DISABLEDACCOUNT = 3;
        public const int ERRORCODES_CREATINGACCOUNT = 4;
        public const int ERRORCODES_MISSINGATTRIBUTES = 5;
        public const int ERRORCODES_FUTUREOREXPIREDTOKEN = 8;
        public const int ERRORCODES_TOKENREPLAY = 9;
        public const int ERRORCODES_METADATADOWNLOADFAILED = 10;

        // Only relevant to SSO connectors and brownfield
        public const int ERRORCODES_INVALIDAPPUSERID = 6;
        public const int ERRORCODES_APPUSERIDNOTINESTABLISHMENT = 7;
        public const int ERRORCODES_NOLICENCE = 101;
        public const int ERRORCODES_INVALIDAPPESTABLISHMENTKEY = 102;

        // Internal errors
        public const int ERRORCODES_TEMPORARYINTERNALERROR = 998;
        public const int ERRORCODES_PERMANENTINTERNALERROR = 999;

        public bool ShowCustomMessage { get; private set; }

        public RmUnifySsoException(int errorCode, string message = "", bool showCustomMessage = false) : base(errorCode, message)
        {
            ShowCustomMessage = showCustomMessage;
        }
    }
}
