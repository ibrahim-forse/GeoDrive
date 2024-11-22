using System.Configuration;
using GeoDrive.Core.Services;

namespace GeoDrive.DroneConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var googleService = new GoogleDriveService();
            var localFileService = new LocalFileServices();

            string localStorageDirectory = ConfigurationManager.AppSettings.Get("LocalStorageDirectory");

            var sampleDataService = new SampleDataService();
            sampleDataService.GenerateSampleData(localStorageDirectory);

            string credentialsPath = ConfigurationManager.AppSettings.Get("GoogleCredentailsPath");
            string folderId = ConfigurationManager.AppSettings.Get("DriveFolderId");
            string[] filesToUpload = localFileService.ReadFileNames(localStorageDirectory);

            bool.TryParse(ConfigurationManager.AppSettings.Get("SimulateError"), out bool isSimulateError);

            googleService.UploadFilesToGoogleDriveConsole(credentialsPath, folderId, filesToUpload, isSimulateError);
        }
    }
}


