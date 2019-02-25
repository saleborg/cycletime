using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThroughputCalculation.GetTheData;

namespace CycleTimeTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            new WriteToFile().uploadDocument();
            Assert.IsTrue(true);

        }
    }
}
