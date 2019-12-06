using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using Microsoft.JScript;
using RS.Data;
using RS.Logger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace RS.Backup
{
    class Engine
    {
        private static Engine instance = null;
        string pathXML = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        XmlDocument xmlDoc = new XmlDocument();
        public List<string> pathList = new List<string>();
      
        private Engine()
        {

        }

        public static Engine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Engine();
                }
                return instance;
            }
        }

        private bool running = false;
        public List<Database> ReadFromXML()
        {

            List<Database> db = new List<Database>();
            xmlDoc.Load(pathXML + @"\XMLDatabase.xml");

            foreach (XmlNode node in xmlDoc.DocumentElement.FirstChild)
            {
                Database d = new Database();
                d.ConnectionString = node.Attributes["ConnectionString"].InnerText;
                d.Cronos = node.Attributes["Cronos"].InnerText;
                db.Add(d);
            }

            return db;
        }
        public FTP.FtpConnectionInfo ReadFTPFromXML()
        {

            xmlDoc.Load(pathXML + @"\XMLDatabase.xml");

            XmlNodeList xnList = xmlDoc.SelectNodes("/Schedule/Destination/FTP");
            FTP.FtpConnectionInfo ftpCon = new FTP.FtpConnectionInfo();


            foreach (XmlNode xn in xnList)
            {
                ftpCon.Host = xn["Host"].InnerText;
                ftpCon.Username = xn["User"].InnerText;
                ftpCon.Password = xn["Password"].InnerText;

            }

            return ftpCon;

        }

        public void Start()
        {
        
            running = true;
            while (running)
            {

                List<Database> list = ReadFromXML();


                foreach (Database d in list)
                {
                    ConnectionData connectionData = new ConnectionData(d.ConnectionString);

                    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(d.ConnectionString);
                    string database = builder.InitialCatalog;
                    BackupManager backupManager = new BackupManager(connectionData);
                    string lastRunResult = string.Empty;


                    using (var connection = JobStorage.Current.GetConnection())
                    {
                       
                         var recurringJobs = connection.GetRecurringJobs();
                        if (recurringJobs.Count == 0)
                        {
                            RecurringJob.AddOrUpdate(database, () => BackupM.BackupDB(connectionData, database), d.Cronos);
                        }
                        else
                        {
                            var job = recurringJobs.FirstOrDefault(p => p.Id.Equals(database));

                            if (job != null)
                            {

                                try
                                {
                                    if (job.LastJobId != null)
                                    { 

                                        var jobState = connection.GetStateData(job.LastJobId);
                                        lastRunResult = jobState.Name;


                                        if ((!job.Cron.Equals(d.Cronos)) || (lastRunResult.Equals("Failed")) || (lastRunResult.Equals("Expired")))
                                        {

                                            RecurringJob.AddOrUpdate(database, () => BackupM.BackupDB(connectionData, database), d.Cronos);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error("Eroor job",e.Message);
                                 

                                }
                            }
                            else
                            {
                                RecurringJob.AddOrUpdate(database, () => BackupM.BackupDB(connectionData, database), d.Cronos);
                            }
                        }
                    }
                }
                FTP.FtpConnectionInfo ftpCon = ReadFTPFromXML();
                FTP.FTPClient fTPClient = new FTP.FTPClient(ftpCon.Host, ftpCon.Username, ftpCon.Password);

                foreach (var p in BackupM.dict)
                    fTPClient.UploadFile(p.Value);

         
            }
        }

    }
}
