using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreManagement
{
    class Config
    {
        public string dbStr;
        public Config()
        {
            this.dbStr = "server=127.0.0.1;port=3306;user=root;password=chenqijie;charset=utf8";
        }
    }
}
