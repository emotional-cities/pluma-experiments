# Setup the full Unity project from the Github repo


The github repo contains the scripts and dependencies required to run the Unity project. 

However, some dependencies are developed separately as standalone Unity packages or libraries (or contain private information / keys that should not be tracked in the main repo). 

These must be manually installed.

You should Setup the project in the order described below.

## Mixed Reality Feature Tool
The [Mixed Reality Feature Tool](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool) is a new way for developers to discover, update, and add Mixed Reality feature packages into Unity projects. You can search packages by name or category, see their dependencies, and even view proposed changes to your projects manifest file before importing. If you've never worked with a manifest file before, it's a JSON file containing all your projects packages. Once you've validated the packages you want, the Mixed Reality Feature tool will download them into the project of your choice.
This should be the first package missing follow instructions on how to install it.

When running it:
1. Press start
2. Select the Unity project folder in the local repo (e.g. C:\Users\user\pluma-experiments\VR-Alfama)
3. Press discover features
4. Select platform support and tick 'Mixed Reality Moving Platform SDK'
5. Select platform support and tick 'Mixed Reality OpenXR plugin'
6. Select spatial audio and tick 'Microsoft Spatializer'
7. Select other features and tick 'Mixed Reality Input'

## Project assets, 3DModels and content

You need to create (or locate if already done) in your pc the Shared Objects folder with OneDrive. 

### Create:
To access the project Unity content you must ask NeuroGEARS to share the content folder (\Objects) with your microsoft user.
Add that folder (\Objects) to your onedrive in the computer.
> There is a user created for accessing the shared content 
> pluma@neurogears.org

### Locate
Use sharepoint (\Objects) folder location to complete the following action.

Then you need to link the sharepoint (\Objects) folder to a folder inside project Assets. (\Assets)
Open Command Prompt in the project Assets folder and
Use the command mklink from command prompt:

Example: 
> mklink /J Objects "C:\...OneDriveFolder...\Objects"

## TMPro import
Open VR-Alfama in Unity with Unity hub and start installing
Some UI components in the project use TextMeshPro which must be imported. With the Unity project open, from the menu bar click:
1. Window >> TextMeshPro >> Import TMP Essential Resources >> Import all
2. Window >> TextMeshPro >> Import TMP Examples and Extras >> Import all

## Reload unity project 
Close unity and open the project again

## HP Omnicept
Open VR-Alfama in Unity with Unity hub and start installing
Navigate to https://developers.hp.com/omnicept/downloads and download and install the latest HP Omnicept SDK.

In Unity import the Omnicept package by navigating into  Assets --> Import Package --> Custom Package, the package is inside your omnicept install location (e.g. Program Files/HP/HP Omnicept SDK/<version>/Unity/Glia.unitypackage). Import the package and select import all.

Successful import will add a new menu option in Unity for "HP Omnicept". Navigate here and click "Configure". This will open a dialog where you can enter the sensors to receive from the Omnicept. Ensure that the license type is set to 'Core' and that the Heart Rate, Eye Tracking, Camera Image and IMU sensors are enabled.

More info [here](https://developers.hp.com/omnicept/docs/unity/getting-started)

## Reload unity project 
Close unity and open the project again

## Setup the OpenXr 
- In Unity go to Edit->ProjectSettings
- Navigate to XR Plug-in Management 
- Check the box OpenXR -> Windows Mixed Reality Feature Group 
- Then select the OpenXR submenu 
- On "Play Mode OpenXR Runtime" and select "Windows Mixed Reality"
- Add a new interaction Profile and select HP reverb G2 Controller profile 
- Close project settings window and press PLAY

## Load unity VR-Alfama scene
If the Vr-Alfama scene is not loaded in Unity, open Assets/Scenes folder and double click the Vr-Alfama scene to load that scene.

## Press Play.


## pluma-vr
Reusable unity infrastructure for VR data acquisition
pluma-vr shouldbe already installed but if there is a problem or update is needed follow the following instructions


### Installing a package from GitHub in Unity
To install this package (or any properly constructed Unity package) from GitHub first open Window>>Package Manager in Unity. Click the '+' icon in the top left and select "Add package from Git URL...". To specify the URL of the package we use <URL>.git?path=<subfolder>#<branch>. E.g. to install the com.neurogears.plumavr package from this repo on the main branch type https://github.com/emotional-cities/pluma-vr.git?path=/com.neurogears.plumavr#main.
