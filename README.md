**Project Code Description**

This section contains description and code documentation of the
project's main parts.

1.  **Technologies Used**

- **.NET MAUI**

Cross-platform app framework for building native apps with C#.

- **Google Drive API**  
  API for managing files in Google Drive.

- **Geolocation API  
  **For retrieving the user\'s current location.

- **MVVM Pattern  
  **Ensures clean separation between the user interface and the business
  logic.

2.  **Architecture Overview**

GeoDrive follows a modular and extensible architecture with the
following layers:

- **Presentation Layer**

Implements the user interface using .NET MAUI views.

- **Business Logic Layer**

Contains the core logic for file uploads and directory management.

- **Data Access Layer**

Handles interactions with Google Drive API and location services.

3.  **Solution Structure**

![A screenshot of a computer Description automatically
generated](media/image1.png){width="6.10101268591426in"
height="1.1732720909886265in"}

1.  **GeoDrive.Core**

> The core project contains all the logic and code for handling the
> drive actions. It also contains the data models that are used for
> requests and data transfer between layers.
>
> The services in this layer are:

- **GoogleDriveService**

This class is used to handle google drive actions

- **LocalFileServices**

This class is used to handle the local files in the testing phase of
this project

- **SampleDataService**

> This class is used to generate sample data in the testing phase of
> this project

1.  **GeoDrive.DroneConsle**

> The DroneConsole project contains a console application that is used
> for the testing phase of this project.
>
> The file in this layer is:

- **Program**

> This class is used as the main class for the testing console
> application used in the testing phase

- **App.config**

> This is a configuration file containing configurations used by the
> application in the testing phase

1.  **GeoDrive.MobileApp**

> The MobileApp project contains the UI and functionality of the mobile
> app of Geo Drive. This project is using .Net MAUI to create a cross
> platform mobile app. Most of the files and directories in this project
> are default for this framework.
>
> The main files and directories in this layer are:

- **MainPage.xaml**

> This is the UI file containing the structure of the main page of the
> system

- **MainPage.xaml.cs**

> This is the class containing the code behind that the UI use in the
> functionalities of the UI, including buttons, file pickers and labels.

- **Resources/Raw**

> This directory is used to save the credential files used in to
> authenticate with the google drive storage

4.  **Code Documentation**

    1.  **GoogleDriveService class**

![](media/image2.png){width="7.093610017497813in"
height="0.33421806649168856in"}

> **Description**:  
> Uploads files to Google Drive from a console application. Files are
> stored in folders named by geolocation data from the file contents.
>
> **Parameters:**

- credentialsPath (string): Path to the JSON credentials file.

- parentFolderId (string): ID of the parent folder in Google Drive.

- filesToUpload (string\[\]): Array of file paths to upload.

- simulateError (bool): Simulates an error for testing purposes
  (default: false).

> **Throws:**

- Exception: If there is an error reading a file or during the upload
  process.

![](media/image3.png){width="7.368832020997376in"
height="0.37159120734908135in"}

> **Description:**  
> Uploads a file to Google Drive from an Android application.
>
> **Parameters:**

- credentialsJson (string): The JSON credentials for Google Drive API.

- fileStream (Stream): The stream of the file to upload.

- fileName (string): The name of the file to upload.

- folderId (string): ID of the destination folder.

- simulateError (bool): Simulates an error for testing purposes
  (default: false).

> **Returns:**

- string: The ID of the uploaded file or \"None\" if upload fails.

> **Throws:**

- Exception: If an error occurs during the upload process.

![](media/image4.png){width="7.197740594925635in"
height="0.1814818460192476in"}

> **Description:**  
> Retrieves or creates a folder in Google Drive under the specified
> parent folder.
>
> **Parameters:**

- credentialsFile (string): Path to the JSON credentials file.

- folderName (string): Name of the folder to retrieve or create.

- parentFolderId (string): ID of the parent folder.

> **Returns:**

- string: The ID of the folder (existing or newly created).

  1.  **LocalFileServices class**

![](media/image5.png){width="6.5in" height="0.22361111111111112in"}

> **Description:**
>
> Reads the names of all .json files from the specified local storage
> directory and returns their full paths.
>
> **Parameters:**

- localStorageDirectory (string): The path to the local directory from
  which .json files will be retrieved.

> **Returns:**

- string\[\]: An array of full paths to .json files in the specified
  directory.

  1.  **Program class**

![](media/image6.png){width="3.835372922134733in"
height="0.2638867016622922in"}

> **Description:**
>
> The main execution method for the test console application. It
> initializes required services, reads configuration settings, generates
> sample data, and uploads files from the local directory to Google
> Drive.
>
> **Parameters**:

- args (string\[\]): Command-line arguments (currently unused).

  1.  **MainPage**

![](media/image7.png){width="4.698865923009624in"
height="0.2956867891513561in"}

> **Description:**
>
> Asynchronously retrieves the user\'s GPS coordinates and
> reverse-geocodes the location to determine the city name. Updates the
> UI with the location data.
>
> **Returns:**

- Task

> ![](media/image8.png){width="6.980742563429572in"
> height="0.23642060367454068in"}
>
> **Description:**
>
> Event handler triggered when the location name input is completed.
> Enables the upload button if the location name is valid.
>
> **Parameters:**

- sender (object): The source of the event.

- e (EventArgs): Event data.

![](media/image9.png){width="7.117567804024497in"
height="0.27679352580927385in"}

> **Description:**
>
> Handles the file upload process. Opens a file picker for the user to
> select files, then uploads them to Google Drive.
>
> **Parameters:**

- sender (object): The source of the event.

- e (EventArgs): Event data.

> **Exceptions:**
>
> Handles permission issues and other runtime exceptions during file
> selection or upload.

![](media/image10.png){width="7.222498906386702in"
height="0.2569542869641295in"}

> **Description:**
>
> Uploads a file stream to Google Drive under a folder associated with
> the current location.
>
> **Parameters:**

- fileStream (Stream): The file to upload.

- fileName (string): The name of the file.

> **Returns:**

- Task

![](media/image11.png){width="6.5in" height="0.21180555555555555in"}

> **Description:**
>
> Retrieves the last known GPS location of the device.

**Returns:**

- Task\<Tuple\<double, double\>\> - A tuple containing the latitude and
  longitude.

> **Exceptions:**
>
> Throws exceptions for unsupported features, disabled GPS, or missing
> permissions, with user-friendly messages.

![](media/image12.png){width="7.598458005249344in"
height="0.18184273840769904in"}

> **Description:**
>
> Performs reverse geocoding on the given latitude and longitude to
> fetch a human-readable address.

**Parameters:**

- latitude (double): The latitude of the location.

- longitude (double): The longitude of the location.

- Returns: Task\<string\> - A string representation of the address.
