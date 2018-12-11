using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebDriverExtensions;

namespace Selenium_UnitTests
{
  [TestClass]
  public class IndieHackersUnitTest
  {
    ChromeDriver driver;
    WebDriverWait webDriverWait;
    List<Interview> interviews;
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    [TestInitialize]
    public void Startup()
    {
      var chromeOptions = new ChromeOptions();
      chromeOptions.AddArguments("--proxy-server='direct://'");
      chromeOptions.AddArguments("--proxy-bypass-list=*");
      chromeOptions.AddArguments("--start-maximized");
      chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
      chromeOptions.AddArguments("headless");

      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(path);
      driver = new ChromeDriver(chromeDriverService, chromeOptions);
      webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }
    [TestCleanup]
    public void CleanUp()
    {
      driver.Quit();
    }
    [TestMethod]
    public void GetIndieHackersInterviewsDetails()
    {
      interviews = new List<Interview>();
      try
      {
        for(int pageNo = 1; pageNo < 18; pageNo++)
        {
          GetPageDetails(pageNo);
        }
        //log.Info(JsonConvert.SerializeObject(interviews, Formatting.Indented));
        GetEachInterviewDetails();
      }
      catch(Exception exception)
      {
        log.Error(exception);
      }
      //log.Info(JsonConvert.SerializeObject(interviews, Formatting.Indented));
    }

    public void GetPageDetails(int pageNo)
    {
      driver.GoToUrlAndWaitTillPageLoaded(webDriverWait, $"https://www.indiehackers.com/interviews/page/{pageNo}?minRevenue=1");
      var iJavaScriptExecutor = (IJavaScriptExecutor)driver;      
      foreach (var interviewcounter in webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
        By.XPath("//div[@class='interviews__interview']"))))
      {
        var interviewLinkElement = interviewcounter.FindElementIgnoreStaleElementReferenceException2(webDriverWait,
          By.CssSelector("a"));
        var interviewLink = interviewLinkElement.GetSafeAttribute("href");

        var textElement = interviewcounter.FindElementIgnoreStaleElementReferenceException2(webDriverWait,
          By.CssSelector("a > div.interview__text > h4"));
        var text = textElement.GetSafeText();

        var productNameElement = interviewcounter.FindElementIgnoreStaleElementReferenceException2(webDriverWait,
          By.CssSelector("a > div.interview__product-wrapper > div > p.interview__product-name"));
        var productName = productNameElement.GetSafeText();

        var tagLineElement = interviewcounter.FindElementIgnoreStaleElementReferenceException2(webDriverWait,
          By.CssSelector("a > div.interview__product-wrapper > div > p.interview__product-tagline"));
        var tagLine = tagLineElement.GetSafeText();

        var interview = new Interview
        {
          Text = text,
          ProductName = productName,
          TagLine = tagLine,
          InterviewLink= interviewLink
        };
        log.Info(JsonConvert.SerializeObject(interview, Formatting.Indented));
        interviews.Add(interview);
      }
    }

    public void GetEachInterviewDetails()
    {
      foreach (var interviewcounter in interviews)
      {
        driver.GoToUrlAndWaitTillPageLoaded(webDriverWait, interviewcounter.InterviewLink);

        string productMetrics = string.Empty;
        try
        {
          var productMetricsElement = webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
          By.CssSelector("a[class='product-metrics__stat product-metrics__stat--revenue ember-view']")));
          productMetrics = productMetricsElement.First().GetSafeText();
        }
        catch(Exception exception)
        {
          productMetrics = string.Empty;
        }

        var websiteElement = webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
          By.CssSelector("a[class='product-metrics__stat product-metrics__stat--website']")));
        var website = websiteElement.First().GetSafeAttribute("href");

        var articleElement = webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
          By.CssSelector("div[class='interview-body ember-view']")));
        var article = articleElement.First().GetSafeText();

        interviewcounter.ProductMetrics = productMetrics;
        interviewcounter.Website = website;
        interviewcounter.Article = article;
        log.Info(JsonConvert.SerializeObject(interviewcounter, Formatting.Indented));
      }
    }
  }

  public class Interview
  {
    public string Text { get; set; }
    public string ProductName { get; set; }
    public string TagLine { get; set; }
    public string InterviewLink { get; set; }
    
    public string ProductMetrics { get; set; }
    public string Website { get; set; }
    public string Article { get; set; }

  }
}
