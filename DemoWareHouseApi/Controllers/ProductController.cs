using System;
using System.Collections.Generic;
using DemoWareHouseApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Web;

namespace DemoWareHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        // GET: api/Product
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            CheckCertificate();

            return new List<Product>
            {
                new Product { Id = 1, Name = "test1", Price = 10 },
                new Product { Id = 2, Name = "test2", Price = 11 },
                new Product { Id = 3, Name = "test3", Price = 12 },
                new Product { Id = 4, Name = "test4", Price = 13 },
                new Product { Id = 5, Name = "test5", Price = 14 }
            };
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public Product Get(int id)
        {
            return new Product { Id = id, Name = $"test{id}", Price = id * 10 };
        }

        // POST: api/Product
        [HttpPost]
        public void Post([FromBody] Product product)
        {
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Product product)
        {
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        // https://docs.microsoft.com/en-us/azure/app-service/app-service-web-configure-tls-mutual-auth#special-considerations-for-certificate-validation
        private string certHeader = "";
        private string errorString = "";
        private X509Certificate2 certificate = null;
        private string certThumbprint = "";
        private string certSubject = "";
        private string certIssuer = "";
        private string certSignatureAlg = "";
        private string certIssueDate = "";
        private string certExpiryDate = "";
        private bool isValidCert = false;

        private void CheckCertificate()
        {
            var headers = Request.Headers;
            certHeader = headers["X-ARR-ClientCert"];
            if (!String.IsNullOrEmpty(certHeader))
            {
                try
                {
                    byte[] clientCertBytes = Convert.FromBase64String(certHeader);
                    certificate = new X509Certificate2(clientCertBytes);
                    certSubject = certificate.Subject;
                    certIssuer = certificate.Issuer;
                    certThumbprint = certificate.Thumbprint;
                    certSignatureAlg = certificate.SignatureAlgorithm.FriendlyName;
                    certIssueDate = certificate.NotBefore.ToShortDateString() + " " + certificate.NotBefore.ToShortTimeString();
                    certExpiryDate = certificate.NotAfter.ToShortDateString() + " " + certificate.NotAfter.ToShortTimeString();
                }
                catch (Exception ex)
                {
                    errorString = ex.ToString();
                }
                finally
                {
                    isValidCert = IsValidClientCertificate();
                    if (!isValidCert) Response.StatusCode = 403;
                    else Response.StatusCode = 200;
                }
            }
            else
            {
                certHeader = "";
            }
        }
            
        // This is a SAMPLE verification routine. Depending on your application logic and security requirements, 
        // you should modify this method
        //
        private bool IsValidClientCertificate()
        {
            // In this example we will only accept the certificate as a valid certificate if all the conditions below are met:
            // 1. The certificate is not expired and is active for the current time on server.
            // 2. The subject name of the certificate has the common name nildevecc
            // 3. The issuer name of the certificate has the common name nildevecc and organization name Microsoft Corp
            // 4. The thumbprint of the certificate is 30757A2E831977D8BD9C8496E4C99AB26CB9622B
            //
            // This example does NOT test that this certificate is chained to a Trusted Root Authority (or revoked) on the server 
            // and it allows for self signed certificates
            //

            if (certificate == null || !String.IsNullOrEmpty(errorString)) return false;

            // 1. Check time validity of certificate
            if (DateTime.Compare(DateTime.Now, certificate.NotBefore) < 0 || DateTime.Compare(DateTime.Now, certificate.NotAfter) > 0) return false;

            // 2. Check subject name of certificate
            bool foundSubject = false;
            string[] certSubjectData = certificate.Subject.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in certSubjectData)
            {
                if (string.Compare(s.Trim(), "CN=devdemo-42.org") == 0)
                {
                    foundSubject = true;
                    break;
                }
            }
            if (!foundSubject) return false;

            // 3. Check issuer name of certificate
            bool foundIssuerCN = false, foundIssuerO = false;
            string[] certIssuerData = certificate.Issuer.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in certIssuerData)
            {
                if (string.Compare(s.Trim(), "CN=devdemo-42.org") == 0)
                {
                    foundIssuerCN = true;
                    if (foundIssuerO) break;
                }

                if (string.Compare(s.Trim(), "O=None") == 0)
                {
                    foundIssuerO = true;
                    if (foundIssuerCN) break;
                }
            }

            if (!foundIssuerCN || !foundIssuerO) return false;

            // 4. Check thumprint of certificate
            if (string.Compare(certificate.Thumbprint.Trim().ToUpper(), "0179B9B3098E1680C03E203892013CB379F4759E") != 0) return false;

            return true;
        }
    }
}
