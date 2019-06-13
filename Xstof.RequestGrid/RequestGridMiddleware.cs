using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Options;
using System.IO;

namespace Xstof.RequestGrid
{
    public class RequestGridMiddleware : IMiddleware
    {
        private readonly string topicEndpoint;
        private readonly string topicKey;

        public RequestGridMiddleware(IOptions<RequestGridMiddlewareOptions> options): this() {
            this.topicEndpoint = options.Value.TopicEndpoint;
            this.topicKey = options.Value.TopicKey;
        }

        public RequestGridMiddleware(){}

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await emitRequest(context);            
            await next(context);
            await emitResponse(context);
        }

        private async Task emitRequest(HttpContext context)
        {
            var client = getEventGridClient();
            var topicHostName = new Uri(topicEndpoint).Host;

           var headers = extractHeaders(context.Request.Headers);
           var body = await extractBody(context.Request.Body);

            var reqEvent = new EventGridEvent(){
                Id = Guid.NewGuid().ToString(),
                EventType = "Xstof.RequestGrid.RequestReceivedEvent",
                Data = new RequestReceivedEventData(){
                    ContentType = context.Request.ContentType,
                    Host = context.Request.Host.Value,
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    Headers = headers,
                    Body = body
                },
                EventTime = DateTime.Now,
                Subject = "RequestReceivedSubject",
                DataVersion = "2.0"
            };

            await client.PublishEventsAsync(topicHostName, new List<EventGridEvent>(){reqEvent});
        }
        private async Task emitResponse(HttpContext context)
        {
            var client = getEventGridClient();
            var topicHostName = new Uri(topicEndpoint).Host;

            var headers = extractHeaders(context.Response.Headers);
            var body = await extractBody(context.Response.Body);

            var reqEvent = new EventGridEvent(){
                Id = Guid.NewGuid().ToString(),
                EventType = "Xstof.RequestGrid.ResponseSentEvent",
                Data = new ResponseSentEventData(){
                    ContentType = context.Response.ContentType,
                    StatusCode = context.Response.StatusCode,
                    Headers = headers,
                    Body = body
                },
                EventTime = DateTime.Now,
                Subject = "RequestReceivedSubject",
                DataVersion = "2.0"
            };

            await client.PublishEventsAsync(topicHostName, new List<EventGridEvent>(){reqEvent});
        }


        private Dictionary<string, string> extractHeaders(IHeaderDictionary headersDictionary){
            var headers = new Dictionary<string, string>();
            foreach (var item in headersDictionary)
            {
                headers.Add(item.Key, item.Value.First());
            }

            return headers;
        }
        private async Task<string> extractBody(Stream body){
            string bodyText = "";
            using(var reader = new StreamReader(body)){
                bodyText = await reader.ReadToEndAsync();
            }

            return bodyText;
        }
        private EventGridClient getEventGridClient(){
            var creds = new TopicCredentials(topicKey);
            var client = new EventGridClient(creds);

            return client;
        }
    }
}
