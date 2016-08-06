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
        public ActionResult testAdding(Content toAdd, HttpPostedFileBase file)
        {
            var user = User.Identity.GetUserId();
            
            toAdd.PostedBy = user;

            try
            {
                // Parse the connection string and return a reference to the storage account.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobConnection"));
                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("imagecontainer");

                string imageName = String.Format("postedhosted-{0}{1}",
                    Guid.NewGuid().ToString(),
                    Path.GetExtension(file.FileName));

                // Create the container if it doesn't already exist.
                //container.CreateIfNotExists();
                //container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("meowblob");

                // Create or overwrite the "myblob" blob with contents from a local file.
                blockBlob.Properties.ContentType = file.ContentType;
                blockBlob.UploadFromStream(file.InputStream);

                string fullpath = String.Format("http://{0}{1}", blockBlob.Uri.DnsSafeHost, blockBlob.Uri.AbsolutePath);

                toAdd.ImgURL = fullpath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                phdbec.Contents.Add(toAdd);
                phdbec.SaveChanges();
            }
            return View();
        }
    }
}