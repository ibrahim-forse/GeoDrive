using GeoDrive.Core.Services;
using System.Collections;
using System.IO;
using System.Text.Json.Nodes;

namespace GeoDrive.MobileApp
{
    public partial class MainPage : ContentPage
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public MainPage()
        {
            InitializeComponent();
            SetLocationAsync();
        }

        public async void SetLocationAsync()
        {
            Tuple<double, double> gpsCoordinates;
            try
            {
                gpsCoordinates = await GetCurrentLocation();
            }
            catch
            {
                gpsCoordinates = await GetCachedLocation();
            }

            locationName.Text = await GetGeocodeReverseData(gpsCoordinates.Item1, gpsCoordinates.Item2);
            UploadFileButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
            //FileNameLabel.Text = gpsCoordinates.Item1 + " "+gpsCoordinates.Item2;
        }

        private async void OnResetClicked(object sender, EventArgs e)
        {
            UploadFileButton.IsEnabled = false;
            ResetButton.IsEnabled = false;
            SetLocationAsync();
        }

        public void OnLocationNameCompleted(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(locationName.Text))
            {
                UploadFileButton.IsEnabled = true;
            }
        }

        private async void OnUploadClicked(object sender, EventArgs e)
        {
            FileNameLabel.TextColor = Color.FromRgb(0, 0, 0);
            UploadFileButton.IsEnabled = false;
            ResetButton.IsEnabled = false;
            FileNameLabel.Text = "Loading ...";
            try
            {
                FileStream fileStream;
                var results = await FilePicker.Default.PickMultipleAsync(new PickOptions
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.Android, new[] { "*/*" } }
                    })
                });

                if (results == null || !results.Any())
                {
                    FileNameLabel.Text = $"Could not read file";
                    //re-enable the button
                    return;
                }

                var uploadTasks = new List<Task>();
                foreach (var result in results)
                {
                    fileStream = (FileStream)await result.OpenReadAsync();
                    _ = UploadeFile(fileStream, Path.GetFileName(fileStream.Name));
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur (e.g., permission issues)
                FileNameLabel.TextColor = Color.FromRgb(130, 0, 0);
                FileNameLabel.Text = $"Error: {ex.Message} .. {ex.StackTrace}";
            }
            finally
            {
                UploadFileButton.IsEnabled = true;
                ResetButton.IsEnabled = true;
            }
        }

        private async Task UploadeFile(Stream fileStream, string fileName)
        {
            try
            {
                FileNameLabel.Text = "Uploading ...";
                GoogleDriveService service = new GoogleDriveService();
                using (var credentialsFile = await FileSystem.OpenAppPackageFileAsync("credentials.json"))
                {
                    using (StreamReader reader = new StreamReader(credentialsFile))
                    {
                        var credentialsFileJson = reader.ReadToEnd();
                        credentialsFile.Close();

                        var folderID = service.GetOrCreateFolder(credentialsFileJson, locationName.Text, "1ONABwwwmZ4tg-j79OsvmzYHIkyyhDR54");
                        service.UploadFilesToGoogleDriveAndroid(credentialsFileJson, fileStream, fileName.ToString(), folderID);

                        FileNameLabel.TextColor = Color.FromRgb(0, 73, 0);
                        FileNameLabel.Text = "Uploaded";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur (e.g., permission issues)
                FileNameLabel.TextColor = Color.FromRgb(130, 0, 0);
                FileNameLabel.Text = $"Error: {ex.Message} .. {ex.StackTrace}";
            }
        }

        private async Task<Tuple<double, double>> GetCachedLocation()
        {
            try
            {
                Location location = await Geolocation.Default.GetLastKnownLocationAsync();

                if (location != null)
                    return Tuple.Create(location.Latitude, location.Longitude);
                else
                    return Tuple.Create(1.1, 1.1);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                return Tuple.Create(0.0, 0.0);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                throw new Exception("Please enable GPS or enter location name");
            }
            catch (PermissionException pEx)
            {
                throw new Exception("Please give GPS permission or enter location name");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string> GetGeocodeReverseData(double latitude, double longitude)
        {
            IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

            Placemark? placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                return $"{placemark.CountryName} - {placemark.AdminArea} - {placemark.Locality}";
            }

            return "Unknown";
        }

        public async Task<Tuple<double, double>> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                _isCheckingLocation = false;

                if (location != null)
                    return Tuple.Create(location.Latitude, location.Longitude);

                return Tuple.Create(0.0, 0.0);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }
    }
}
