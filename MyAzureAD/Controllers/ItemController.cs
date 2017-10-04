using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using MyAzureAD.Models;
using System.Configuration;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types

namespace MyAzureAD.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.Completed);
            ViewDataInformation();
            return View(items);
        }

        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {               
                UpdateCompleteDate(item);
                await DocumentDBRepository<Item>.CreateItemAsync(item);               

                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                UpdateCompleteDate(item);
                await DocumentDBRepository<Item>.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        private void UpdateCompleteDate(Item item)
        {
            if (item.Completed)
            {
                item.CompletedDate = DateTime.Now;
                item.User = User.Identity.Name;

                // Write to file queue
                // Parse the connection string and return a reference to the storage account.
                CloudStorageAccount storageAccount =
                    CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                CloudQueue queue = queueClient.GetQueueReference("todo");

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(item);

                CloudQueueMessage message = new CloudQueueMessage(jsonString);
                queue.AddMessage(message);
            }
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {

            await DocumentDBRepository<Item>.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }

        // GET: Item
        [ActionName("Completed")]
        public async Task<ActionResult> CompleteAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => d.Completed);
            ViewDataInformation();
            return View(items);
        }


        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        private void ViewDataInformation()
        {
            ViewData["environment"] = ConfigurationManager.AppSettings["environment"];
        }
    }
}