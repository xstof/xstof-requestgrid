using System.Collections.Generic;

namespace Xstof.RequestGrid
{
    internal class RequestReceivedEventData
    {
        public RequestReceivedEventData(){}
        public string ContentType { get; set; }
        public string Host { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Body {get; set;}
        
    }
}