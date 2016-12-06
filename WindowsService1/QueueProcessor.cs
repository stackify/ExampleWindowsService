using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackifyLib;

namespace WindowsService1
{
    class QueueProcessor
    {
        private QueueClient _QueueClient = null;

        public void Start()
        {
            OnMessageOptions options = new OnMessageOptions
            {
                MaxConcurrentCalls = 5,
                AutoComplete = false
            };

            try
            {

                _QueueClient = QueueClient.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"], "WindowsService1");

                _QueueClient.OnMessageAsync(async m =>
                {
                    bool shouldAbandon = false;
                    try
                    {
                    //Create a new ID for the operation for correlation
                    var tracer = ProfileTracer.CreateAsOperation("Queue.WindowsService1", Trace.CorrelationManager.ActivityId.ToString());

                    // asynchronouse processing of messages

                    await tracer.ExecAsync(() =>
                        {
                            return ProcessMessageAsync(m);
                        });


                    // complete if successful processing
                    await m.CompleteAsync();
                    }
                    catch (Exception ex)
                    {
                        shouldAbandon = true;
                        Console.WriteLine(ex);
                    }

                    if (shouldAbandon)
                    {
                        await m.AbandonAsync();
                    }
                },
                options);

            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }

        }

        private async Task<bool> ProcessMessageAsync(BrokeredMessage message)
        {
            Debug.WriteLine("Recieved message " + message.MessageId);

            Debug.WriteLine("Do some cool back end processing here");

            return true;
        }

        public void Stop()
        {
            _QueueClient.Close();
        }
    }
}
