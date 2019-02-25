using System;
using System.Collections.Generic;

namespace ThroughputCalculation.GetTheData
{
    internal class CalculateCycleTime
    {
        public CalculateCycleTime()
        {
        }

        internal double AvrageLeadtimeTime(IList<Deploy> deploys, bool onlyCriticalHighBugs)
        {
            TimeSpan avrageTot = new TimeSpan();

            int count = 0;
            foreach(Deploy dp in deploys)
            {
                TimeSpan diffTotal = new TimeSpan();

                foreach(Item it in dp.items)
                {
                    if (onlyCriticalHighBugs)
                    {
                        if (it.WorkItemType.Equals("Bug") && (it.severity.Equals("2 - High") || it.severity.Equals("1 - Critical")))
                        {
                            count++;
                            diffTotal = diffTotal.Add(dp.completedOn.Subtract(it.CreatedDate));
                        }
                        
                    }
                    else
                    {
                        count++;
                        diffTotal = diffTotal.Add(dp.completedOn.Subtract(it.CreatedDate));
                    }
                }

                if (dp.items.Count > 0)
                {
                    
                    avrageTot = avrageTot.Add(new TimeSpan (diffTotal.Ticks / dp.items.Count));
                }

            }
            Console.WriteLine("Number of Items: " + count);
            if(count == 0)
            {
                count++;
            }
            return new TimeSpan(avrageTot.Ticks / count).TotalDays;
        }

        internal double AvrageCycleTime(IList<Deploy> deploys, bool onlyCriticalHighBugs)
        {

            TimeSpan avrageTot = new TimeSpan();
            int count = 0;
            foreach (Deploy dp in deploys)
            {
                TimeSpan diffTotal = new TimeSpan();
                foreach (Item it in dp.items)
                {

                    if (onlyCriticalHighBugs)
                    {
                        if (it.WorkItemType.Equals("Bug") && (it.severity.Equals("2 - High") || it.severity.Equals("1 - Critical")))
                        {
                            count++;
                            diffTotal = diffTotal.Add(dp.completedOn.Subtract(it.CreatedDate));
                        }

                    }
                    else
                    {
                        count++;
                        diffTotal = diffTotal.Add(dp.completedOn.Subtract(it.commitedDate));
                    }
                }

                if (dp.items.Count > 0)
                {
                    
                    avrageTot = avrageTot.Add(new TimeSpan(diffTotal.Ticks / dp.items.Count));
                }

            }
            if (count == 0)
            {
                count++;
            }
            return new TimeSpan(avrageTot.Ticks / count).TotalDays; ;


        }
    }
}