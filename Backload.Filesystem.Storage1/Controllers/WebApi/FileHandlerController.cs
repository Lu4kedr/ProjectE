using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Contracts.Status;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Backload.Filesystem.Storage1.WebApi.Controllers.Api
{
    public class FileHandlerController : ApiController
    {
        private IFileHandler _handler;

        public FileHandlerController()
        {
            // Create and initialize the handler
            _handler = Backload.FileHandler.Create();
            _handler.Init(new HttpRequestWrapper(HttpContext.Current.Request));
        }



        // GET api/<controller>
        // Requests meta data of files already stored 
        [ResponseType(typeof(IClientPluginResultCore))]
        public async Task<HttpResponseMessage> Get(HttpRequestMessage request, [FromUri(Name = "objectContext")]string userId)
        {
            try
            {
                // Call the execution pipeline and get the result
                IBackloadResult result = await _handler.Execute();

                // Get the client plugin specific result
                var clientResult = ((IFileStatusResult)result).ClientStatus;

                // return result object and status code
                return request.CreateResponse(result.HttpStatusCode, clientResult);


                #region Optional: Use Backload Api (Services namespace. Pro/Enterprise only) 

                //// Simple demo: Call the GET execution method and get the result
                //IFileStatus status = await _handler.Services.GET.Execute();

                //// Create plugin specific result
                //result = _handler.Services.Core.CreatePluginResult();

                //// return plugin specific result object
                //return request.CreateResponse(HttpStatusCode.OK, result.ClientStatus);

                #endregion Optional: Use backload Api (Services namespace. Pro/Enterprise only)

            }
            catch (Exception e)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }



        // GET api/<controller>?fileName=[name]  
        // Requests file data (bytes) of a specific file
        [ResponseType(typeof(ByteArrayContent))]
        public async Task<HttpResponseMessage> Get(HttpRequestMessage request, string fileName, [FromUri(Name = "objectContext")]string userId)
        {
            try
            {
                // Call the execution pipeline and get the result
                IBackloadResult result = await _handler.Execute();

                // Get the client plugin specific result
                var clientResult = (IFileDataResult)result;

                //// Create response ojbect (bytes)
                HttpResponseMessage response = request.CreateResponse(result.HttpStatusCode);
                response.Content = new ByteArrayContent(clientResult.FileData);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(result.ContentType);

                return response;
            }
            catch (Exception e)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }




        // DELETE api/<controller>?fileName=[name]  
        // Deletes a specific file from storage 
        [ResponseType(typeof(IClientPluginResultCore))]
        public async Task<HttpResponseMessage> Delete(HttpRequestMessage request, string fileName, [FromUri(Name = "objectContext")]string userId)
        {
            try
            {
                // Call the execution pipeline and get the result
                IBackloadResult result = await _handler.Execute();

                // Get the client plugin specific result
                var clientResult = ((IFileStatusResult)result).ClientStatus;

                // return result object and status code
                return request.CreateResponse(result.HttpStatusCode, clientResult);

            }
            catch (Exception e)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

        }




        // POST api/<controller> 
        // Stores a file
        [ResponseType(typeof(IClientPluginResultCore))]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request, [FromUri(Name = "objectContext")]string userId)
        {
            try
            {
                // Call the execution pipeline and get the result
                IBackloadResult result = await _handler.Execute();

                // Get the client plugin specific result
                var clientResult = ((IFileStatusResult)result).ClientStatus;

                // return result object and status code
                return request.CreateResponse(result.HttpStatusCode, clientResult);

            }
            catch (Exception e)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

        }



    }
}