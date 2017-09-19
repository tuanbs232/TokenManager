using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TokenManager.common
{
    class OcspValidator
    {
        public static readonly int SUCCESSFULL = 0;

        public const int STATUS_GOOD = 0;
        public const int STATUS_REVOKED = 1;
        public const int STATUS_UNKNOWN = 2;

        public static int check(BigInteger serialNumber, X509Certificate issuer, string ocspUrl)
        {
            OcspReq req = GenerateOcspRequest(issuer, serialNumber);

            byte[] binaryResp = PostData(ocspUrl, req.GetEncoded(), "application/ocsp-request", "application/ocsp-response");

            return ProcessOcspResponse(issuer, binaryResp);
        }

        public static string GetMessage(int code)
        {
            switch (code)
            {
                case STATUS_GOOD:
                    return "GOOD";
                case STATUS_REVOKED:
                    return "REVOKED";
                default:
                    return "UNKNOWN";
            }
        }
        public static X509Extensions GetX509Extensions(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            try
            {
                var inputStream = new Asn1InputStream(certificate.RawData);
                var certificateAsAsn1 = inputStream.ReadObject();
                var certificateStructure = X509CertificateStructure.GetInstance(certificateAsAsn1);
                var toBeSignedPart = certificateStructure.TbsCertificate;
                var extensions = toBeSignedPart.Extensions;
                if (extensions == null)
                {
                    throw new Exception("No X509 extensions found");
                }
                return extensions;
            }
            catch (CertificateEncodingException e)
            {
                throw new ArgumentException("Error while extracting Access Description", e);
            }
        }

        public static String GetAccessDescriptionUrlForOid(DerObjectIdentifier oid, AccessDescription[] authorityInformationAccessArray)
        {
            foreach (AccessDescription authorityInformationAcces in authorityInformationAccessArray)
            {
                if (oid.Equals(authorityInformationAcces.AccessMethod))
                {
                    var name = authorityInformationAcces.AccessLocation;
                    return ((DerIA5String)name.Name).GetString();
                }
            }
            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="eeCert"></param>
        /// <param name="issuerCert"></param>
        /// <param name="binaryResp"></param>
        /// <returns></returns>
        private static int ProcessOcspResponse(X509Certificate issuerCert, byte[] binaryResp)
        {
            OcspResp r = new OcspResp(binaryResp);
            int cStatus = STATUS_UNKNOWN;

            switch (r.Status)
            {
                case OcspRespStatus.Successful:
                    BasicOcspResp or = (BasicOcspResp)r.GetResponseObject();

                    //ValidateResponse(or, issuerCert); 

                    if (or.Responses.Length == 1)
                    {
                        SingleResp resp = or.Responses[0];

                        //ValidateCertificateId(issuerCert, eeCert, resp.GetCertID());
                        //ValidateThisUpdate(resp); 
                        //ValidateNextUpdate(resp); 

                        Object certificateStatus = resp.GetCertStatus();

                        if (certificateStatus == Org.BouncyCastle.Ocsp.CertificateStatus.Good)
                        {
                            cStatus = STATUS_GOOD;
                        }
                        else if (certificateStatus is Org.BouncyCastle.Ocsp.RevokedStatus)
                        {
                            cStatus = STATUS_REVOKED;
                        }
                        else if (certificateStatus is Org.BouncyCastle.Ocsp.UnknownStatus)
                        {
                            cStatus = STATUS_UNKNOWN;
                        }
                    }
                    break;
                default:
                    throw new Exception("Unknow status '" + r.Status + "'.");
            }

            return cStatus;
        }


        /// <summary>
        /// Check match request and response
        /// </summary>
        /// <param name="issuerCert"></param>
        /// <param name="eeCert"></param>
        /// <param name="certificateId"></param>
        private void ValidateCertificateId(X509Certificate issuerCert, X509Certificate eeCert, CertificateID certificateId)
        {
            CertificateID expectedId = new CertificateID(CertificateID.HashSha1, issuerCert, eeCert.SerialNumber);

            if (!expectedId.SerialNumber.Equals(certificateId.SerialNumber))
            {
                throw new Exception("Invalid certificate ID in response");
            }

            if (!Org.BouncyCastle.Utilities.Arrays.AreEqual(expectedId.GetIssuerNameHash(), certificateId.GetIssuerNameHash()))
            {
                throw new Exception("Invalid certificate Issuer in response");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        private static byte[] PostData(string url, byte[] data, string contentType, string accept)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = contentType;
            request.ContentLength = data.Length;
            request.Accept = accept;
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream respStream = response.GetResponseStream();
            byte[] resp = ToByteArray(respStream);
            respStream.Close();

            return resp;
        }

        private static readonly int BufferSize = 4096 * 8;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static byte[] ToByteArray(Stream stream)
        {
            byte[] buffer = new byte[BufferSize];
            MemoryStream ms = new MemoryStream();

            int read = 0;

            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerCert"></param>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private static OcspReq GenerateOcspRequest(X509Certificate issuerCert, BigInteger serialNumber)
        {
            CertificateID id = new CertificateID(CertificateID.HashSha1, issuerCert, serialNumber);
            return GenerateOcspRequest(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static OcspReq GenerateOcspRequest(CertificateID id)
        {
            OcspReqGenerator ocspRequestGenerator = new OcspReqGenerator();

            ocspRequestGenerator.AddRequest(id);

            BigInteger nonce = BigInteger.ValueOf(new DateTime().Ticks);

            ArrayList oids = new ArrayList();
            Hashtable values = new Hashtable();

            oids.Add(OcspObjectIdentifiers.PkixOcsp);

            Asn1OctetString asn1 = new DerOctetString(new DerOctetString(new byte[] { 1, 3, 6, 1, 5, 5, 7, 48, 1, 1 }));

            values.Add(OcspObjectIdentifiers.PkixOcsp, new X509Extension(false, asn1));
            ocspRequestGenerator.SetRequestExtensions(new X509Extensions(oids, values));

            return ocspRequestGenerator.Generate();
        }
    }
}
