//-------------------------------------------------
// <copyright file="SignInMessage.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;
using System.Xml;
using System.Collections.Generic;

namespace RM.Unify.Sdk.Client.SsoImpl
{
    internal class SignInMessage
    {
        internal class AttrDictionary : Dictionary<string, string>
        {
            internal string GetOrDefault(string key)
            {
                string val = null;
                TryGetValue(key, out val);
                return val;
            }
        }

        private XmlDocument _message;
        private XmlElement _samlToken;
        private XmlNamespaceManager _nSpace;
        private AttrDictionary _attrs;

        private static int MaxTokenAgeSeconds = 60;

        /// <summary>
        /// Create a SignInMessage object from the message string
        /// </summary>
        /// <param name="message">message string - value of the "wa" paramenter during sign in</param>
        internal SignInMessage(string message)
        {
            try
            {
                _message = new XmlDocument();
                _message.PreserveWhitespace = true;
                _message.LoadXml(message);

                _nSpace = new XmlNamespaceManager(_message.NameTable);
                _nSpace.AddNamespace("trust", "http://docs.oasis-open.org/ws-sx/ws-trust/200512");
                _nSpace.AddNamespace("saml", "urn:oasis:names:tc:SAML:1.0:assertion");
                _nSpace.AddNamespace("ds", SamlSignedXml.XmlDsigNamespaceUrl);
            }
            catch (Exception)
            {
                throw new RmUnifySsoException("Login failed: malformed message (XML can't be parsed)");
            }

            _samlToken = (XmlElement)_message.DocumentElement.SelectSingleNode(
                    "/trust:RequestSecurityTokenResponseCollection/trust:RequestSecurityTokenResponse/trust:RequestedSecurityToken/saml:Assertion",
                    _nSpace);
            if (_samlToken == null)
            {
                throw new RmUnifySsoException("Login failed: malformed message (SAML 1.1 token not found)");
            }
        }

        /// <summary>
        /// Verify that a SAML token is valid and intended for this relying party
        /// </summary>
        /// <param name="realm">Relying party realm</param>
        /// <param name="maxClockSkewSeconds">Maximum clock skew between identity provider and relying party</param>
        /// <param name="tokenCache">Token cache for replay detection</param>
        /// <returns>Time token is valid until</returns>
        internal DateTime Verify(string realm, int maxClockSkewSeconds, IRmUnifyCache tokenCache)
        {
            VerifyAudience(realm);
            VerifyIssuer();
            VerifySignature();
            DateTime notOnOrAfter = VerifyDateConditions(maxClockSkewSeconds);
            VerifyReplay(maxClockSkewSeconds, tokenCache);

            return notOnOrAfter;
        }

        /// <summary>
        /// The attributes from the SAML token
        /// </summary>
        internal AttrDictionary Attributes
        {
            get
            {
                if (_attrs == null)
                {
                    _attrs = new AttrDictionary();
                    XmlNodeList nodes = _samlToken.SelectNodes("//saml:Attribute", _nSpace);
                    foreach (var node in nodes)
                    {
                        XmlElement elem = node as XmlElement;
                        if (elem != null)
                        {
                            string attrNamespace = elem.Attributes["AttributeNamespace"].Value;
                            string attrName = elem.Attributes["AttributeName"].Value;
                            if (!string.IsNullOrEmpty(attrNamespace))
                            {
                                attrName = attrNamespace + (attrNamespace.EndsWith("/") ? "" : "/") + attrName;
                            }

                            XmlElement valueElement = elem.FirstChild as XmlElement;
                            if (valueElement != null && valueElement.Name == "saml:AttributeValue")
                            {
                                _attrs[attrName] = valueElement.InnerText;
                            }
                        }
                    }
                }

                return _attrs;
            }
        }

        #region Private methods
        /// <summary>
        /// Verify that the audience of the SAML token matches the relying party realm
        /// </summary>
        /// <param name="realm">Relying party realm</param>
        private void VerifyAudience(string realm)
        {
            XmlElement audienceElement = (XmlElement)_samlToken.SelectSingleNode("//saml:AudienceRestrictionCondition/saml:Audience", _nSpace);

            if (audienceElement == null)
            {
                throw new RmUnifySsoException("Login failed: malformed token (audience restriction condition not found)");
            }

            if (audienceElement.InnerText != realm)
            {
                throw new RmUnifySsoException("Login failed: malformed token (audience restriction condition has unexpected value)");
            }
        }

