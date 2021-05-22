using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace UWPattendance.Tasks
{
  class Text_to_Speech
  {

    public async void ReadText(string mytext)
    {
      MediaElement mediaplayer = new MediaElement();
      using (var speech = new SpeechSynthesizer())
      {
        speech.Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Female);
        string ssml = @"<speak version='1.0' " + "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-UK'>" + mytext + "</speak>";
        SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
        mediaplayer.SetSource(stream, stream.ContentType);
        mediaplayer.Play();
      }
    }
  }

 
}
