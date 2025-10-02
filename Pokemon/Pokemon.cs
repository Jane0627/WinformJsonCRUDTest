using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsTestv5
{
    public class Rootobject
    {
        public int count { get; set; }
        public string next { get; set; }
        public object previous { get; set; }
        public Result[] results { get; set; }
    }

    public class Result
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

}
