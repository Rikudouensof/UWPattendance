using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance.Tasks
{
    public class Database_Connection
    {

        public static string _dbpath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "AnAttendanceS.db3");


        
    }
}
