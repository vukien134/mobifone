using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Accounting.Common
{
    public static class Util
    {
        public static bool ILike(string propertyName,string value)
        {
            return EF.Functions.ILike(propertyName, value);
        }
        public static string MD5Hash(string input)
        {
            using var provider = MD5.Create();
            var builder = new StringBuilder();

            foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(input)))
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }
        public static string SignXml(string certFile,string password,string xml,
                                            string signId,string dataUri,string tagSign)
        {
            XmlDocument document = new XmlDocument();
            document.PreserveWhitespace = true;

            
            document.LoadXml(xml);                

            X509Certificate2 cert = new X509Certificate2(certFile, password);

            AsymmetricAlgorithm asymmetricAlgorithm = cert.GetRSAPrivateKey();

            SignedXml signedXml = new SignedXml(document);
            signedXml.Signature.Id = signId;
            signedXml.SigningKey = asymmetricAlgorithm;

            KeyInfo keyInfo = new KeyInfo();

            //RSACng rsaprovider = (RSACng)cert.PublicKey.GetRSAPublicKey();
            //keyInfo.AddClause(new RSAKeyValue((RSA)rsaprovider));

            KeyInfoX509Data keyInfoData = new KeyInfoX509Data(cert);
            keyInfoData.AddSubjectName(cert.SubjectName.Name);
            keyInfo.AddClause(keyInfoData);

            signedXml.KeyInfo = keyInfo;

            Reference reference = new Reference();
            reference.Uri = dataUri;

            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();

            reference.AddTransform(env);

            XmlDsigC14NTransform env1 = new XmlDsigC14NTransform();
            reference.AddTransform(env1);

            signedXml.AddReference(reference);

            signedXml.ComputeSignature();

            XmlElement xmlDigitalSignature = signedXml.GetXml();


            XmlNodeList nodeList = document.GetElementsByTagName(tagSign);

            foreach (XmlNode node in nodeList)
            {

                node.AppendChild(xmlDigitalSignature);
            }
            

            return document.OuterXml;
        }
    }
}
