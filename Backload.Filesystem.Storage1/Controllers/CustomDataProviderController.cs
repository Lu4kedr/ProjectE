using Backload.Context.DataProvider;
using Backload.Contracts.Context;
using Backload.Contracts.Context.Provider;
using Backload.Contracts.FileHandler;
using Backload.Helper;
using System;
using System.Collections.Generic;
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
    public class CustomDataProviderController : Controller
    {
        /// <summary>
        /// Custom file handler. 
        /// To access it in an Javascript ajax request use: <code>var url = "/CustomDataProvider/FileHandler/";</code>.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete | HttpVerbs.Options)]
        public async Task<ActionResult> FileHandler()
        {
            try
            {
                var currentUser = UserRepository.Get("Homer");

                if (currentUser.Authenticated)
                {
                    // Create and initialize the handler
                    IFileHandler handler = Backload.FileHandler.Create();


                    /*
                        Init Backloads execution environment and execute the request
                        In this demo we do not use HttpContext.Request directory. Instead we create
                        a IBackloadDataProvider object.
                        Note: The using statement ensures that provided stream objects on uploads are disposed after 
                        internal processing finishes. If you need to use the streams after internal processing, 
                        do not use the using statement and set provider.DisposeStream=false: 
                         
                        var provider = new BackloadDataProvider();
                        provider.DisposeStream = false;
                        // Do more settings here: e.g. provider.BackloadValues.ObjectContext = currentUser.UserId;
                        handler.Init (provider);
                        IBackloadResult result = await handler.Execute();
                     */
                    using (var provider = new BackloadDataProvider(this.Request))
                    {
                        // In this demo we set ObjectContext server side to the user id.
                        // All files for this user will be stored within the users folder.
                        provider.BackloadValues.ObjectContext = currentUser.UserId;

                        // Instead of initializing the handler with a HttpRequest instance we can
                        // also initialize it with a IBackloadDataProvider instance to have full control
                        // over the parameters and variables provided to the handler 
                        // From release 2.2+ the handler is fully decoupled from an HttpRequest instance internally
                        handler.Init(provider);
                        IBackloadResult result = await handler.Execute();

                        // Helper to create an ActionResult object from the IBackloadResult instance
                        return ResultCreator.Create(result);
                    }

                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

    }



    #region Helper class


    /// <summary>
    /// Returns custom json result to the client
    /// </summary>
    internal class UserRepository
    {
        private static IList<User> _users;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="message"></param>
        static UserRepository()
        {
            _users = new List<User>(5);
            _users.Add(new User("Homer", "D3CA2E4F-03EA-45D4-81E5-FF0AABB334DC", true));
            _users.Add(new User("Lisa", "AA9355AE-9411-4EDF-A187-57F57F48A7CE", true));
            _users.Add(new User("Bart", "A1B0161E-E08F-4075-993B-4F605BB01D89", true));
            _users.Add(new User("Marge", "E0A68452-F88C-441E-8A50-61080C6FEF11", false));
            _users.Add(new User("Maggie", "0CF1BCDA-DF82-4767-80F2-48FB0F3842F6", false));
        }


        internal static User Get(string name)
        {
            var user = _users.Where(e => e.Name == name).FirstOrDefault();
            if (user == null) user = _users[0];

            return user;
        }



        internal class User
        {
            internal User(string name, string userId, bool authenticated)
            {
                this.Name = name;
                this.UserId = userId;
                this.Authenticated = authenticated;
            }

            internal string Name { get; set; }
            internal string UserId { get; set; }
            internal bool Authenticated { get; set; }
        }
    }

    #endregion Helper class

}
