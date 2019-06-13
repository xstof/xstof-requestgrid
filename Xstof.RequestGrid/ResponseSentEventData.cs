using System.Collections.Generic;

namespace Xstof.RequestGrid
{
    internal class ResponseSentEventData
    {
        public ResponseSentEventData(){}
        public string ContentType { get; set; }
        public int StatusCode { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Body {get; set;}
    }
}