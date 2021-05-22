using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance.Models
{
    class Person
    {

        [PrimaryKey]
        public int Id { get; set; }



        public string FirstName { get; set; }


        public string LastName { get; set; }


        public string ImagePath { get; set; }

        public DateTime DateRegistered { get; set; }

    public string ImageName { get; set; }


  }
}
