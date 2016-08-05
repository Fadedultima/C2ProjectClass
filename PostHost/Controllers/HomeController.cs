using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PostHost.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.Net;
using System.IO;

namespace PostHost.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobConnection"));
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("imagecontainer");

            // Create the container if it doesn't already exist.
            //container.CreateIfNotExists();
            //container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("meowblob");

            // Create or overwrite the "myblob" blob with contents from a local file.
            WebRequest req = WebRequest.Create("http://i2.kym-cdn.com/photos/images/facebook/001/070/061/d96.jpg");
            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                blockBlob.UploadFromStream(stream);
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult testAdding()
        {
            return View();
        }
        [HttpPost]
        public ActionResult testAdding(Content toAdd)
        {
            var user = User.Identity.GetUserId();
            
            toAdd.PostedBy = user;


            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                phdbec.Contents.Add(toAdd);
                phdbec.SaveChanges();
            }
            return View();
        }
    }
}