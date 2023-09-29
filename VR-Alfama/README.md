* Glia folder needs to be intalled from the download SDK 

https://developers.hp.com/omnicept/docs/unity/getting-started

* The Unity Bonsai installation and associated .dlls are not tracked. The emotional-cities/unity .unitypackage should be added via the package manager and then installed using the BonsaiInstaller asset.

# Recovering the full Unity project from the Github repo

The github repo contains the most of the scripts and dependencies required to run the Unity project. However, some dependencies are developed separately as standalone Unity packages or libraries (or contain private information / keys that should not be tracked in the main repo). These must be manually installed.

You should recover the project in the order described below.

## HP Omnicept
Navigate to https://developers.hp.com/omnicept/downloads and download the latest HP Omnicept SDK. Inside your install location find the Unity pluging (e.g. Program Files/HP/HP Omnicept SDK/<version>/Glia.unitypackage). Import it into the Unity project with Assets --> Import Package --> Custom Package and import all.

Successfull import will add a new menu option in Unity for "HP Omnicept". Navigate here and click "Configure". This will open a dialog where you can enter the sensors to receive from the Omnicept. Ensure that the license type is set to 'Core' and that the Heart Rate, Eye Trackng, Camera Image and IMU sensors are enabled.

## Bonsai
~~~To install a local Bonsai environment in Unity to run the workflows, download the .unitypackage for Unity/Bonsai integration from https://github.com/emotional-cities/unity/releases/tag/v0.1. Import it with Assets --> Import Package --> Custom Package and import everything when prompted by the import dialog popup. This will create or populate 2 folders called "Bonsai" and "Packages" within the Assets folder. Navigate to the Bonsai folder and find the BonsaiInstaller asset. Click on it to open an inspector dialog to set up the Bonsai environment (Download, Install, Consolidate Packages).~~~

We will no longer use a local Unity Bonsai environment in favour of using the top-level pluma environment

## 3D Models and Content