using System.IdentityModel.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

//  Add these KeyVault using statements
// using Microsoft.Azure.KeyVault;
// using System.Web.Configuration;

namespace TaskTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            // I put my GetToken method in a Utils class. Change for wherever you placed your method.
            //var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(EncryptionHelper.GetToken));
            //var dbEndPoint = kv.GetSecretAsync(WebConfigurationManager.AppSettings["DocumentDbName"]).Result;
            //var dbKey = kv.GetSecretAsync(WebConfigurationManager.AppSettings["DocumentDbValue"]).Result;

            ////I put a variable in a Utils class to hold the secret for general  application use.
            //EncryptionHelper.DbEndPoint = dbEndPoint.Value;
            //EncryptionHelper.DBAcessKey = dbKey.Value;

            DocumentDBRepository<TaskTracker.Models.Item>.Initialize();

        }
    }
}
