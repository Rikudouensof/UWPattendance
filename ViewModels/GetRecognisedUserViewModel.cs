using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance.ViewModels
{
  public class GetRecognisedUserViewModel
  {

    public Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person highestperson { get; set; }

    public string Answers { get; set; }
    public double ConfidenceLevel { get; set; }
  }


  public class outputUser
  {
    public int MyProperty { get; set; }
  }
}
