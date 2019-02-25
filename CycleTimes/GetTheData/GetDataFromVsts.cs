using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ThroughputCalculation.GetTheData
{
    class GetDataFromVsts
    {

        private string TOKEN = "qlo2plbf6nkmdfhu5zvxrmbkhmb3uoht4updvqls4eu4tfol6eha";
        


        public GetDataFromVsts()
        {
            IList<Deploy> Deploys;
            Console.WriteLine("URAX");
            Deploys = new DownloadData().DownloadDataFromVsts("URAX");

            var UraxLeadTime = new CalculateCycleTime().AvrageLeadtimeTime(Deploys);
            var UraxCycleTime = new CalculateCycleTime().AvrageCycleTime(Deploys);



            Console.WriteLine("Soft offer");
            Deploys = new DownloadData().DownloadDataFromVsts("SoftOffer");
            
            var SOLeadTime = new CalculateCycleTime().AvrageLeadtimeTime(Deploys);
            var SOCycleTime = new CalculateCycleTime().AvrageCycleTime(Deploys);


            Console.WriteLine("CLE");
            Deploys = new DownloadData().DownloadDataFromVsts("CLE");
       

            var CLELeadTime = new CalculateCycleTime().AvrageLeadtimeTime(Deploys);
            var CLECycleTime = new CalculateCycleTime().AvrageCycleTime(Deploys);

            

            Console.WriteLine("SoftOffer");
            Console.WriteLine("Avrage lead time in Days: " + SOLeadTime);
            Console.WriteLine("Avrage Cycle time in Days: " + SOCycleTime);


            Console.WriteLine("URAX");
            Console.WriteLine("Avrage Lead time in Days: " + UraxLeadTime);
            Console.WriteLine("Avrage Cycle time in Days: " + UraxCycleTime);

            Console.WriteLine("CLE");
            Console.WriteLine("Avrage time in Days: " + CLELeadTime);
            Console.WriteLine("Avrage Cycle time in Days: " + CLECycleTime);

            new WriteToFile().Write(SOLeadTime, SOCycleTime, UraxLeadTime, UraxCycleTime, CLELeadTime, CLECycleTime);

        }
    }
}
