//-------------------------------------------------
// <copyright file="SamlSignedXml.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;
using System.Xml;
using System.Security.Cryptography.Xml;

namespace RM.Unify.Sdk.Client.SsoImpl
{
    // http://www.cedricascoop.be/?p=309
    internal class SamlSignedXml : SignedXml
    {
        private string _referenceAttributeId = "";

        public SamlSignedXml(XmlElement element, string referenceAttributeId)
            : base(element)
        {
            _referenceAttributeId = referenceAttributeId;
        }
        
        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            //return document.DocumentElement;
            var elem = (XmlElement)document.SelectSingleNode(string.Format("//*[@{0}='{1}']", _referenceAttributeId, idValue));
            return elem;
        }
    }
}
