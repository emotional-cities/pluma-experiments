# Setup the full Unity project from the Github repo

This repo contains most of the scripts and dependencies required to run the Unity project.

However, some dependencies are developed separately as standalone Unity asset packages or libraries (or contain private information / keys that should not be tracked in the main repo), and therefore must be manually installed.

You should Setup the project in the order described below.

## Mixed Reality Feature Tool

The [Mixed Reality Feature Tool](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool) should be the first missing package. It is distributed by Microsoft and is used to discover, update, and add Mixed Reality feature packages into Unity projects.

Below are instructions on how to install and configure:
1. [Download the latest version of the Mixed Reality Feature Tool](https://aka.ms/MRFeatureTool)
2. Run `MixedRealityFeatureTool.exe`
3. Press the `Start` button
4. Select the Unity project folder in the local repo (e.g. `C:\Users\user\pluma-experiments\VR-Alfama`)
5. Press discover features
6. Select platform support and tick `Mixed Reality Moving Platform SDK`
7. Select platform support and tick `Mixed Reality OpenXR plugin`
8. Select spatial audio and tick `Microsoft Spatializer`
9. Select other features and tick `Mixed Reality Input`

## Project assets, 3D Models and content

You need to create (or locate if it already exists) the Shared `\Objects` folder on your PC OneDrive. 

### Create
To access the project Unity content you must ask NeuroGEARS to share the content folder `\Objects` with your Microsoft user. Add that folder `\Objects` to your OneDrive in the computer.

> If you are a member of the eMOTIONAL Cities consortium, you can use the `pluma@neurogears.org` account for accessing the shared content.

### Locate
Open Command Prompt in the project Assets folder `\Assets` and run the command `mklink` replacing `...OneDriveFolder...` with the location of the `\Objects` content folder in your OneDrive:

Example: 
```
mklink /J Objects "C:\...OneDriveFolder...\Objects"
```

This will make a link between the sharepoint `\Objects` folder and the local folder inside the project Assets.

## TMPro import

Some UI components in the project use TextMeshPro which must be imported. With the Unity project open, from the menu bar click:

1. `Window >> TextMeshPro >> Import TMP Essential Resources >> Import all`
2. `Window >> TextMeshPro >> Import TMP Examples and Extras >> Import all`

## Reload unity project 
Close unity and open the project again

## HP Omnicept

Navigate to https://developers.hp.com/omnicept/downloads and download and install the latest HP Omnicept SDK.

In Unity, import the Omnicept package by navigating into `Assets >> Import Package >> Custom Package`. A file explorer dialog will appear. Find and select the Omnicept package which is inside your omnicept install location (e.g. `Program Files/HP/HP Omnicept SDK/<version>/Unity/Glia.unitypackage`). On the import dialog select import all.

Successful import will add a new menu option in Unity for `HP Omnicept`. Navigate here and click `Configure`. This will open a dialog where you can enter the sensors to receive from the Omnicept. Ensure that the license type is set to `Core` and that the Heart Rate, Eye Tracking, Camera Image and IMU sensors are enabled.

More info [here](https://developers.hp.com/omnicept/docs/unity/getting-started).

## Reload unity project 
Close unity and open the project again

## Setup the OpenXR
1. In Unity go to `Edit >> ProjectSettings`
2. Navigate to XR Plug-in Management 
3. Check the box `OpenXR >> Windows Mixed Reality Feature Group`
4. Select the OpenXR submenu 
5. On `Play Mode OpenXR Runtime` select `Windows Mixed Reality`
6. Add a new interaction Profile and select HP reverb G2 Controller profile 
7. Close project settings window

## Load unity VR-Alfama scene
If the Vr-Alfama scene is not yet loaded in Unity, open `Assets/Scenes` folder and double click the Vr-Alfama scene.

## Press Play
If all the above instructions are followed successfully you should be able to run the Unity scene and look around through the VR Headset.


## pluma-vr

The pluma-vr package should be automatically installed from the project manifest, but if there is a problem or an update is needed follow the instructions below.

### Installing a package from GitHub in Unity
To install this package (or any properly constructed Unity package) from GitHub, first open `Window >> Package Manager` in Unity. Click the `+` icon in the top left and select `Add package from Git URL...`.

To specify the URL of the package you must use the pattern `<URL>.git?path=<subfolder>#<branch>`, e.g. to install the package from the main branch of the `pluma-vr` repo:

```
https://github.com/emotional-cities/pluma-vr.git?path=/com.neurogears.plumavr#main
```