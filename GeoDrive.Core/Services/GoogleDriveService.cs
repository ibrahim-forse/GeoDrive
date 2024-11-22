using GeoDrive.Core.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GeoDrive.Core.Services
{
    public class GoogleDriveService
    {
        private DriveService CreateGoogleService(string credentialsFileJson)
        {
            GoogleCredential credential;

            credential = GoogleCredential.FromJson(credentialsFileJson).CreateScoped(new[] {
                    DriveService.ScopeConstants.DriveFile,
                });

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Geo Drive Android"
            });
        }

        public void UploadFilesToGoogleDriveConsole(string credentialsPath, string parentFolderId, string[] filesToUpload,
            bool simulateError = false)
        {
            GoogleCredential credential;

            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[] {
                    DriveService.ScopeConstants.DriveFile
                });

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Geo Drive Drone"
                });

                int errors = 0;
                int successes = 0;
                foreach (var file in filesToUpload)
                {
                    var data = JsonConvert.DeserializeObject<SampleCoordinateData>(File.ReadAllText(file));
                    if (data == null)
                    {
                        throw new Exception($"Error occurred reading file {file}");
                    }

                    var folderMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = $"{data.Latitude}-{data.Longitude}",
                        MimeType = "application/vnd.google-apps.folder",
                        Parents = new List<string> { parentFolderId }
                    };

                    var folderRequest = service.Files.Create(folderMetadata);
                    folderRequest.Fields = "id";
                    var folder = folderRequest.Execute();
                    Console.WriteLine("Folder ID: " + folder.Id);

                    var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = Path.GetFileName(file),
                        Parents = new List<string> { folder.Id }
                    };

                    FilesResource.CreateMediaUpload request;

                    try
                    {
                        if (simulateError && errors == 0)
                        {
                            throw new Exception();
                        }

                        using (var fileStream = new FileStream(file, FileMode.Open))
                        {

                            request = service.Files.Create(fileMetaData, fileStream, "");
                            request.Fields = "id";
                            request.Upload();
                        }

                        var uploadedFile = request.ResponseBody;
                        Console.WriteLine($"File '{fileMetaData.Name}' uploaded with ID: {uploadedFile.Id}");
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                        successes++;
                    }
                    catch
                    {
                        errors++;
                        Console.WriteLine($"An error occurred while uploading {Path.GetFileName(file)}. File will be stored and uploaded next cycle");
                    }
                }

                Console.WriteLine($"Process Done. {successes} files uploaded. {errors} files failed.");
            }
        }

        public string UploadFilesToGoogleDriveAndroid(string credentialsJson, Stream fileStream,
            string fileName, string folderId, bool simulateError = false)
        {
            var service = CreateGoogleService(credentialsJson);

            var fileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            FilesResource.CreateMediaUpload request;

            try
            {
                if (simulateError)
                {
                    throw new Exception();
                }

                request = service.Files.Create(fileMetaData, fileStream, "");
                request.Fields = "id";
                request.Upload();

                var uploadedFile = request.ResponseBody;
                if (uploadedFile != null)
                {
                    return uploadedFile.Id;
                }

                return "None";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetOrCreateFolder(string credentialsFile, string folderName, string parentFolderId)
        {
            var service = CreateGoogleService(credentialsFile);

            var newFile = new Google.Apis.Drive.v3.Data.File
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { parentFolderId }
            };

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Q = $"'{parentFolderId}' in parents";
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name, parents)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Name.Equals(newFile.Name, StringComparison.OrdinalIgnoreCase)
                        && file.Parents != null
                        && file.Parents.Any() && file.Parents.Contains(parentFolderId))
                    {
                        return file.Id;
                    }
                }
            }

            var result = service.Files.Create(newFile).Execute();

            return result.Id;
        }
    }
}
