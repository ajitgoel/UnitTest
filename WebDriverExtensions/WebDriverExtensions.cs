using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WebDriverExtensions
{
  public static class ChromeDriverExtensions
  {
    public static void GoToUrlAndWaitTillPageLoaded(this ChromeDriver chromeDriver, WebDriverWait webDriverWait = null, string url = null)
    {
      try
      {
        chromeDriver.Navigate().GoToUrl(url);
        chromeDriver.WaitTillPageLoads(webDriverWait);
      }
      catch (TimeoutException timeoutException)
      {
        chromeDriver.Navigate().Refresh();
      }
    }

    public static void WaitTillPageLoads(this ChromeDriver chromeDriver, WebDriverWait webDriverWait = null)
    {
      if (webDriverWait == null)
      {
        webDriverWait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
      }
      var iJavaScriptExecutor = (IJavaScriptExecutor)chromeDriver;
      webDriverWait.Until(driver1 => iJavaScriptExecutor.ExecuteScript("return document.readyState").Equals("complete"));
    }
    public static void OpenAndSwitchToNewTab(this ChromeDriver chromeDriver)
    {
      ((IJavaScriptExecutor)chromeDriver).ExecuteScript("window.open();");
      chromeDriver.SwitchTo().Window(chromeDriver.WindowHandles.Last());
    }
  }

  public static class IWebElementExtensions
  {
    public static IWebElement FindElementIgnoreStaleElementReferenceException2(this IWebElement iWebElement, WebDriverWait webDriverWait = null,
      By by = null)
    {
      bool staleElement = true;
      IWebElement iWebElementReturn = null;
      while (staleElement)
      {
        try
        {
          iWebElementReturn = webDriverWait.Until(driver1 => iWebElement.FindElement(by));
          staleElement = false;
        }
        catch (StaleElementReferenceException staleElementReferenceException)
        {
          staleElement = true;
        }
      }
      return iWebElementReturn;
    }

    public static string GetSafeText(this IWebElement iWebElement)
    {
      string result = string.Empty;
      try
      {
        result = iWebElement.Text;
      }
      catch (StaleElementReferenceException staleElementReferenceException)
      {
        result = string.Empty;
      }
      return result;
    }
    public static string GetSafeAttribute(this IWebElement iWebElement, string attributeName)
    {
      string result = string.Empty;
      try
      {
        result = iWebElement.GetAttribute(attributeName);
      }
      catch (StaleElementReferenceException staleElementReferenceException)
      {
        result = string.Empty;
      }
      return result;
    }
  }
}