        /// <summary>
        /// Verify that the token was issued by RM Unify
        /// Not absolutely necessary, as we're checking the signature, but probably worth it for belt and braces (and to reject invalid tokens with minimal work)
        /// </summary>
        private void VerifyIssuer()
        {
            XmlAttribute issuerAttr = _samlToken.Attributes["Issuer"];

            if (issuerAttr == null)
            {
                throw new RmUnifySsoException("Login failed: malformed token (issuer attribute not found on SAML assertion)");
            }

            if (issuerAttr.Value != SsoConfig.Issuer)
            {
                throw new RmUnifySsoException("Login failed: malformed token (issuer has unexpected value)");
            }
        }

        /// <summary>
        /// Verify that the SAML token is signed by RM Unify and that the signature is valid
        /// </summary>
        private void VerifySignature()
        {
            XmlElement signatureElement = (XmlElement)_samlToken.SelectSingleNode("//ds:Signature", _nSpace);

            SamlSignedXml samlSignedXml = new SamlSignedXml(_samlToken, "AssertionID");
            samlSignedXml.LoadXml(signatureElement);

            bool samlValid = samlSignedXml.CheckSignature(SsoConfig.SsoCertificate, true);

            if (!samlValid)
            {
                if (SsoConfig.UpdateSsoCertificate())
                {
                    VerifySignature();
                }
                else
                {
                    throw new RmUnifySsoException("Login failed: malformed token (signature could not be verified)");
                }
            }
        }

        /// <summary>
        /// Verify that the SAML token is valid at the current time
        /// </summary>
        /// <param name="maxClockSkewSeconds">Allowed clock skew between STS and RP (seconds)</param>
        /// <returns>Time token is valid until</returns>
        private DateTime VerifyDateConditions(int maxClockSkewSeconds)
        {
            DateTime now = DateTime.Now;

            XmlElement conditions = (XmlElement)_samlToken.SelectSingleNode("//saml:Conditions", _nSpace);
            if (conditions == null)
            {
                throw new RmUnifySsoException("Login failed: malformed token (conditions not found)");
            }

            // Check NotBefore condition
            XmlAttribute notBeforeAttr = conditions.Attributes["NotBefore"];
            if (notBeforeAttr == null)
            {
                throw new RmUnifySsoException("Login failed: not before date missing");
            }

            DateTime notBefore;
            try
            {
                notBefore = DateTime.Parse(notBeforeAttr.InnerText);
            }
            catch (Exception)
            {
                throw new RmUnifySsoException("Login failed: malformed token (can't parse not before date)");
            }
            if (notBefore.AddSeconds(-maxClockSkewSeconds) > now)
            {
                throw new RmUnifySsoException("Login failed: not before date invalid");
            }

            // Because we don't necessarily have a replay token cache that works across servers, we'll also reject any token more than 60 seconds old
            if (notBefore.AddSeconds(maxClockSkewSeconds + MaxTokenAgeSeconds) < now)
            {
                throw new RmUnifySsoException("Login failed: token is stale");
            }

            // Check NotBefore condition
            XmlAttribute notOnOrAfterAttr = conditions.Attributes["NotOnOrAfter"];
            if (notOnOrAfterAttr == null)
            {
                throw new RmUnifySsoException("Login failed: not on or after date missing");
            }

            DateTime notOnOrAfter;
            try
            {
                notOnOrAfter = DateTime.Parse(notOnOrAfterAttr.InnerText);
            }
            catch (Exception)
            {
                throw new RmUnifySsoException("Login failed: malformed token (can't parse not on or after date)");
            }
            if (notOnOrAfter.AddSeconds(-maxClockSkewSeconds) <= now)
            {
                throw new RmUnifySsoException("Login failed: not on or after date invalid");
            }

            return notOnOrAfter;
        }

        /// <summary>
        /// Verify that the SAML token is not being replayed
        /// </summary>
        /// <param name="maxClockSkewSeconds">Allowed clock skew between STS and RP (seconds)</param>
        /// <param name="tokenCache">Token cache</param>
        void VerifyReplay(int maxClockSkewSeconds, IRmUnifyCache tokenCache)
        {
            XmlAttribute assertionIdAttr = _samlToken.Attributes["AssertionID"];
            if (assertionIdAttr == null)
            {
                throw new RmUnifySsoException("Login failed: assertion ID not found");
            }

            string assertionId = assertionIdAttr.Value;
            if (tokenCache.Get(assertionIdAttr.Value) != null)
            {
                throw new RmUnifySsoException("Login failed: token replay detected");
            }
            tokenCache.Add(assertionId, "", DateTime.Now.AddSeconds(maxClockSkewSeconds + MaxTokenAgeSeconds));
        }
        #endregion
    }
}
