using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Widgets;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Auth;
using WidgetRefactuatorSite.Models;
using System.Configuration;

namespace WidgetRefactuatorSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Refactuator(RefactuateModel model)
        {
            string filename = model.FileName;
            Widget widget = new Widget(filename);
            ViewBag.FileName = filename;
            try
            {

                widget.Refactuate();

                if (filename.Length > 0)
                {
                    string accountName = ConfigurationManager.AppSettings["StorageAccount"];
                    string accountKey = ConfigurationManager.AppSettings["StorageAccountKey"];

                    StorageCredentials credentials = new StorageCredentials(accountName, accountKey);
                    CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, useHttps: true);

                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference("refactuatedwidgetscontainer");

                    bool Refactuated = await widget.SendToBlobStorage(container);
                }
            }

            catch (Exception ex)
            {
                widget.Defactuate(ex);
            }
            return View("Index", model);
        }
    }
}