using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Service1()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {

                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "-install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "-uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    default:
                        Debug.Listeners.Add(new TextWriterTraceListener(Console.Out, "Console"));
                        Service1 service = new Service1();
                        service.Start();

                        Console.WriteLine("Started");
                        Console.WriteLine("Press enter to exit");
                        Console.ReadLine();
                        service.Stop();
                        break;

                }

          

            }
        }
    }
}
