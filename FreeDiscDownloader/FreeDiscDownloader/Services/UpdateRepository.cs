using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FreeDiscDownloader.Services
{
    class UpdateRepository
    {
        private float versionCurrent;
        public float versionServer { get; private set; }
        public String versionUpdateUrl { get; private set; }

        public async Task<bool> CheckUpdate()
        {
            #if DEBUG
            Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "CheckUpdate(): " + App.AppSetting.UpdateServerUrl);
            #endif


            if (!float.TryParse(App.AppSetting.GetAppVersion, NumberStyles.Float | NumberStyles.AllowThousands,
                             CultureInfo.InvariantCulture, out versionCurrent))
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "!float.TryParse(App.AppSetting.GetAppVersion, out versionCurrent)");
                #endif
                return false;
            }

            WebRequest request = null;
            try
            {
                request = WebRequest.Create(App.AppSetting.UpdateServerUrl);
                request.Timeout = 10000;
            }
            catch (System.Exception e)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "WebRequest Exception: " + e.Message.ToString());
                #endif
                return false;
            }

            WebResponse response = null;
            String responseString;
            try
            {
                Task<WebResponse> webResponseTask = request.GetResponseAsync();
                await Task.WhenAll(webResponseTask);

                if (webResponseTask.IsFaulted)
                {
                    #if DEBUG
                    Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "webResponseTask.IsFaulted");
                    #endif
                    return false;
                }

                response = webResponseTask.Result;
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }
            }
            catch (System.Exception e)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "WebResponse Exception: " + e.Message.ToString());
                #endif
                return false;
            }

            if (responseString.Length == 0)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "data.Length == 0");
                #endif
                return false;
            }

            XmlDocument doc = new XmlDocument();
            XmlNodeList nodes;
            try
            {
                doc.LoadXml(responseString);
                nodes = doc.DocumentElement.SelectNodes("/VersionInfo");
            }
            catch (System.Exception e)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "XML load Exception: " + e.Message.ToString());
                #endif
                return false;
            }

            if (nodes.Count == 0)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name, "nodes.Count == 0");
                #endif
                return false;
            }

            try
            {
                versionUpdateUrl = System.Net.WebUtility.UrlDecode(nodes[0].SelectSingleNode("UpdatePage").InnerText);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "System.Xml.XPath.XPathException: " + e.Message.ToString());
                #endif
                return false;
            }

            Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        nodes[0].SelectSingleNode("Version").InnerText);

            try
            {
                if (!float.TryParse(nodes[0].SelectSingleNode("Version").InnerText, NumberStyles.Float | NumberStyles.AllowThousands,
                             CultureInfo.InvariantCulture, out float versionS))
                {
                    #if DEBUG
                    Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "!float.TryParse(nodes[0].SelectSingleNode(\"Version\")");
                    #endif
                    return false;
                }
                versionServer = versionS;
            }
            catch (System.Xml.XPath.XPathException e)
            {
                #if DEBUG
                Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "System.Xml.XPath.XPathException: " + e.Message.ToString());
                #endif
                return false;
            }

            Debug.Write(System.Reflection.MethodBase.GetCurrentMethod().Name,
                       versionServer.ToString());
            return true;
        }


        public bool IsNewVersion()
        {
            if ((versionServer > versionCurrent) && (versionUpdateUrl.Length > 0))
                return true;
            return false;
        }
    }
}
