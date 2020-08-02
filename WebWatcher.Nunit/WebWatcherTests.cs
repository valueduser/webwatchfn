using NUnit.Framework;
using Moq;
using WebWatcher;
using WebWatcher.Nunit;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using static WebWatcher.WebWatcherFunction;
using Newtonsoft.Json;
using System;

namespace WebWatcherTests
{
    public class WebWatcherTests
    {
        ILogger mockLogger;
        //List<WebDiff> expectedDiff1 = new List<WebDiff> { new WebDiff(DiffLib.DiffOperation.Modify, "1", "2") };

        public static WebDiff testDiff1 = new WebDiff(DiffLib.DiffOperation.Modify, "1", "2");

        public static List<WebDiff> expectedDiff1 = new List<WebDiff>
        {
            testDiff1
        };



        [SetUp]
        public void Setup()
        {
            mockLogger = Mock.Of<ILogger>();
        }

        [TestCase("testHtml1", "testHtml2", DiffLib.DiffOperation.Modify, "1", "2")]
        [TestCase("testHtml2", "testHtml1", DiffLib.DiffOperation.Modify, "2", "1")]
        [TestCase(TestConstants.bigHtmlWithNonce, TestConstants.bigHtmlWithDifferentNonce, DiffLib.DiffOperation.Modify, "jkl", "zxcvbnn")] // jklmnopqr vs zxcvbnmn
        //[TestCase(TestConstants.bigHtmlWithNonce, TestConstants.bigHtmlWithRealDifference, DiffLib.DiffOperation.Modify, "2", "1" )]
        public void GetDiff_Returns_Diff(string previousValue, string currentValue,
            DiffLib.DiffOperation expectedOperation, string expectedCollection1, string expectedCollection2)
		{

            List<WebDiff> expectedDiff = new List<WebDiff> { new WebDiff(expectedOperation, expectedCollection1, expectedCollection2) };
			//string noChangeDiffHtml = $"<div style='font-family: courier;'></div>";
			//string expectedDiffHtml = $"<div style='font-family: courier;'><div><span style='background-color: {Constants.lightRed};'>{expectedDiff[0]}</span><span style='background-color: {Constants.lightGreen};'>{expectedDiff[1]}</span><br></div></div>";
			List<WebDiff> actualDiff = WebWatcherFunction.GetDiff(previousValue, currentValue, mockLogger);

            string output = JsonConvert.SerializeObject(actualDiff);
            Console.WriteLine(output);

            for(int i = 0; i < expectedDiff.Count; i++)
            {
                Assert.IsTrue(expectedDiff[i].Equals(actualDiff[i]));
            }
        }
	}
}