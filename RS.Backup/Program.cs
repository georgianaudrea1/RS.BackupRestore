using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RS.Backup
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

        
            if (Environment.UserInteractive)
            {

                var serv = new ServiceBackup();

                serv.StartBackUp();

            }
            else
                ServiceBase.Run(new ServiceBackup());

        }
        
    }
}
