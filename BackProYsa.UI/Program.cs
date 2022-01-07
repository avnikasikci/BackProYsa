using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackProYsa.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*Aşağıdaki Kodlar Windows Service için yazıldı**/
            //Check for the Debugger is attached or not if attached then run the application in IIS or IISExpress
            var isService = false;

            //when the service start we need to pass the --service parameter while running the .exe
            //if (Debugger.IsAttached == false && args.Contains("--service"))
            //{
            //    isService = true;
            //}
            if (args.Contains("--service"))
            {
                isService = true;
            }

            if (isService)
            {
                //Get the Content Root Directory
                var pathToContentRoot = Directory.GetCurrentDirectory();

                //If the args has the console element then than make it in array.
                var webHostArgs = args.Where(arg => arg != "--console").ToArray();

                string ConfigurationFile = "appsettings.json"; //Configuration file.
                string portNo = "5001"; //Port default port atanır. appsetting.jsonda varsa oradaki kullanılır.

                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                pathToContentRoot = Path.GetDirectoryName(pathToExe);

                //Get the json file and read the service port no if available in the json file.
                string AppJsonFilePath = Path.Combine(pathToContentRoot, ConfigurationFile);

                if (File.Exists(AppJsonFilePath))
                {
                    using (StreamReader sr = new StreamReader(AppJsonFilePath))
                    {
                        string jsonData = sr.ReadToEnd();
                        JObject jObject = JObject.Parse(jsonData);
                        if (jObject["ServicePort"] != null)
                            portNo = jObject["ServicePort"].ToString();

                    }
                }

                //var host = WebHost.CreateDefaultBuilder(webHostArgs)
                //.UseContentRoot(pathToContentRoot)
                //.UseStartup<Startup>()
                //.UseUrls("http://localhost:" + portNo)
                //.Build();

                //host.RunAsService();


                CreateWebHostBuilder(args).UseContentRoot(pathToContentRoot).UseUrls("http://localhost:" + portNo).Build().RunAsService();//Windows server olmayan projeye için kapatıldı

            }
            else
            {
                //CreateHostBuilder(args).Build().Run();
                CreateWebHostBuilder(args).Build().Run();
            }
            /*Yukarıdaki Kodlar Windows Service için yazıldı örn proje:windows server olamyan serverlarda proje otomatik baslatilsin**/

            //CreateHostBuilder(args).Build().Run();
            //CreateWebHostBuilder(args).Build().Run();//Windows server olmayan projeye için kapatıldı
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                 WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

        //    public static IHostBuilder CreateHostBuilder(string[] args) =>
        //            // The `UseServiceProviderFactory(new AutofacServiceProviderFactory())` call here allows for
        //            // ConfigureContainer to be supported in Startup with
        //            // a strongly-typed ContainerBuilder. If you don't
        //            // have the call to AddAutofac here, you won't get
        //            // ConfigureContainer support. This also automatically
        //            // calls Populate to put services you register during
        //            // ConfigureServices into Autofac.
        //            Host.CreateDefaultBuilder(args)
        //                //.UseServiceProviderFactory(new AutofacServiceProviderFactory())
        //                .ConfigureWebHostDefaults(webBuilder =>
        //                {
        //                    webBuilder.UseStartup<Startup>();
        //                });
    }
}
