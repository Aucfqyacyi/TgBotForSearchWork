using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V110.Page;

namespace SimpleCloudflareBypass.Utilities;

public static class ChromeDriverCreator
{
    public static IWebDriver Create()
    {
        ChromeOptions options = new();
        options.AddArgument("start-maximized");
        options.AddArgument("disable-infobars");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddArgument("--no-sandbox");
        //options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-software-rasterizer");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("--disable-dev-shm-usage");
        ChromeDriver chromeDriver = new ChromeDriver(options);
        DevToolsSession session = chromeDriver.GetDevToolsSession();
        var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V110.DevToolsSessionDomains>();
        domains.Page.Enable(new EnableCommandSettings());
        domains.Page.AddScriptToEvaluateOnNewDocument(new AddScriptToEvaluateOnNewDocumentCommandSettings()
        {
            Source = "delete window.cdc_adoQpoasnfa76pfcZLmcfl_Array;" +
                     "delete window.cdc_adoQpoasnfa76pfcZLmcfl_Promise;" +
                     "delete window.cdc_adoQpoasnfa76pfcZLmcfl_Symbol;"
        });
        return chromeDriver;
    }
}
