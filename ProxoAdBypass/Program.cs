using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace ProxoAdBypass {
    class Program {
        static async Task AsyncMain() {
            Console.WriteLine("Downloading chromium...");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Console.WriteLine("Initializing browser...");
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions {
                Headless = false,
                DefaultViewport = null,
            });
            var page = await browser.NewPageAsync();
            Console.WriteLine("Setting up request interception...");
            await page.SetRequestInterceptionAsync(true);
            page.Request += (sender, args) => {
                if (!args.Request.Url.Contains("proxo")) {
                    args.Request.ContinueAsync();
                    return;
                }

                var payload = new Payload {
                    Headers = args.Request.Headers
                };
                if (!payload.Headers.TryAdd("Referer", "https://proxo.wtf/ProxoKeyKeyLol/index.php")) {
                    payload.Headers["Referer"] = "https://proxo.wtf/ProxoKeyKeyLol/index.php";
                }

                args.Request.ContinueAsync(payload);
            };
            Console.WriteLine("Opening proxo key site...");
            await page.GoToAsync("https://proxo.wtf/ProxoKeyKeyLol/index.php");
            Console.WriteLine("Done! Press any key to exit.");
            Console.ReadLine();
            Console.WriteLine("Closing browser...");
            await browser.CloseAsync();
        }

        static void Main(string[] args) {
            AsyncMain().Wait();
        }
    }
}
