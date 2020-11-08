using Microarea.CheckTLS.Mago4.DigitalHubService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace Microarea.CheckTLS.Mago4
{
    public partial class DigitalHub_Wrapper : IDisposable
    {
        private fatelwV1Client FatelWebService = null;
        private fatelwV1Return FatelWebResult = null;
        private string token = string.Empty;

        private string Response = string.Empty;
        private string String_Error = string.Empty;
        //private int Code_Error = -1;
        private string XMLNotify = string.Empty;

        private int NumFileForZip = 0;

        private SHA256 Sha256 = null;
        //private byte[][] BlockArray = null;
        //private byte[] Hashfile = null;
        //private byte[] Hashdescriptor = null;
        private List<string> ListDocToCheck = null;
        private List<string> ListDocForNextOperation = null;
        private List<string> ListDocForDocInfo = null;

        #region Properties
        private string Token
        {
            get
            {
                if (FatelWebService != null)
                    token = FatelWebResult.token;
                else
                    token = string.Empty;

                return token;
            }
        }
        #endregion

        public DigitalHub_Wrapper(string Url, bool forceTLS = false)
        {
            try
            {
                BasicHttpBinding binding = null;
                if (forceTLS)
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                }
                else
                {
                    binding = new BasicHttpBinding();
                }
                binding.AllowCookies = true;
                Sha256 = SHA256Managed.Create();
                binding.MaxBufferPoolSize = 2147483647;
                binding.MaxBufferSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.ReaderQuotas.MaxArrayLength = 2147483647;
                binding.ReaderQuotas.MaxDepth = 2147483647;
                binding.ReaderQuotas.MaxBytesPerRead = 2147483647;



                EndpointAddress remoteAdd = new EndpointAddress(Url);

                Uri uri = null;

                bool bOk = Uri.TryCreate(Url, UriKind.Absolute, out uri);

                if (!bOk ||
                    uri == null ||
                    (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
                {
                    String_Error = "URL is not well formed";
                    return;
                }

                if (uri != null && uri.Scheme == Uri.UriSchemeHttps)
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;

                FatelWebService = new fatelwV1Client(binding, remoteAdd);

                ListDocToCheck = new List<string>();
                ListDocForNextOperation = new List<string>();
                ListDocForDocInfo = new List<string>();

                NumFileForZip = 0;
            }
            catch (Exception e)
            {
                String_Error = e.Message;
            }
        }

        #region Getter
        public int GetNumZipFiles() { return NumFileForZip; }

        public string GetWSError() { return String_Error; }

        public string GetWSResponse() { return Response; }

        public string GetXMLNotify() { return XMLNotify; }
        #endregion

        #region Lists Management
        public void AddDocumentToCheck(string DocId)
        {
            ListDocToCheck.Add(DocId);
        }

        public bool SomeDocumentToCheck()
        {
            if (ListDocToCheck == null)
                return false;

            return ListDocToCheck.Count > 0;
        }

        public void AddDocumentForNextOperation(string DocId)
        {
            ListDocForNextOperation.Add(DocId);
        }

        public void ClearDocumentForNextOperation()
        {
            ListDocForNextOperation.Clear();
        }

        public bool SomeDocumentForNextOperation()
        {
            if (ListDocForNextOperation == null)
                return false;

            return ListDocForNextOperation.Count > 0;
        }

        public void AddDocumentForDocInfo(string DocId)
        {
            ListDocForDocInfo.Add(DocId);
        }

        public bool SomeDocumentForDocInfo()
        {
            if (ListDocForDocInfo == null)
                return false;

            return ListDocForDocInfo.Count > 0;
        }
        #endregion

        #region Connect_Disconnect
        //-----------------------------------------------------------------------------
        public bool Connect(string username, string password, string company)
        {
            try
            {
                if (
                    string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(company)
                   )
                {
                    String_Error = "Some parameters are empty";
                    return false;
                }
                if (FatelWebService == null)
                    return false;

                FatelWebResult = FatelWebService.connect(username, password, company);

                if (FatelWebResult.code > 0)
                    String_Error = FatelWebResult.message;
            }
            catch (TimeoutException)
            {
                String_Error = "connection timed out";
                return false;
            }
            catch (Exception e)
            {
                String_Error = e.Message;
                return false;
            }

            return FatelWebResult.code == 0;
        }
        //-----------------------------------------------------------------------------
        public bool Disconnect()
        {
            try
            {
                FatelWebResult = FatelWebService.disconnect(Token);
                if (FatelWebResult.code > 0)
                    String_Error = FatelWebResult.message;
            }
            catch (TimeoutException)
            {
                String_Error = "connection timed out";
                return false;
            }
            catch (Exception e)
            {
                String_Error = e.Message;
                return false;
            }
            return FatelWebResult.code == 0;
        }
        #endregion

        #region Generic Functions
        ////-----------------------------------------------------------------------------
        public bool GetAziMaster()
        {
            try
            {
                FatelWebResult = FatelWebService.getAziMaster(Token);
                if (FatelWebResult.code > 0)
                    String_Error = FatelWebResult.message;
                else
                    Response = FatelWebResult.response;
            }
            catch (TimeoutException)
            {
                String_Error = "connection timed out";
                return false;
            }
            catch (Exception e)
            {
                String_Error = e.Message;
                return false;
            }
            return FatelWebResult.code == 0;
        }

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
