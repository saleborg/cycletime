using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThroughputCalculation.GetTheData
{
    class DownloadData
    {
        private const string SoftOfferDefinitionId = "4";
        private const string SoftOfferWebDefinitionId = "2";

        private const string UraxDefinitionId = "14";

        private const string CLE_PartnerGroupFeed = "18";
        private const string CLE_PartnerFeedRDM = "16";
        private const string CLE_FindOrder = "21";
        private const string CLE_DistanceCalculator = "15";
        private const string GOF = "20";
        private const string CLE_StockCarLive = "19";


       


        public IList<Deploy> DownloadDataFromVsts(string product)
        {
            var request = new RestRequest(Method.GET);
            request.AddHeader("Postman-Token", "34838981-39b5-4248-b4cb-045323c1e71a");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Authorization", "Basic OnFsbzJwbGJmNm5rbWRmaHU1enZ4cm1ia2htYjN1b2h0NHVwZHZxbHM0ZXU0dGZvbDZlaGE=");

            List<Deploy> allDeploys = new List<Deploy>();
            if (product.Equals("SoftOffer"))
            {
                var deployments = getDeployments(request, SoftOfferWebDefinitionId);
                allDeploys = deployments.Concat(getDeployments(request, SoftOfferDefinitionId)).ToList<Deploy>();
                Console.WriteLine("Number of prod deploys Soft Offer: " + allDeploys.Count);
            } else if (product.Equals("URAX"))
            {
                allDeploys = getDeployments(request, UraxDefinitionId).ToList();
                Console.WriteLine("Number of prod deploys URAX: " + allDeploys.Count);
            }

            else if (product.Equals("CLE"))
            {
                List<Deploy> deployments = getDeployments(request, CLE_PartnerGroupFeed).ToList<Deploy>();
                allDeploys.AddRange(deployments);

                deployments = getDeployments(request, CLE_PartnerFeedRDM).ToList<Deploy>();
                allDeploys.AddRange(deployments);

                
                deployments = getDeployments(request, CLE_FindOrder).ToList<Deploy>();
                allDeploys.AddRange(deployments);

                deployments = getDeployments(request, CLE_DistanceCalculator).ToList<Deploy>();
                allDeploys.AddRange(deployments);


                deployments = getDeployments(request, GOF).ToList<Deploy>();
                allDeploys.AddRange(deployments);

                deployments = getDeployments(request, CLE_StockCarLive).ToList<Deploy>();
                allDeploys.AddRange(deployments);
                
                Console.WriteLine("Number of prod deploys CLE: " + allDeploys.Count);
            }


            List<string> buildIds = new List<string>();
            List<Deploy> removeDep = new List<Deploy>();
            foreach (Deploy dep in allDeploys)
            {
                Console.WriteLine("Build Id " + dep.buildId);
              
                if (!buildIds.Contains(dep.buildId))
                {
                    buildIds.Add(dep.buildId);
                    
                    GetItemsInBuild(request, dep);
                }
                else
                {
                    removeDep.Add(dep);
                }
            }
            
            foreach(Deploy dp in removeDep)
            {
                allDeploys.Remove(dp);
            }

            return allDeploys;
        }


     


        private Dictionary<string, object> getWorkItemRevisions(RestRequest request, string url)
        {
            var client = new RestClient(url);
            IRestResponse response = client.Execute(request);
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
            return values;
        }

        private void GetItemsInBuild(RestRequest request, Deploy dep)
        {
            var client = new RestClient("https://volvocargroup.visualstudio.com/DSPA/_apis/build/builds/" + dep.buildId + "/workitems?api-version=4.1");
            IRestResponse response = client.Execute(request);
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
             if (values.TryGetValue("value", out object itemsObj))
            {
                JArray items = (JArray)itemsObj;
                foreach (JToken re in items)
                {

                    var url = re.Value<string>("url") + "?$expand=relations";
                    client = new RestClient(url);
                    IRestResponse responseItem = client.Execute(request);
                    Dictionary<string, object> itemsreturned = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseItem.Content);
                   
                    object itemArray;
                    if (itemsreturned.TryGetValue("fields", out itemArray))
                    {
                        var it = (JObject)itemArray;
                        Item item = new Item();
                        item.Id = re.Value<string>("id");
                        item.SystemState = it.Value<string>("System.State");
                        item.WorkItemType = it.Value<string>("System.WorkItemType");
                        var CreatedDate = it.Value<string>("System.CreatedDate");
                        var date = DateTime.Parse(CreatedDate);
                        GetCommitedDateForItem(request, item);
                        if(item.commitedDate.Equals(new DateTime()))
                        {
                            item.commitedDate = date;
                        }
                        item.CreatedDate = date;
                        dep.items.Add(item);
                    }
                }
            }
        }

        private void GetCommitedDateForItem(RestRequest request, Item item)
        {

            string URL = "https://volvocargroup.visualstudio.com/2bc271a6-ecdf-46eb-946b-bd490f89dd42/_apis/wit/workItems/" + item.Id + "/updates";

            var client = new RestClient(URL);
            IRestResponse response = client.Execute(request);
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

            if (values.TryGetValue("value", out object releaseObj))
            {
                var revosionsTemp = (JArray)releaseObj;
                bool breaked = false;
                foreach (JToken jt in revosionsTemp) {
                    DateTime revisedDate = DateTime.Parse(jt.Value<string>("revisedDate"));
                    var fields = jt.Value<JObject>("fields");
                    if (fields != null) { 
                        foreach (KeyValuePair<string, JToken> property in fields) 
                        {
                            if (property.Key.Equals("System.State"))
                            {
                                string value = "{\r\n  \"oldValue\": \"New\",\r\n  \"newValue\": \"Committed\"\r\n}";
                                if (value.Equals(property.Value.ToString()))
                                {

                                    item.commitedDate = revisedDate;
                                    breaked = true;
                                    break;
                                }

                            }
                        }
                    }
                    if (breaked)
                    {
                        break;
                    }
                }
            }
        }


        private IList<Deploy> getDeployments(RestRequest request, string definitionId)
        {
            IList<Deploy> deployments = new List<Deploy>();
            var client = new RestClient("https://volvocargroup.vsrm.visualstudio.com/DSPA/_apis/release/deployments?api-version=5.0&$top=10000&definitionId=" + definitionId);
            IRestResponse response = client.Execute(request);
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

            if (values.TryGetValue("value", out object releaseObj))
            {
                var releaseTemp = (JArray)releaseObj;
                foreach (JToken re in releaseTemp)
                {
                    var environment = re.Value<JObject>("releaseEnvironment").Value<string>("name");
                    if (environment.ToUpper().Equals("PROD") && re.Value<string>("deploymentStatus").ToLower().Equals("succeeded"))
                    {

                        Deploy newDeploy = new Deploy();
                        newDeploy.releaseEnvironment = environment;
                        newDeploy.id = re.Value<string>("id");
                        newDeploy.releaseId = re.Value<JObject>("release").Value<string>("id");
                        newDeploy.releaseName = re.Value<JObject>("release").Value<string>("name");
                        Console.WriteLine(newDeploy.releaseName);
                        newDeploy.createdOn = DateTime.Parse(re.Value<string>("queuedOn"));
                        newDeploy.completedOn = DateTime.Parse(re.Value<string>("completedOn"));

                        newDeploy.operationStatus = re.Value<string>("operationStatus");
                        newDeploy.deploymentStatus = re.Value<string>("deploymentStatus");


                        var artifactArray = re.Value<JObject>("release").Value<JArray>("artifacts");
                        foreach (JToken token in artifactArray)
                        {
                            if (token.Value<string>("type").Equals("Build"))
                            {
                                newDeploy.buildId = token.Value<JObject>("definitionReference").Value<JToken>("version").Value<string>("id");
                            }
                        }
                        deployments.Add(newDeploy);
                    }
                }
            }
            return deployments;

        }

    }
}
