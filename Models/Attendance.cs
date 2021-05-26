using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance.Models
{
    public class Attendance
    {


        [PrimaryKey]
        public int Id { get; set; }

        public int User_ID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }



        [Display(Name ="Last Name")]
        public string LastName { get; set; }


        public string NewPicturePath { get; set; }
    public string ImageName { get; set; }


    public DateTime Date_Signed_In_Date_and_Time { get; set; }

    }


    public class AttendanceList
    {

    public DateTime yesterday =   new DateTime(2021,05,14);

    DateTime tomorrow = DateTime.UtcNow.AddDays(1);


        public static List<Attendance> GetAttendance()
            {

            var attendance = new List<Attendance>();

            attendance.Add(new Attendance { Id = 1, FirstName = "Ains Soph", LastName = "Rikudou", Date_Signed_In_Date_and_Time = DateTime.UtcNow });
            attendance.Add(new Attendance { Id = 1, FirstName = "Klaus", LastName = "Nick", Date_Signed_In_Date_and_Time = DateTime.UtcNow });
            attendance.Add(new Attendance { Id = 1, FirstName = "Iweh", LastName = "John", Date_Signed_In_Date_and_Time = DateTime.UtcNow });


      attendance.Add(new Attendance { Id = 1, FirstName = "Ains Soph", LastName = "Rikudou", Date_Signed_In_Date_and_Time = new DateTime(2021, 05, 14) });
   

      attendance.Add(new Attendance { Id = 1, FirstName = "Klaus", LastName = "Nick", Date_Signed_In_Date_and_Time = DateTime.UtcNow.AddDays(1) });
      attendance.Add(new Attendance { Id = 1, FirstName = "Iweh", LastName = "John", Date_Signed_In_Date_and_Time = DateTime.UtcNow.AddDays(1) });



      return attendance;

        }
    }

}
