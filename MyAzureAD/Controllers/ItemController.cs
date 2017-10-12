using System;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using TaskTracker.Models;
using System.Configuration;
using Microsoft.Azure;                      // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage;       // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types

namespace TaskTracker.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private string environmentName;

        public ItemController()
        {
            environmentName = ConfigurationManager.AppSettings["environment"];
        }

        // GET: Item
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.Completed);

            var model = new ItemLisModel
            {
                List = items,
                EnvironmentName = environmentName
            };

            return View(model);
        }

        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            // TODO: get statuses
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(ItemViewModel item)
        {
            if (ModelState.IsValid)
            {               
                var model = UpdateDataModel(item);
                await DocumentDBRepository<Item>.CreateItemAsync(model);               

                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(ItemViewModel item)
        {
            if (ModelState.IsValid)
            {
                var model = UpdateDataModel(item);
                await DocumentDBRepository<Item>.UpdateItemAsync(item.Id, model);
                return RedirectToAction("Index");
            }

            return View(item);
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

            var viewModel = new ItemViewModel();
            TransferModelToViewModel(item, viewModel);

            return View(viewModel);
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
            var model = new ItemLisModel
            {
                List = items,
                EnvironmentName = environmentName
            };

            return View(model);
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

            var viewModel = new ItemViewModel();
            TransferModelToViewModel(item, viewModel);
            return View(viewModel);
        }


        #region Private methods

        private Item UpdateDataModel(ItemViewModel item)
        {
            var model = new Item();

            if (item.Completed)
            {
                item.CompletedDate = DateTime.Now;
                item.User = User.Identity.Name;

                // Write to file queue
                // Parse the connection string and return a reference to the storage account.
                var stgConnection = CloudConfigurationManager.GetSetting("StorageConnectionString");
                var queueName = CloudConfigurationManager.GetSetting("QueueName");

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(stgConnection);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container. 
                CloudQueue queue = queueClient.GetQueueReference(queueName);

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                TransferViewModelToModel(item, model);

                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model);

                CloudQueueMessage message = new CloudQueueMessage(jsonString);
                queue.AddMessage(message);
            }
            else
            {
                TransferViewModelToModel(item, model);
            }
            return model;
        }

        private static void TransferViewModelToModel(ItemViewModel item, Item model)
        {
            model.Completed = item.Completed;
            model.CompletedDate = item.CompletedDate;
            model.User = item.User;
            model.Description = item.Description;
            model.Id = item.Id;
            model.Name = item.Name;
        }

        private static void TransferModelToViewModel(Item model, ItemViewModel item)
        {
            item.Id = model.Id;
            item.Name = model.Name;
            item.Description = model.Description;
            item.Completed = model.Completed;
            item.CompletedDate = model.CompletedDate;
            item.User = model.User;
        }

        #endregion

    }

}