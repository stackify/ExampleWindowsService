using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        private TimerExample _TimerExample = null;
        private QuartzExample _QuartzExample = null;
        private QueueProcessor _QueueProcessor = null;

        public Service1()
        {
            InitializeComponent();
        }

        public void Start()
        {
            _QueueProcessor = new QueueProcessor();
            _QueueProcessor.Start();

            _TimerExample = new TimerExample();
            _TimerExample.Start();

            _QuartzExample = new QuartzExample();
            _QuartzExample.Start();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            _TimerExample.Stop();
            _QuartzExample.Stop();
            _QueueProcessor.Stop();
        }
    }
}
