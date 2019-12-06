using RS.Common;
using RS.Common.Extensions;
using RS.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RS.Backup
{
    class BackupM
    {
        public static ConcurrentDictionary<String, String> dict = new ConcurrentDictionary<string, string>();
        public BackupM()
        {

        }

        public static string BackupDB(ConnectionData connectionData, string database)
        {
            BackupManager backupManager = new BackupManager(connectionData);
            string backupFileName = database + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string path = backupManager.Backup(database, backupFileName);


            if (File.Exists(backupFileName))
            {
           
                dict.TryAdd(database, path);

            }

            return path;
        }

    }
}
