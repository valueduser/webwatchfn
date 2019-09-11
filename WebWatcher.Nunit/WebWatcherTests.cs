using NUnit.Framework;
using Moq;
using WebWatcher;
using WebWatcher.Nunit;
using Microsoft.Extensions.Logging;

namespace WebWatcherTests
{
	public class WebWatcherTests
	{
        ILogger mockLogger;

        [SetUp]
		public void Setup()
		{
            mockLogger = Mock.Of<ILogger>();
        }

        [TestCase("testHtml1", "testHtml2", new string[] { "1", "2" })]
		[TestCase("testHtml2", "testHtml1", new string[] { "2", "1" })]
		[TestCase(TestValues.bigHtmlWithNonce, TestValues.bigHtmlWithDifferentNonce, new string[] { "2", "1" })]
		[TestCase(TestValues.bigHtmlWithNonce, TestValues.bigHtmlWithRealDifference, new string[] { "2", "1" })]
		public void GetHtmlDiffFromPreviousContent_Returns_Diff(string previousValue, string currentValue, string[] expectedDiff)
		{
			string noChangeDiffHtml = $"<div style='font-family: courier;'></div>";
			string expectedDiffHtml = $"<div style='font-family: courier;'><div><span style='background-color: {Constants.lightRed};'>{expectedDiff[0]}</span><span style='background-color: {Constants.lightGreen};'>{expectedDiff[1]}</span><br></div></div>";
			string actualDiffHtml = WebWatcherFunction.GetHtmlDiffFromPreviousContent(previousValue, currentValue, mockLogger);

			Assert.AreEqual(expectedDiffHtml, actualDiffHtml);
		}
	}
}