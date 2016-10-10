using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using System.Web;
using Xrm.Tools.WebAPI;

namespace CrmChatBot.CRM
{
    [Serializable]
    public class CrmDataConnection
    {
        public static IOrganizationService GetOrgService()
        {
            var soapOrgUrl = ConfigurationManager.AppSettings["CrmServerUrl"].ToString() + "/XRMServices/2011/Organization.svc";
            var username = ConfigurationManager.AppSettings["CrmUsername"].ToString();
            var password = ConfigurationManager.AppSettings["CrmPassword"].ToString();

            var credentials = new ClientCredentials();
            credentials.UserName.UserName = username;
            credentials.UserName.Password = password;
            var serviceUri = new Uri(soapOrgUrl);
            var proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
            proxy.EnableProxyTypes();
            return proxy;
        }

        public static CRMWebAPI GetAPI()
        {
            string authority = ConfigurationManager.AppSettings["AdOath2AuthEndpoint"];// "https://login.microsoftonline.com/common";
            string clientId = ConfigurationManager.AppSettings["AdClientId"];
            string crmBaseUrl = ConfigurationManager.AppSettings["CrmServerUrl"];

            var authContext = new AuthenticationContext(authority);
            UserCredential userCreds = new UserCredential(ConfigurationManager.AppSettings["CrmUsername"], ConfigurationManager.AppSettings["CrmPassword"]);
            var result = authContext.AcquireToken(crmBaseUrl, clientId, userCreds);
            CRMWebAPI api = new CRMWebAPI(crmBaseUrl + "/api/data/v8.1/", result.AccessToken);

            return api;
        }

        public static async Task<HttpResponseMessage> CrmWebApiRequest(string apiRequest, HttpContent requestContent, string requestType)
        {
            AuthenticationContext authContext = new AuthenticationContext(ConfigurationManager.AppSettings["AdOath2AuthEndpoint"], false);
            UserCredential credentials = new UserCredential(ConfigurationManager.AppSettings["CrmUsername"],
                ConfigurationManager.AppSettings["CrmPassword"]);
            AuthenticationResult tokenResult = authContext.AcquireToken(ConfigurationManager.AppSettings["CrmServerUrl"],
                ConfigurationManager.AppSettings["AdClientId"], credentials);
            HttpResponseMessage apiResponse;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["CrmServerUrl"]);
                httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.AccessToken);

                if (requestType == "retrieve")
                {
                    apiResponse = await httpClient.GetAsync(apiRequest);
                }
                else if (requestType == "create")
                {
                    apiResponse = await httpClient.PostAsync(apiRequest, requestContent);
                }
                else
                {
                    apiResponse = null;
                }
            }
            return apiResponse;
        }
    }
}