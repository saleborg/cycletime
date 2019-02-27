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
        public GetDataFromVsts()
        {
            IList<Deploy> Deploys;
            Console.WriteLine("URAX");
            Deploys = new DownloadData().DownloadDataFromVsts("URAX");

            var UraxLeadTime = new CalculateCycleTime().AvrageLeadtimeTime(Deploys, false);
            var UraxCycleTime = new CalculateCycleTime().AvrageCycleTime(Deploys, false);
            var UraxLeadTimeBug = new CalculateCycleTime().AvrageLeadtimeTime(Deploys, true);
            var UraxCycleTimeBug = new CalculateCycleTime().AvrageCycleTime(Deploys, true);


            Console.WriteLine("Soft offer");
            Deploys = new DownloadData().DownloadDataFromVsts("SoftOffer");
            
            var SOLeadTime = new CalculateCycleTime().AvrageLeadtimeTime(Deploys, false);
            var SOCycleTime = new CalculateCycleTime().AvrageCycleTime(Deploys, false);
            var SOLeadTimeBug = new CalculateCycleTime().AvrageLeadtimeTime(Deploys, true);
            var SOCycleTimeBug = new CalculateCycleTime().AvrageCycleTime(Deploys, true);

            Console.WriteLine("CLE");
            Deploys = new DownloadData().DownloadDataFromVsts("CLE");
       

            var CLELeadTime = new CalculateCycleTime().AvrageLeadtimeTime(Deploys, false);
            var CLECycleTime = new CalculateCycleTime().AvrageCycleTime(Deploys, false);
            var CLELeadTimeBug = new CalculateCycleTime().AvrageLeadtimeTime(Deploys, true);
            var CLECycleTimeBug = new CalculateCycleTime().AvrageCycleTime(Deploys, true);


            Console.WriteLine("SoftOffer");
            Console.WriteLine("Avrage lead time in Days: " + SOLeadTime);
            Console.WriteLine("Avrage Cycle time in Days: " + SOCycleTime);
            Console.WriteLine("Avrage lead time in Days (high and critical): " + SOLeadTimeBug);
            Console.WriteLine("Avrage Cycle time in Days (high and critical): " + SOCycleTimeBug);


            Console.WriteLine("URAX");
            Console.WriteLine("Avrage Lead time in Days: " + UraxLeadTime);
            Console.WriteLine("Avrage Cycle time in Days: " + UraxCycleTime);
            Console.WriteLine("Avrage Lead time in Days (high and critical): " + UraxLeadTimeBug);
            Console.WriteLine("Avrage Cycle time in Days (high and critical): " + UraxCycleTimeBug);

            Console.WriteLine("CLE");
            Console.WriteLine("Avrage time in Days: " + CLELeadTime);
            Console.WriteLine("Avrage Cycle time in Days: " + CLECycleTime);
            Console.WriteLine("Avrage time in Days (high and critical): " + CLELeadTimeBug);
            Console.WriteLine("Avrage Cycle time in Days Bugs (high and critical): " + CLECycleTimeBug);

            new WriteToFile().Write(SOLeadTime, SOCycleTime, UraxLeadTime, UraxCycleTime, CLELeadTime, CLECycleTime);

        }
    }
}
