using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Backload.Filesystem.Storage1.Controllers
{
    using System.IO;

    public class HomeController : Controller
    {

        // Empty view
        public ActionResult Empty()
        {
            return View();
        }


        // Start view
        public ActionResult Index()
        {
            return View();
        }


        #region jQuery File Upload Plugin

        // Basic theme (Bootstrap) 
        public ActionResult Basic()
        {
            return View();
        }

        //Basic Plus theme (Bootstrap) 
        public ActionResult BasicPlus()
        {
            return View();
        }

        // Basic Plus UI theme (Bootstrap) 
        public ActionResult BasicPlusUI()
        {
            return View();
        }

        // AngularJS theme
        public ActionResult AngularJS()
        {
            return View();
        }

        // jQuery UI theme
        public ActionResult JQueryUI()
        {
            return View();
        }

        #endregion



        #region PlUpload

        // Moxiecode PlUpload plugin simple demo
        public ActionResult PlUploadSimple()
        {
            return View();
        }

        // Moxiecode PlUpload plugin ui demo
        public ActionResult PlUploadUI()
        {
            return View();
        }

        #endregion



        #region Fine Uploader

        // Fine Uploader default demo
        public ActionResult FineUploaderDefault()
        {
            return View();
        }

        // Fine Uploader gallery demo
        public ActionResult FineUploaderGallery()
        {
            return View();
        }

        // Fine Uploader simple thumbnails demo
        public ActionResult FineUploaderSimple()
        {
            return View();
        }

        #endregion



        #region Other demos with integrated controller

        // Integrated controller with basic file chunking demo
        public ActionResult OtherChunkingBasic()
        {
            return View();
        }

        // Integrated controller with resume chunked files demo
        public ActionResult OtherChunkingResume()
        {
            return View();
        }

        // Integrated controller with file overwrite protection demo
        public ActionResult OtherChunkingAdvanced()
        {
            return View();
        }

        // Integrated controller with tracing demo
        public ActionResult OtherTracing()
        {
            return View();
        }

        // Integrated controller with tracing demo
        public ActionResult OtherCustomClient()
        {
            return View();
        }

        #endregion



        #region Custom controller

        // Custom controller with events demo
        public ActionResult CustomEvents()
        {
            return View();
        }

        // Custom controller with basic API method calls
        public ActionResult CustomAPI()
        {
            return View();
        }

        // Custom controller with database storage demo
        public ActionResult CustomDatabase()
        {
            return View();
        }

        // Custom controller with database storage demo
        public ActionResult InternalDatabase()
        {
            return View();
        }

        // Custom controller with post processing demo
        public ActionResult CustomPostProcessing()
        {
            return View();
        }

        // Custom controller with data provider demo
        public ActionResult CustomDataProvider()
        {
            return View();
        }

        // Custom controller for the WebApi controller demo
        public ActionResult CustomWebApi()
        {
            return View();
        }

        #endregion

    }
}
