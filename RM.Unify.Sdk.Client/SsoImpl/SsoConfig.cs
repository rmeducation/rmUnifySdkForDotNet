//-------------------------------------------------
// <copyright file="SsoConfig.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace RM.Unify.Sdk.Client.SsoImpl
{
    internal class SsoConfig
    {
        private static string _issuer = "http://sts.platform.rmunify.com/sts";
        private static string _issueUrl = "https://sts.platform.rmunify.com/issue/wsfed/";
        private static string _metadataUrl = "https://sts.platform.rmunify.com/FederationMetadata/2007-06/FederationMetadata.xml";

        private static string _ssoCertificateStr;
        private static X509Certificate2 _ssoCertificate;
        private static DateTime _certUpdated = DateTime.MinValue;
        private static object _lock = new object();

        /// <summary>
        /// RM Unify's signing certificate
        /// </summary>
        internal static X509Certificate2 SsoCertificate
        {
            get
            {
                if (_ssoCertificateStr == null)
                {
                    UpdateSsoCertificate();
                }
                if (_ssoCertificate == null)
                {
                    byte[] ssoCertificateBytes = Convert.FromBase64String(_ssoCertificateStr);
                    _ssoCertificate = new X509Certificate2(ssoCertificateBytes);
                }
                return _ssoCertificate;
            }
        }

        /// <summary>
        /// The issuer realm for RM Unify
        /// </summary>
        internal static string Issuer
        {
            get
            {
                return _issuer;
            }
        }

        /// <summary>
        /// The RM Unify endpoint for implementing the WS-Federation passive profile
        /// </summary>
        internal static string IssueUrl
        {
            get
            {
                return _issueUrl;
            }
        }

        /// <summary>
        /// Update the SSO certificate from metadata
        /// </summary>
        /// <returns>True if updated, false otherwise</returns>
        internal static bool UpdateSsoCertificate()
        {
            bool updated = false;

            // Keep other SSO threads waiting while we update the cert
            lock (_lock)
            {
                // Check a maximum of every 5 minutes to avoid DOS attacks
                if (_certUpdated < DateTime.Now.AddMinutes(-5))
                {
                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.Load(_metadataUrl);
                    }
                    catch
                    {
                        throw new RmUnifySsoException("Login failed: unable to download RM Unify metadata from " + _metadataUrl);
                    }

                    var nSpace = new XmlNamespaceManager(doc.NameTable);
                    nSpace.AddNamespace("md", "urn:oasis:names:tc:SAML:2.0:metadata");
                    nSpace.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

                    XmlElement certElement = (XmlElement)doc.SelectSingleNode("//md:KeyDescriptor[@use='signing']/ds:KeyInfo/ds:X509Data/ds:X509Certificate", nSpace);
                    if (certElement == null)
                    {
                        throw new RmUnifySsoException("Login failed: can't find certificate in RM Unify metadata from " + _metadataUrl);
                    }

                    string newCertStr = certElement.InnerText;
                    if (newCertStr != _ssoCertificateStr)
                    {
                        _ssoCertificateStr = newCertStr;
                        _ssoCertificate = null;
                        updated = true;
                    }

                    _certUpdated = DateTime.Now;
                }
            }

            return updated;
        }
    }
}
