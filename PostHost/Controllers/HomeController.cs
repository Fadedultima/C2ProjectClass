﻿using System;
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
using System.Web.Routing;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace PostHost.Controllers
{ 
    //this is a test
    public class HomeController : Controller
    {
        static List<Content> Gallerylist;
        public ActionResult Index(string searchtype, string searchstring = null)
        {
            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                if (searchstring == null)
                {
                    Gallerylist = phdbec.Contents.ToList();
                    return View(Gallerylist);
                }
                else
                {
                    if (searchtype == "Title")
                    {
                        Gallerylist = phdbec.Contents.Where(s => s.Title.ToUpper().Contains(searchstring.ToUpper())).ToList();
                        return View(Gallerylist);
                    }
                    else
                    {
                        List<Content> unicon = phdbec.Contents.ToList();
                        Tag t = phdbec.Tags.Where(s => s.TagTitle.ToUpper().Contains(searchstring.ToUpper())).First();
                        var ttc = phdbec.TagToContents.Where(p => p.T_Id == t.T_Id);
                        var glist = from c in phdbec.Contents
                                    join tc in ttc on c.C_Id equals tc.C_Id into newlist
                                    from nl in newlist
                                    where nl.C_Id == c.C_Id
                                    select c;
                        Gallerylist = glist.ToList();
                        return View(Gallerylist);
                    }
                }
            }
        }

        public ActionResult UserProfile()
        {
            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                string user = User.Identity.GetUserId();
                List<Content> userposts = phdbec.Contents.Where(n => n.PostedBy == user).ToList();
                return View(userposts);
            }
        }

        //CODE FOR CREATING A COMMENT - ALEX
        [Authorize]
        public ActionResult CommentCreator(string comment, long C_id)
        {
            Comment comm = new Comment();
            comm.Comment_Id = (int)LongGenerator();
            comm.User_Comment = comment;
            comm.Content_Id = C_id;
            comm.Posted_Username = User.Identity.GetUserName();
            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                phdbec.Comments.Add(comm);
                try
                {
                    phdbec.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }
            }
            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }

        [Authorize]
        public ActionResult testAdding()
        {
            List<Tag> thetags = new List<Tag>();
            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                thetags = phdbec.Tags.ToList();

            }

            List<TaggerViewModel> tvm = new List<TaggerViewModel>();

            foreach (Tag t in thetags)
            {
                tvm.Add(new TaggerViewModel() { slatedId = t.T_Id, slatedTitle = t.TagTitle });
            }

            ContentViewModels cvm = new ContentViewModels();

            cvm.theTags = tvm;

            return View("TestAddingBETTER", cvm);
        }

        [HttpPost]
        public ActionResult TestAdding( ContentViewModels toAddVM, HttpPostedFileBase file )
        {
            Content toAdd = toAddVM.theContent;

            string filename = Guid.NewGuid().ToString();
            string im = "https://posthoststorage.blob.core.windows.net/imagecontainer/" + filename;
            var user = User.Identity.GetUserId();

            toAdd.PostedBy = user;
            toAdd.C_Id = LongGenerator();

            try
            {
                // Parse the connection string and return a reference to the storage account.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobConnection"));
                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("imagecontainer");

                //string imageName = String.Format("postedhosted-{0}{1}",
                //    Guid.NewGuid().ToString(),
                //    Path.GetExtension(file.FileName));

                // Create the container if it doesn't already exist.
                //container.CreateIfNotExists();
                //container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);

                // Create or overwrite the "myblob" blob with contents from a local file.
                //blockBlob.Properties.ContentType = file.ContentType;
                //blockBlob.UploadFromStream(file.InputStream);

                //string fullpath = "https://posthoststorage.blob.core.windows.net/imagecontainer/" + imageName;



                blockBlob.UploadFromStream(file.InputStream);
                toAdd.ImgURL = im;
                /*WebRequest req = WebRequest.Create(toAdd.ImgURL);
                using (Stream stream = req.GetResponse().GetResponseStream())
                {
                    blockBlob.UploadFromStream(stream);
                    toAdd.ImgURL = im;
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {

                phdbec.Contents.Add(toAdd);
                foreach (TaggerViewModel tvm in toAddVM.theTags)
                {
                    if (tvm.ischecked)
                    {
                        phdbec.TagToContents.Add(new TagToContent { C_Id = toAdd.C_Id, T_Id =  tvm.slatedId});
                    }
                }
                try
                {
                    phdbec.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult ViewSingle(long C_Id)
        {
            ContentViewModels cvm = new ContentViewModels();
            Boolean shownextbutton;
            Boolean showprevbutton;
            Content red = null;
            Content next = null;
            Content prev = null;
            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                cvm.theContent = phdbec.Contents.Find(C_Id);

                LikeViewModels LikeVM = new LikeViewModels();
                LikeVM.contentId = cvm.theContent.C_Id;
                LikeVM.theLikes = cvm.theContent.Likes;
                LikeVM.theDislikes = cvm.theContent.Dislikes;

                string useid = User.Identity.GetUserId();

                if (phdbec.Likes.Any(l => l.UserId == useid && l.ContentId == cvm.theContent.C_Id))
                {
                    if (phdbec.Likes.Where(r => r.UserId == useid && r.ContentId == cvm.theContent.C_Id).First().OneOrOther)
                    {
                        LikeVM.hasLiked = true;
                        LikeVM.hasDisliked = false;
                    }
                    else
                    {
                        LikeVM.hasDisliked = true;
                        LikeVM.hasLiked = false;
                    }
                }
                else
                {
                    LikeVM.hasLiked = false;
                    LikeVM.hasDisliked = false;
                }

                cvm.LVM = LikeVM;

                cvm.theComments = phdbec.Comments.Where(c => c.Content_Id == C_Id).ToList();

                var tagids = from t in phdbec.Tags
                             join tc in phdbec.TagToContents on t.T_Id equals tc.T_Id into tagGroup
                             from ta in tagGroup
                             where ta.C_Id == C_Id
                             select t;

                List<TaggerViewModel> tvm = new List<TaggerViewModel>();

                foreach(Tag t in tagids)
                {
                    tvm.Add(new TaggerViewModel() { slatedId = t.T_Id, slatedTitle = t.TagTitle });
                }

                cvm.theTags = tvm;

                var list = Gallerylist;
                red = phdbec.Contents.Find(C_Id);
                var currentcontent = Gallerylist.Where( p => p.C_Id == red.C_Id).First();
                int curIndex = list.IndexOf(currentcontent);
                if (curIndex == (list.Count() - 1))
                {
                    shownextbutton = false;
                    next = currentcontent;
                }
                else
                {
                    shownextbutton = true;
                    next = list[curIndex + 1];
                }
                if (curIndex == 0)
                {
                    prev = currentcontent;
                    showprevbutton = false;

                }
                else
                {
                    prev = list[curIndex - 1];
                    showprevbutton = true;
                }
                TempData["prevId"] = prev.C_Id;
                TempData["nextId"] = next.C_Id;
                TempData["shownextbutton"] = shownextbutton;
                TempData["showprevbutton"] = showprevbutton;
            }

            return View(cvm);
        }

        public ActionResult TagManager()
        {
            IEnumerable<Tag> thetags = new List<Tag>();
            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                thetags = phdbec.Tags.ToList();
            }
            return View(thetags);
        }

        [HttpPost]
        public ActionResult TagCreator(string newTT)
        {

            IEnumerable<Tag> thetags = new List<Tag>();
            using (PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                Tag tagObj = new Tag();
                tagObj.TagTitle = newTT;
                phdbec.Tags.Add(tagObj);

                try
                {
                    phdbec.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    System.Console.Write(dbEx);
                }

                thetags = phdbec.Tags.ToList();
            }
            return RedirectToAction("TagManager");//View("TagManager");
        }
        
        [Authorize]
        public ActionResult likeModifier(int value, long toMod)
        {
            LikeViewModels lvm = new LikeViewModels();

            using(PostHostDBEntities phdbec = new PostHostDBEntities())
            {
                Content toUpdate = phdbec.Contents.Where(x => x.C_Id == toMod).First();
                if (value == 1)
                {
                    Like l = new Like() { UserId = User.Identity.GetUserId(), ContentId = toMod, OneOrOther = true };
                    if (phdbec.Likes.Any(tl => tl.UserId == l.UserId && tl.ContentId == l.ContentId))
                    {
                        Like exLike = phdbec.Likes.Where(xl => xl.UserId == l.UserId && xl.ContentId == l.ContentId).First();
                        exLike.OneOrOther = true;
                        toUpdate.Dislikes--;
                    }
                    else
                    {
                        phdbec.Likes.Add(l);
                    }
                    toUpdate.Likes++;
                }
                else if (value == -1)
                {
                    Like d = new Like() { UserId = User.Identity.GetUserId(), ContentId = toMod, OneOrOther = false };
                    if (phdbec.Likes.Any(td => td.UserId == d.UserId && td.ContentId == d.ContentId))
                    {
                        Like exDislike = phdbec.Likes.Where(xd => xd.UserId == d.UserId && xd.ContentId == d.ContentId).First();
                        exDislike.OneOrOther = false;
                        toUpdate.Likes--;
                    }
                    else
                    {
                        phdbec.Likes.Add(d);
                    }
                    toUpdate.Dislikes++;
                }
                phdbec.SaveChanges();

                lvm.contentId = toMod;
                lvm.theLikes = toUpdate.Likes;
                lvm.theDislikes = toUpdate.Dislikes;
            }
            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            //return PartialView("_LikePartialView", lvm);
        }

        private long LongGenerator()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();

            return BitConverter.ToInt64(buffer, 0);
        }
    }
}