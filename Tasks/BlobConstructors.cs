using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance.Tasks
{
  public class BlobConstructors
  {

    public static string Blobconnectionstring = "DefaultEndpointsProtocol=https;AccountName=eeeproject;AccountKey=8v6040AWFPSOVrpRWlK0re0UgZRLslrTT+OZUSCJOOB8PLnMIKhRnJHvYoME/6UXupcONrVLza/ksH8crX7R2A==;EndpointSuffix=core.windows.net";
    public static string containername = "facerecblob";

    public static string blobBaseUrl = "https://eeeproject.blob.core.windows.net/facerecblob/";


    public static IConfigurationRoot GetConfiguration() => new ConfigurationBuilder()
     .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
     .AddJsonFile("appsettings.json")
     .Build();


    public static IEnumerable<FileInfo> GetFiles(string sourceFolder)
      => new DirectoryInfo(sourceFolder)
      .GetFiles()
      .Where(f => !f.Attributes.HasFlag(System.IO.FileAttributes.Hidden));

    


    public static void UploadFile(
      FileInfo file, string connectionString, string container)
    {
      var containerClient = new BlobContainerClient(connectionString, container);

      try
      {
        var blobClient = containerClient.GetBlobClient(file.Name);
        using (var fileStream = File.OpenRead(file.FullName))
        {
          blobClient.Upload(fileStream);
        }
      }
      catch (Exception)
      {

        throw;
      }


    }
  }


}
