using System.Net;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace BC_WebServer.Controllers
{
    public class DownloadController : ApiController
    {
        // GET: api/Download
        /*public HttpResponseMessage Get1()
        {
            
            var path = @"C:\bc-apps\SkypeSetupFull.exe";
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");

            result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(path);
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(MimeMapping.GetMimeMapping(path));
            return result;
        }*/

        // GET: api/Download/5
        public string Get(int id)
        {
            return "value";
        }

       
        // POST: api/Download
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Download/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Download/5
        public void Delete(int id)
        {
            //[RoutePrefix("aa")]

            //[Route("kk/{cdn:regex(.)}")]

        }


        // GET: api/Download
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            string targetURL = "";
            string uri = request.RequestUri.AbsolutePath;
            System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> keyParis = request.GetQueryNameValuePairs();

            foreach (var k in keyParis)
            {
                System.Diagnostics.Debug.WriteLine(k.Key +";;"+ k.Value);
                if(k.Key.Equals("url"))
                {
                    targetURL = k.Value;
                }
            }

            string fullfilepathpath = "";
            string filename = "";

            if (targetURL != null)
            {
                string[] a = targetURL.Split('/');

                filename = a[a.Length-1];
                fullfilepathpath = @"C:\bc-apps\" + filename;

                if(!File.Exists(fullfilepathpath))
                {
                    using (WebClient myWebClient = new WebClient())
                    {
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(targetURL, fullfilepathpath);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("URI: " + uri);

            string[] words = uri.Split('/');

            foreach(var word in words)
            {
                System.Diagnostics.Debug.WriteLine($"<{word}>");
            }

            var mediaType = MimeMapping.GetMimeMapping(filename);
            string fullpath = fullfilepathpath;
            var stream = System.IO.File.OpenRead(fullpath);

            if (Request.Headers.Range != null)
            {
                try
                {
                    HttpResponseMessage partialResponse = Request.CreateResponse(HttpStatusCode.PartialContent);
                    partialResponse.Content = new ByteRangeStreamContent(stream, Request.Headers.Range, mediaType);
                    return partialResponse;
                }
                catch (InvalidByteRangeException invalidByteRangeException)
                {
                    return Request.CreateErrorResponse(invalidByteRangeException);
                }
            }
            else
            {
                // If it is not a range request we just send the whole thing as normal
                HttpResponseMessage fullResponse = Request.CreateResponse(HttpStatusCode.OK);
                fullResponse.Content = new StreamContent(stream);
                fullResponse.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(mediaType);
                fullResponse.Content.Headers.ContentLength = stream.Length;
                fullResponse.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue(string.Format("\"7314eaa63931d51\""));
                fullResponse.Headers.Remove("Cache-Control");
                fullResponse.Headers.Remove("Pragma");
                //fullResponse.Headers.Remove("Expires");
                fullResponse.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = filename };
                fullResponse.Headers.Add("Accept-Ranges", "bytes");
                return fullResponse;
            }
        }
    }
}

