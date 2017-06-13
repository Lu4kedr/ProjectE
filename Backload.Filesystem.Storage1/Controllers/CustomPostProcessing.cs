using Backload.Context.DataProvider;
using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Helper;
using Backload.Status;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Backload.Filesystem.Storage1.Controllers
{

    /// <summary>
    /// Custom controller for the events demo  Note: events must be enabled in the config.
    /// </summary>
    public class CustomPostProcessingController : Controller
    {
        /// <summary>
        /// The Backload file handler. 
        /// To access it in an Javascript ajax request use: <code>var url = "/Backload/FileHandler/";</code>.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete | HttpVerbs.Options)]
        public async Task<ActionResult> FileHandler()
        {
            try
            {
                // Create and initialize the handler
                IFileHandler handler = Backload.FileHandler.Create();


                // Attach event handlers to events
                handler.Events.PostProcessingStarted += Events_PostProcessingStarted;
                handler.Events.PostProcessingFinished += Events_PostProcessingFinished;
                handler.Events.PostProcessingException += Events_PostProcessingException;


                // Init Backloads execution environment and execute the request
                handler.Init(this.Request);
                IBackloadResult result = await handler.Execute();

                // Helper to create an ActionResult object from the IBackloadResult instance
                return ResultCreator.Create(result);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }




        #region event handler


        /// <summary>
        /// Demo: Start the built-in archive operation and return data about the operation back to the client 
        /// (see PostProcessingFinished event handler below). 
        /// 3 options to set the archive file path: 
        ///     1) Create archive in a sub folder of the upload folder (e.g. archives\)
        ///     2) Create archive relative to the web application folder (e.g. ~/archives/...)
        ///     3) Create archive by an absolute path (e.g. z:\\archives\\...)
        /// </summary>
        /// <example>
        /// // Using archive options to create an archive
        /// e.Param.ArchiveOptions.ArchivePath = "archives\\" + DateTime.UtcNow.Ticks.ToString() + ".zip";
        /// e.Param.ArchiveOptions.ArchivePath = "~/archives/" + e.Param.BackloadValues.ObjectContext + "/" + DateTime.UtcNow.Ticks.ToString() + ".zip";
        /// e.Param.ArchiveOptions.ArchivePath = sender.RequestSetting.StorageInfo.StorageRoot + "\\archives\\" + e.Param.BackloadValues.ObjectContext + "\\" + DateTime.UtcNow.Ticks.ToString() + ".zip";
        /// 
        /// // Using core services to create archives (Pro/Enterprise)
        /// var task = handler.Services.Core.CreateArchive(directory, archivePath);
        /// </example>
        private void Events_PostProcessingStarted(IFileHandler sender, Backload.Contracts.Eventing.IPostProcessingEventArgs e)
        {
            // The post processing event provides some integrated actions that can be executed. 
            // In this demo, the client sends a post processing method parameter we can use decide if files should be archived or deleted.
            // Note: In a real world project, you usually do not allow a client execute server side methods directly or without security checks.
            bool archiving = bool.Parse(e.Methods.Parameter[0]);
            bool deleting = bool.Parse(e.Methods.Parameter[1]);

            // Create archive of files. Pro/Enterprise users can also call core service CreateArchive() methods direcly.
            // In this example we use ObjectContext as a user id. We also use it as archive name combined with the current date/time.
            e.Methods.CreateArchive = archiving;
            string name = e.Param.BackloadValues.ObjectContext + "-" + DateTime.UtcNow.Ticks.ToString("X") + ".zip";
            e.Methods.ArchiveOptions.ArchivePath = "~/Uploads/archives/" + name;

            // Delete files after archive is created. Pro/Enterprise users can also call core service DeleteFiles() methods directly.
            e.Methods.DeleteFiles = deleting;
        }



        /// <summary>
        /// Demo: Return some status data about the archive creation operation to the client
        /// In this example we override the internally created result and set a custom result.
        /// An ICoreResult object can take any ojbect type to be returned as Json to the client.
        /// To override the internal result object, assign an IBackloadResult object (e.g. ICoreResult)
        /// to the Result property of the current IFileHandler instance. This can be done by the 
        /// sender object or a global IFileHandler instance variable. Example:
        /// <example>
        /// handler.Result = new CoreResult(ret);            // Globally declared IFileHandler instance variable
        /// sender.Result = new CoreResult(ret);             // Sender object
        /// </example>
        /// </summary>
        private void Events_PostProcessingFinished(IFileHandler sender, Backload.Contracts.Eventing.IPostProcessingEventArgs e)
        {
            // Return a custom Json result to the client 
            CustomResult ret = new CustomResult();

            // ArchiveOperationResult provides info about the archive operation result
            var archive = e.Methods.ArchiveOperationResult;
            var deleted = (e.Methods.DeleteFiles && e.Methods.DeleteOperationResult.Deleted);

            string msgArchiveSuccess = "Archive created " + (deleted ? "and uploaded files deleted " : "") + "successfully.";
            string msgNoFiles = "No files found to archive" + (deleted ? " or delete." : ".");
            string msgDeleteSuccess = "Uploaded files deleted successfully.";


            if (archive != null)
            {
                if (archive.FilesFound && archive.Created)
                    ret = new CustomResult(archive.FilesCount, archive.DownloadUrl, archive.ArchiveName, msgArchiveSuccess, archive.CreationDate.ToString(), e.Methods.DeleteFiles);
                else
                    ret = new CustomResult(((archive.Exception == null) ? msgNoFiles : archive.Exception.Message), e.Methods.DeleteFiles);
            }
            else
            {
                if (deleted)
                    ret = new CustomResult(msgDeleteSuccess, true);
                else if (e.Methods.DeleteFiles)
                    ret = new CustomResult(msgNoFiles, false);
            }


            // In this demo we override the internal created result with a custom result object to be returned to the client as json
            sender.Result = new CoreResult(ret);
        }



        /// <summary>
        /// Called on post processing exceptions
        /// </summary>
        /// <param name="sender">Current IFilehandler instance</param>
        /// <param name="e">Event parameters</param>
        private void Events_PostProcessingException(IFileHandler sender, Backload.Contracts.Eventing.IPostProcessingEventArgs e)
        {
            // Post processing exception code here
        }


        #endregion event handler

    }



    #region Helper class

    /// <summary>
    /// Returns custom json result to the client
    /// </summary>
    public class CustomResult
    {

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="message"></param>
        public CustomResult()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public CustomResult(string message, bool deleted)
        {
            this._message = message;
            this._deleted = deleted;
        }

        public CustomResult(int count, string url, string name, string message, string created, bool deleted)
        {
            this._success = true;
            this._count = count;
            this._url = url;
            this._name = name;
            this._message = message;
            this._created = created;
            this._deleted = deleted;
        }


        private bool _success;
        public bool success { get { return _success; } set { _success = value; } }

        private int _count;
        public int count { get { return _count; } set { _count = value; } }

        private string _message = "No operation info available.";
        public string message { get { return _message; } set { _message = value; } }

        private string _url = string.Empty;
        public string url { get { return _url; } set { _url = value; } }

        private string _name = string.Empty;
        public string name { get { return _name; } set { _name = value; } }

        private string _created = string.Empty;
        public string created { get { return _created; } set { _created = value; } }

        private bool _deleted;
        public bool deleted { get { return _deleted; } set { _deleted = value; } }

    }

    #endregion Helper class

}
