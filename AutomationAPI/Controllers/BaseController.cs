using AutomationAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace AutomationAPI.Controllers
{
    public class BaseController : ApiController
    {
        public HttpResponseMessage OK(string msg = "")
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(msg, Encoding.Unicode);
            return response;
        }
        public HttpResponseMessage ERROR(string msg = "")
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            response.Content = new StringContent(msg, Encoding.Unicode);
            return response;
        }
        /// <summary>
        /// 将数据填充到excel模板并下载
        /// </summary>
        /// <param name="fileName">下载时生成的文件名</param>
        /// <param name="filePath">模板文件的全路径含模板文件名</param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HttpResponseMessage Download(string fileName, string filePath, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new HttpResponseMessage(HttpStatusCode.OK);
            ExcelHelper excel = new ExcelHelper(filePath);
            MemoryStream ms = excel.CreateExcelWithTemplate(dt, 1);

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(ms);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8).Replace("+", "%20")
            };

            return httpResponseMessage;
        }
        /// <summary>
        /// 将数据填充到excel模板并下载
        /// </summary>
        /// <param name="fileName">下载时生成的文件名</param>
        /// <param name="filePath">模板文件的全路径含模板文件名</param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public HttpResponseMessage Download(string fileName, string filePath, DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
                return new HttpResponseMessage(HttpStatusCode.OK);
            ExcelHelper excel = new ExcelHelper(filePath);
            MemoryStream ms = excel.CreateExcelWithTemplate(ds, 1);

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(ms);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8).Replace("+", "%20")
            };

            return httpResponseMessage;
        }
        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="fileName">下载时生成的文件名</param>
        /// <param name="filePath">模板文件的全路径含模板文件名</param>
        /// <returns></returns>
        public HttpResponseMessage Download(string fileName, string filePath)
        {
            FileStream fs = File.OpenRead(filePath);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();

            Stream ms = new MemoryStream(bytes);
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(ms);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8).Replace("+", "%20")
            };

            return httpResponseMessage;
        }
    }
}