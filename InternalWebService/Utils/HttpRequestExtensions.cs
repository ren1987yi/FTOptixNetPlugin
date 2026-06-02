using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService.Utils
{
    internal static class HttpRequestExtensions
    {
        public static async Task<MultipartSection> ParseMultipart(this NetCoreServer.HttpRequest request)
        {

            var contentType = request.GetHeaderValue("Content-Type");
          
            var stream = new MemoryStream(request.BodyBytes);


            var mediaType = MediaTypeHeaderValue.Parse(contentType);
            var boundary = HeaderUtilities.RemoveQuotes(mediaType.Boundary).Value;

            var reader = new MultipartReader(boundary, stream);

            MultipartSection section;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                // Process each section (file or form field)
                return section;
                //Console.WriteLine($"Section Content-Type: {section.ContentType}");
            }

            return null;
        }


        public static async Task<FileMultipartSection> GetFile(this NetCoreServer.HttpRequest request)
        {
            var m = await request.ParseMultipart();

            var f = m.AsFileSection();
            if (f != null)
            {
                return f;
            }
            return null;
        }
    
    
        public static string GetHeaderValue(this NetCoreServer.HttpRequest request,string header_name)
        {
            for (var i = 0; i < request.Headers; i++)
            {
                var (k, v) = request.Header(i);
                if (k.ToLower() == header_name.ToLower())
                {
                    return v;
                }
            }

            return string.Empty;
        }
    
    }
}
