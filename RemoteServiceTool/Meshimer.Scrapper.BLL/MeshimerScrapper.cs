using Meshimer.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Linq;

namespace Meshimer.Scrapper.BLL
{
    /// <summary>
    /// Author: Narek Yegoryan
    /// </summary>
    public class MeshimerScrapper : IDisposable
    {
        #region Fields

        private IWebDriver _webDriver;
        private FirefoxOptions _fireFoxOptions;
        private ChromeOptions _chromeOptions;
        private InternetExplorerOptions _explorerOptions;
        private readonly BrowserTypeEnum _browserType;

        #endregion

        #region Constructors

        public MeshimerScrapper(BrowserTypeEnum browserType = BrowserTypeEnum.Chrome)
        {
            _browserType = browserType;
            InitializeWebDriver(_browserType);
        }

        #endregion

        #region Methods

        public string GetUserNameFromMeshimerPageAndHandle(Action executeOnMismatch = null)
        {
            var parsedUsername = string.Empty;
            if (_webDriver != null)
            {
                _webDriver.Navigate().GoToUrl("https://www.youtube.com/");
                IWebElement blockElement = null;

                if (_browserType != BrowserTypeEnum.IE) // IE issue
                    blockElement = _webDriver.FindElement(By.Id(Constants.wb_BlockDetails));

                if (blockElement != null)
                {
                    var elementText = blockElement.Text;
                    var index = elementText.IndexOf('\r');
                    parsedUsername = elementText.Substring(5, index - 5).Trim();
                }
            }

            string currentMachineUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            if (currentMachineUserName.Split('\\').LastOrDefault() != parsedUsername)
                executeOnMismatch?.Invoke();

            return parsedUsername;
        }

        void InitializeWebDriver(BrowserTypeEnum browserType)
        {
            switch (browserType)
            {
                case BrowserTypeEnum.Chrome:
                    InitializeChromeDriverOptions();
                    _webDriver = new ChromeDriver(_chromeOptions);
                    break;
                case BrowserTypeEnum.Firefox:
                    InitializeFirefoxDriverOptions();
                    _webDriver = new FirefoxDriver(_fireFoxOptions);
                    break;
                default:
                    InitializeIEDriverOptions();
                    _webDriver = new InternetExplorerDriver(_explorerOptions);
                    break;
            }
        }

        void InitializeChromeDriverOptions()
        {
            _chromeOptions = new ChromeOptions();
            //_chromeOptions.PageLoadStrategy = PageLoadStrategy.Normal;
            //_chromeOptions.AcceptInsecureCertificates = true;
            _chromeOptions.AddArgument("disable-infobars");
            //_chromeOptions.AddArgument("-no-sandbox");
            //_chromeOptions.AddArguments("--headless");
            // _chromeOptions.AddArguments("enable-nacl");
            //_chromeOptions.AddArgument("--start-maximized");
            //_chromeOptions.AddArgument("--ignore-certificate-errors");
            //_chromeOptions.AddArgument("--disable-popup-blocking");
        }

        void InitializeFirefoxDriverOptions()
        {
            _fireFoxOptions = new FirefoxOptions();
            _fireFoxOptions.AcceptInsecureCertificates = true;
        }

        void InitializeIEDriverOptions()
        {
            _explorerOptions = new InternetExplorerOptions { EnableNativeEvents = false };
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// IDisposable support
        /// </summary>
        public void Dispose()
        {
            _webDriver?.Quit();
            _webDriver?.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
