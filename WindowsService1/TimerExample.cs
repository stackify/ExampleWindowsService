using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.ServiceBus.Messaging;

namespace WindowsService1
{
    class TimerExample
    {
        private Timer _Timer = null;

        public void Start()
        {
            _Timer = new Timer(10000); //30 seconds
            _Timer.Elapsed += async (sender, e) => await HandleTimer();
            _Timer.AutoReset = true;
            _Timer.Enabled = true;
            Console.WriteLine("TimerExample Timer started");
        }


        private static async Task<bool> HandleTimer()
        {
            var op = StackifyLib.ProfileTracer.CreateAsOperation("Timer.DownloadPage");

            var result = await op.ExecAsync(async () =>
            {
                try
                {
                    Debug.WriteLine("TimerExample: Downloading page");

                    using (HttpClient client = new HttpClient())
                    {
                        var data = await client.GetAsync("https://www.microsoft.com/en-us/");
                        var content = await data.Content.ReadAsStringAsync();
                        Console.WriteLine("Finished downloading Microsoft page. Length:" + content.Length);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                return false;
            });


            var op2 = StackifyLib.ProfileTracer.CreateAsOperation("Timer.WriteToQueue");

            var result2= await op2.ExecAsync(async () =>
            {
                try
                {
                    Debug.WriteLine("TimerExample: Writing to queue");

                    var client = QueueClient.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"], "WindowsService1");
                    var message = new BrokeredMessage("This is a test message!");
                    client.Send(message);

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                return false;
            });


            return result;
        }

        public void Stop()
        {
            try
            {
                _Timer.Stop();
                _Timer.Dispose();
            }
            catch (Exception)
            {
                
                
            }
        }
    }
}
