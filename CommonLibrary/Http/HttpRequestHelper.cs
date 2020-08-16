using iFactory.CommonLibrary.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.CommonLibrary.Http
{
    /// <summary>
    /// http请求对象
    /// </summary>
    public class HttpRequestHelper : IHttpRequest
    {
        private readonly ILogWrite _log;//日志写入对象

        public HttpRequestHelper(ILogWrite logWrite)
        {
            this._log = logWrite;//注入日志写入
        }
        /// <summary>
        ///  http Post
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="postData">抛送参数</param>
        /// <param name="contentType">默认json</param>
        /// <param name="authorization">token</param>
        /// <returns></returns>
        public string HttpPost(string url, string postData, string contentType = "application/json;charset=UTF-8", string authorization = null)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest _reques = (HttpWebRequest)WebRequest.Create(url);
                _reques.Method = "POST";
                _reques.Timeout = 5000;
                //判断是否需要认证
                if (!string.IsNullOrEmpty(authorization))
                {
                    _reques.Headers.Add("Authorization", authorization);
                }
                //设定ContentType
                _reques.ContentType = contentType;
                _reques.ContentLength = 0;
                if (!string.IsNullOrEmpty(postData))
                {
                    byte[] data = Encoding.UTF8.GetBytes(postData);
                    _reques.ContentLength = data.Length;
                    using (Stream reqStream = _reques.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);

                        reqStream.Close();
                    }
                }
                
                HttpWebResponse resp = (HttpWebResponse)_reques.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog("HttpPost错误", ex);
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public string HttpPut(string url, string postData, string contentType = "application/json;charset=UTF-8", string authorization = null)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest _reques = (HttpWebRequest)WebRequest.Create(url);
                _reques.Method = "PUT";
                _reques.Timeout = 5000;
                //判断是否需要认证
                if (!string.IsNullOrEmpty(authorization))
                {
                    _reques.Headers.Add("Authorization", authorization);
                }
                //设定ContentType
                _reques.ContentType = contentType;
                _reques.ContentLength = 0;
                if (!string.IsNullOrEmpty(postData))
                {
                    byte[] data = Encoding.UTF8.GetBytes(postData);
                    _reques.ContentLength = data.Length;
                    using (Stream reqStream = _reques.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);

                        reqStream.Close();
                    }
                }
                HttpWebResponse resp = (HttpWebResponse)_reques.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog("HttpPut错误", ex);
            }


            return result;
        }
        /// <summary>
        /// Http Get
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="contentType">contentType</param>
        /// <param name="authorization">token</param>
        /// <returns></returns>
        public string HttpGet(string url, string contentType = "application/json;charset=UTF-8", string authorization = null)
        {
            string result = string.Empty;
            try
            {
                // _log.WriteLog($"开始请求{url}");
                HttpWebRequest _reques = (HttpWebRequest)WebRequest.Create(url);
                _reques.Method = "GET";
                if (!string.IsNullOrEmpty(authorization))
                {
                    _reques.Headers.Add("Authorization", authorization);
                }
                _reques.ContentType = contentType;
                _reques.Timeout = 5000;
                HttpWebResponse resp = (HttpWebResponse)_reques.GetResponse();
                Stream myResponseStream = resp.GetResponseStream();
                StreamReader reader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = reader.ReadToEnd();
                reader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {
                _log.WriteLog("HttpGet错误", ex);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public string HttpDel(string url, string contentType = "application/json;charset=UTF-8", string authorization = null)
        {
            string result = string.Empty;
            try
            {
                // ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest _reques = (HttpWebRequest)WebRequest.Create(url);
                _reques.Method = "DELETE";
                if (!string.IsNullOrEmpty(authorization))
                {
                    _reques.Headers.Add("Authorization", authorization);
                }
                _reques.ContentType = contentType;
                _reques.Timeout = 5000;
                HttpWebResponse resp = (HttpWebResponse)_reques.GetResponse();
                Stream myResponseStream = resp.GetResponseStream();
                StreamReader reader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = reader.ReadToEnd();
                reader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {
                _log.WriteLog("HttpDel错误", ex);
            }
            return result;
        }

    }
}
