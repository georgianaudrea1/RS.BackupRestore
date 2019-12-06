using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Backup
{
      public  class Database
    {
       public string ConnectionString { get; set; }
       public string Cronos { get; set; }
        public Database(string ConnectionString,string Cronos)
        {
            this.ConnectionString = ConnectionString;
           this.Cronos = Cronos;
        }
        
        public Database() { }
    }
}
