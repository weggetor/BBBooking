using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;
using DotNetNuke.Web.Api;

namespace Bitboxx.DNNModules.BBBooking.Services
{
    public class ServiceController : DnnApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage HelloWorld(string eins, int zwei)
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                                          String.Format("Hello Welt {0} , {1}!", eins, zwei));
        }
    }
}