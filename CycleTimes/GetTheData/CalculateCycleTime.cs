using System;
using System.Collections.Generic;

namespace ThroughputCalculation.GetTheData
{
    internal class CalculateCycleTime
    {
        public CalculateCycleTime()
        {
        }

        internal double AvrageLeadtimeTime(IList<Deploy> deploys)
        {
            TimeSpan avrageTot = new TimeSpan();

            foreach(Deploy dp in deploys)
            {
                TimeSpan diffTotal = new TimeSpan();

                foreach(Item it in dp.items)
                {
                    diffTotal = diffTotal.Add(dp.completedOn.Subtract(it.CreatedDate));
                }

                if (dp.items.Count > 0)
                {
                    avrageTot = avrageTot.Add(diffTotal.Divide(dp.items.Count));
                }

            }

            return avrageTot.Divide(deploys.Count).TotalDays;
        }

        internal double AvrageCycleTime(IList<Deploy> deploys)
        {

            TimeSpan avrageTot = new TimeSpan();
            foreach (Deploy dp in deploys)
            {
                TimeSpan diffTotal = new TimeSpan();
                foreach (Item it in dp.items)
                {
                    diffTotal = diffTotal.Add(dp.completedOn.Subtract(it.commitedDate));
                }

                if (dp.items.Count > 0)
                {
                    avrageTot = avrageTot.Add(diffTotal.Divide(dp.items.Count));
                }

            }

            return avrageTot.Divide(deploys.Count).TotalDays;


        }
    }
}