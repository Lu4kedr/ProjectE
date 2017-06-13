using Backload.Bundles;
using System.Web.Optimization;

namespace Backload.Filesystem.Storage1
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            // Add or remove this line for the bundeling feature
            BackloadBundles.RegisterBundles(bundles);

        }
    }
}