using System.ServiceProcess;
using System.Threading;

namespace FileManager
{
    internal partial class FileWatcher : ServiceBase
    {
        private Logger logger;

        public FileWatcher()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger = new Logger();
            var loggerThread = new Thread(logger.Start);
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }
}