using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SeleniumExtras.WaitHelpers;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Selenium_UnitTests
{
  [TestClass]
  public class SproutsUnitTest
  {
    ChromeDriver driver;
    WebDriverWait webDriverWait;
    [TestInitialize]
    public void Startup()
    {
      var chromeOptions = new ChromeOptions();
      //chromeOptions.AddArguments("headless");
      //chromeOptions.AddArguments("window-sized1200,600");
      chromeOptions.AddArguments("--proxy-server='direct://'");
      chromeOptions.AddArguments("--proxy-bypass-list=*");
      chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
      
      //chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
      //chromeOptions.AddArguments("--disable-extensions");
      chromeOptions.AddArguments("--start-maximized");

      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(path);
      //chromeDriverService.SuppressInitialDiagnosticInformation = true;
      //chromeDriverService.HideCommandPromptWindow = true;

      driver = new ChromeDriver(chromeDriverService, chromeOptions);
      //driver.Manage().Window.Maximize();
      webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }
    [TestCleanup]
    public void CleanUp()
    {
      driver.Quit();
    }
    [TestMethod]
    public void GetSproutsWeeklyAdDetails()
    {
      var items = new List<Item>();

      try
      {

        try
        {
          driver.Navigate().GoToUrl("http://www.sprouts.com/store/tx/plano/plano/");
        }
        catch (TimeoutException timeoutException)
        {
          driver.Navigate().Refresh();
        }
        var iJavaScriptExecutor = (IJavaScriptExecutor)driver;
        webDriverWait.Until(driver1 => iJavaScriptExecutor.ExecuteScript("return document.readyState").Equals("complete"));

        var elementLinkToFlyer = webDriverWait.Until(ExpectedConditions.ElementToBeClickable(
          By.XPath("//div[@class='cell small-6 divider']/button")));
        Thread.Sleep(2000);
        elementLinkToFlyer.Click();
        webDriverWait.Until(driver1 => iJavaScriptExecutor.ExecuteScript("return document.readyState").Equals("complete"));

        //var elementsLeftHandSideMenu = webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
        //  By.XPath("//ul[@class='menu sidenav']/li[@class='child']/a")));
        //System.Diagnostics.Debug.WriteLine(elementsLeftHandSideMenu.Count);
        foreach (var elementLeftHandSideMenu in webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
          By.XPath("//ul[@class='menu sidenav']/li[@class='child']/a"))))
        {
          var parent = elementLeftHandSideMenu.Text;
          Thread.Sleep(2000);
          try
          {
            elementLeftHandSideMenu.Click();
            driver.Navigate().Refresh();
            //iJavaScriptExecutor.ExecuteScript("arguments[0].click();", elementLeftHandSideMenu);
          }
          catch (Exception exception)
          {
            System.Diagnostics.Debug.WriteLine(exception.ToString());
          }
          //var elementsRightHandSideMenu = webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
          //By.XPath("//div[@class='cell-content-wrapper']")));

          var children = new List<string>();
          foreach (var elementRightHandSideMenu in webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
          By.XPath("//div[@class='cell-content-wrapper']"))))
          {
            var element3 = elementRightHandSideMenu.FindElement(By.XPath(".//span[@class='cell-title-text']"));
            var element4 = elementRightHandSideMenu.FindElement(By.XPath(".//div[@class='price sale on-sale']"));
            children.Add(element3.Text + " " + element4.Text);
          }
          items.Add(new Item() { Parent = parent, Children = children });
        }
      }
      catch(Exception exception)
      {
        System.Diagnostics.Debug.WriteLine(exception.ToString());
      }
      System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(items, Formatting.Indented));
      System.Diagnostics.Debug.WriteLine("Exit");
    }

    [TestMethod]
    public void GetSproutsWeeklyAdDetails2()
    {
      try
      {
        driver.Navigate().GoToUrl("https://shop.sprouts.com/shop/flyer");
      }
      catch (TimeoutException timeoutException)
      {
        driver.Navigate().Refresh();
      }
      var iJavaScriptExecutor = (IJavaScriptExecutor)driver;
      webDriverWait.Until(driver1 => iJavaScriptExecutor.ExecuteScript("return document.readyState").Equals("complete"));

      var elementsLeftHandSideMenu = webDriverWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(
        By.XPath("//ul[@class='menu sidenav']/li[@class='child']/a")));
      System.Diagnostics.Debug.WriteLine(elementsLeftHandSideMenu.Count);
      var items = new List<Item>();
      foreach (var elementLeftHandSideMenu in elementsLeftHandSideMenu)
      {
        Thread.Sleep(2000);
        try
        {
          elementLeftHandSideMenu.Click();
          driver.Navigate().Refresh();
          //iJavaScriptExecutor.ExecuteScript("arguments[0].click();", elementLeftHandSideMenu);
        }
        catch (Exception exception)
        {
          System.Diagnostics.Debug.WriteLine(exception.ToString());
        }
        System.Diagnostics.Debug.WriteLine("Exit");
      }
    }
  }
  public class Item
  {
    public string Parent { get; set; }
    public List<string> Children { get; set; }
  }
}
