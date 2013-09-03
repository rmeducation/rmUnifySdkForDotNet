//-------------------------------------------------
// <copyright file="RmUnifySsoException.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;

namespace RM.Unify.Sdk.Client
{
    public class RmUnifySsoException : Exception
    {
        public RmUnifySsoException(string message)
            : base(message)
        {
        }
    }
}
