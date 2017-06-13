using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Contracts.Status;
using Backload.Helper;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Backload.Filesystem.Storage1.Controllers
{

    /// <summary>
    /// Custom controller for the events demo  Note: events must be enabled in the config.
    /// </summary>
    public class CustomEventsController : Controller
    {
        /// <summary>
        /// A custom file handler. 
        /// To access it in an Javascript ajax request use: <code>var url = "/CustomEvents/FileHandler/";</code>.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete | HttpVerbs.Options)]
        public async Task<ActionResult> FileHandler()
        {
            try
            {
                // Create and initialize the handler
                IFileHandler handler = Backload.FileHandler.Create();


                // Attach event handlers to events
                handler.Events.IncomingRequestStarted += Events_IncomingRequestStarted;
                handler.Events.GetFilesRequestStarted += Events_GetFilesRequestStarted;
                handler.Events.GetFilesRequestFinished += Events_GetFilesRequestFinished;
                handler.Events.StoreFileRequestStarted += Events_StoreFileRequestStarted;


                // Init Backloads execution environment and execute the request
                handler.Init(HttpContext.Request);
                IBackloadResult result = await handler.Execute();


                // Helper to create an ActionResult object from the IBackloadResult instance
                return ResultCreator.Create(result);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }



        void Events_IncomingRequestStarted(IFileHandler sender, Backload.Contracts.Eventing.IIncomingRequestEventArgs e)
        {
            // Set a user id to store files in a user related folder
            string currentUserId = "97966ABE-0691-4874-958C-98AD07BB461C";

            // Adjust storage path
            sender.BasicStorageInfo.FileDirectory += currentUserId + "\\";
            sender.BasicStorageInfo.ThumbsDirectory += currentUserId + "\\";

            // If download is enabled, adjust the url also
            sender.BasicStorageInfo.FileDirectoryUrl = new System.Uri(sender.BasicStorageInfo.FileDirectoryUrl, currentUserId + "/");
            sender.BasicStorageInfo.ThumbsDirectoryUrl = new System.Uri(sender.BasicStorageInfo.ThumbsDirectoryUrl, currentUserId + "/");

        }


        void Events_GetFilesRequestStarted(IFileHandler sender, Backload.Contracts.Eventing.IGetFilesRequestEventArgs e)
        {
            // Backload component has started the internal GET handler method. 
            string searchPath = e.Param.SearchPath;
        }


        void Events_GetFilesRequestFinished(IFileHandler sender, Backload.Contracts.Eventing.IGetFilesRequestEventArgs e)
        {
            // Backload component has finished the internal GET handler method. 
            // Results can be found in e.Param.FileStatus or sender.FileStatus

            IFileStatus status = e.Param.FileStatus;
        }



        void Events_StoreFileRequestStarted(IFileHandler sender, Backload.Contracts.Eventing.IStoreFileRequestEventArgs e)
        {
            //e.Context.Configuration.Images.ForceImageType = "image/png";
            //if (e.Param.FileStatusItem.MainContentType == "image")
            //    e.Param.FileStatusItem.ContentType = "image/png";
            //var storageInfo = e.Param.FileStatusItem.StorageInfo;
        }
    }
}
