# Setup the full Unity project from the Github repo

The github repo contains the most of the scripts and dependencies required to run the Unity project. However, some dependencies are developed separately as standalone Unity packages or libraries (or contain private information / keys that should not be tracked in the main repo). These must be manually installed.

You should Setup the project in the order described below.

## Mixed Reality Feature Tool
The [Mixed Reality Feature Tool](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool) is a new way for developers to discover, update, and add Mixed Reality feature packages into Unity projects. You can search packages by name or category, see their dependencies, and even view proposed changes to your projects manifest file before importing. If you've never worked with a manifest file before, it's a JSON file containing all your projects packages. Once you've validated the packages you want, the Mixed Reality Feature tool will download them into the project of your choice.
This should be the first package missing follow instructions on how to install it.

When running it:
1- Go to settings and override any existing package file 
2- Select project path 
3- Press Restore Features 
 


## HP Omnicept
Open Unity and start intalling
Navigate to https://developers.hp.com/omnicept/downloads and download the latest HP Omnicept SDK. Inside your install location find the Unity pluging (e.g. Program Files/HP/HP Omnicept SDK/<version>/Glia.unitypackage). Import it into the Unity project with Assets --> Import Package --> Custom Package and import all.

Successfull import will add a new menu option in Unity for "HP Omnicept". Navigate here and click "Configure". This will open a dialog where you can enter the sensors to receive from the Omnicept. Ensure that the license type is set to 'Core' and that the Heart Rate, Eye Trackng, Camera Image and IMU sensors are enabled.

More info [here](https://developers.hp.com/omnicept/docs/unity/getting-started)

## pluma-vr
Reusable unity infrastructure for VR data acquisition

### Installing a package from GitHub in Unity
To install this package (or any properly constructed Unity package) from GitHub first open Window>>Package Manager in Unity. Click the '+' icon in the top left and select "Add package from Git URL...". To specify the URL of the package we use <URL>.git?path=<subfolder>#<branch>. E.g. to install the com.neurogears.plumavr package from this repo on the main branch type https://github.com/emotional-cities/pluma-vr.git?path=/com.neurogears.plumavr#main.

## Project assets, 3DModels and content
The project assets has the Objects folder shared with a sharepoint drive. 
There is the need to create a link between the two folders for that use the command mklink from command line.

Example: 
> mklink /J Objects "C:\Users\YourUserName\OneDrive - NeuroGEARS Ltd\Objects"
