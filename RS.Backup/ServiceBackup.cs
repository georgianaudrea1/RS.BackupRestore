using Hangfire;
using Hangfire.Common;
using Hangfire.SqlServer;
using Hangfire.Storage;
using RS.Common;
using RS.Common.Extensions;
using RS.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RS.Backup
{
    public partial class ServiceBackup : ServiceBase
    {
        BackgroundJobServer server;
        Engine e = Engine.Instance;


        public ServiceBackup()
        {
            InitializeComponent();
            GlobalConfiguration.Configuration.UseSqlServerStorage("Data source=RSOFT021;initial catalog=Test;persist security info=True;user id=testBK;password='Parola!1';MultipleActiveResultSets=True;");


        }

        public void StartBackUp()
        {


          

             e.Start();
            
        }


        protected override void OnStart(string[] args)
        {
          
            StartBackUp();


        }
        protected override void OnStop()
        {

        }

    }
}
