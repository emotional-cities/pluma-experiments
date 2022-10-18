# Experiment Notes

This experiment will test the integration of various sources of hardware. These include:

 - Bricklets
   - GPS
   - CO2
   - Ambient Light
   - Air Quality
   - Sound Pressure
   - Humidity
   - Analog input
 - Harp BioData board (ECG, synching, clock reference)
 - GPS Module (ZED-F9P)
 - Biaural audio
	* no audio will be collected, instead we will short an input from the synchpulse to the audiocard to benchmark latencies
 - Pupillabs pupil core
 - Harp-triggered I2C Accelarometer

## Tinker Forge 

### Install notes 
 - Bonsai reads from Tinkerforge trhough the Brick Deamon, so you should follow the Brick Daemon Installation on Windows https://www.tinkerforge.com/en/doc/Software/Brickd_Install_Windows.html#brickd-install-windows

 - Is also usefull to have the Brick viewer in order to be able to check the tinkerforge system and view outputs/inputs to all sensors connected. https://www.tinkerforge.com/en/doc/Software/Brickv.html
 - On Bonsai side you should install the tinkerforge nuget package.

### Execution Notes 
- Open Bonsai and insert a CreateBrickConnection node, check the port and host, but the default values should be ok for a local system with a clena inhstallation.
- connect that node to all specific tinkerforge sensor or actuator nodes that you have connected in your system, confure them properly (different components have differet set of settings). Don't forget to give the proper Uid (there is a dropdown that only shows compatible sensors to easy your life).


## Pupillabs pupilcore

Opening Pupil Capture ---
Open the Pupil Capture software after headset is plugged in. Adjust the eye cameras until the whole of each eye is visible. When the eye 3D model is stable, the eye capture windows will show blue outline around the eyes.

For the LSL relay to work, there needs to be a valid calibration (for the screen position) loaded, press the C button on the left to initiate calibration. It doesn't need to be a good calibration, just needs to finish to initiate the data stream. Also only needs to be done once, not on each startup. In the plugin manager (on the right of the window) ensure that the Pupil LSL relay plugin is enabled. To start the system recording open the recorded tab and set save directories etc. - then press the R button on the left (and again to stop). Note that the data from the LSL relay will still stream even if a recording isn't started, so record start not required to acquire data from LSL stream in Bonsai. If you change a plugin in the pupil_capture_settings folder, the changes will not take effect until the Pupil Capture software is restarted.

The Pupil Capture software that initiates the LSL relay and records the world camera uses python plugins for certain featues (including the LSL relay). It looks for these plugins always in the users home directory (C:/Users/xxx) in a folder called pupil_capture_settings. There doesn't seem to be any setting to change where this folder gets found so all plugins and changes to plugins must happen in this folder to be registered. (in the current experiment computer the folder is located at C:\Users\AndréAlmeida\pupil_capture_settings\plugins) - run "pupil_invisible_lsl_relay" in an anaconda prompt

N.B. there is some weirdness going on with the streaming rate here in the channel output
Eye capture for each eye is at about 120 fps, but the received data with LSL is at 240 (double).
Streams that are dependent on BOTH eyes (e.g. gaze position) seem to be indeed updated at 240.
Those that are independent based on each eye (e.g. diameter_2d) produce samples at 120 each but somehow get duplicated for each sample, probably either at the plugin level or pupil core level

```python
class SceneCameraGaze(Outlet):

    @property
    def name(self) -> str:
        return "pupil_capture"

    @property
    def event_key(self) -> str:
        return "gaze"

    def setup_channels(self):

        return (
            confidence_channel(), # confidence (1 chan both eyes) - 1 chan

            # *norm_pos_channels(), # 'screen' position, x and y value - 2 chan

            # *gaze_point_3d_channels(), # 3d gaze position - 3 chan

            # *eye_center_channels(), # 3d position of front of each eye (3 chan per eye) - 6 chan

            *gaze_normal_channels(), # 3d position for each eye (probably normal vector from eye center) 6 chan

            *diameter_2d_channels(), # 2d diameter each eye - 2 chan

            # *diameter_3d_channels(), # 3d diameter each eye - 2 chan

        )
```

## Pupil Labs Pupil Invisible Python Lsl Relay solution 
 
### Install notes 
 - To have in Bonsai pupil invible capture you will need to intall the pupil relay lsl from https://pupil-invisible-lsl-relay.readthedocs.io/en/stable/
    - pip install pupil-invisible-lsl-relay
 - In bonsai side you need to install Bonsai.lsl package.

### Execution Notes 
 - On the phone run the invisible Companion app 
 - Both phone and computer needs to be on the same wifi network
 - in python run pupil_invisible_lsl_relay
    - If discovery works you get a list of all connected devices.
        - Enter the index of the device you want to connect to.
    - If not on the device go to the menu-streaming 
        - there is an IP adrees : port of the device 
        - you should then from python run pupil_invisible_lsl_relay --device_address device_ip:device_port
 - On Bonsai side inser an lsl stream inlet 
    - name the stream to pupil_invisible_Gaze
    - Stream ChannelCount 2
    - Stream ChannelFormat Float32 

## Pupil Labs Pupil Invisible 0MQ

Pupil labs implements 0MQ messages using the NDSI protocol. https://github.com/pupil-labs/pyndsi/blob/v1.0/ndsi-commspec.md

### Execution Notes 
- pip install ndsi
- pip install opencv-python
- git clone https://github.com/pupil-labs/pyndsi.git
- navigate to the cloned folder 
- activate emotional cities conda env
- 



## Bonsai data logging

Most of the data currently being saved in Bonsai is packaged in a HARP message format. For each different event (different address) a new .bin file will be created.

## Synchronization
Synchronization is either being achieved at the software level (Bonsai) by timestamping a given sample with the latest timestamp available from the HARP device or through a hardware-level TTL strategy.
To achieve this, Bonsai is randomly toggling a digital output in the HARP behavior board every 10-30 seconds for 100ms. This signal is currently being logged in the GPS module, Bricklet's analog input. In the future we could use this to burn an LED in various camera streams if needed.


# Current event codes:

## [Firmware](https://github.com/emotional-cities/pluma/blob/main/Firmware/EmotionalCities/app_ios_and_regs.h)

[//]: [(https://www.tablesgenerator.com/markdown_tables#)]

|         **Device**        |    **Stream**   | **EventCode** |                    **Obs**                    |
|:-------------------------:|:---------------:|:-------------:|:---------------------------------------------:|
|        **BioData**        |       ???       |       32      |             Enable some sort of stream?       |
|                           |       ???       |       33      |             Disable some sort of stream?      |
|                           |       ECG       |       35      |            Array[2] = {ECG, Photodiode}       |
|                           |       GSR       |       36      |                                               |
|                           |  Accelarometer  |       37      |                                               |
|                           |  Digital Input  |       38      |         BitMask 0x1 (GPS lock) and 0x2        |
|                           | SynchPulse(Set) |       39      |                   BitMask 0x1                 |
|                           |SynchPulse(Clear)|       40      |                   BitMask 0x1                 |
|       **Microphone**      |   BufferIndex   |      222      | Raw data is saved to a separate .bin file (") |
|         **TK-GPS**        |     Latitude    |      227      |                                               |
|                           |    Longitude    |      228      |                                               |
|                           |     Altitude    |      229      |                                               |
|                           |       Data      |      230      |                                               |
|                           |       Time      |      231      |                                               |
|                           |      HasFix     |      232      |                                               |
|        **TK-CO2V2**       |     CO2Conc     |      224      |                                               |
|                           |   Temperature   |      225      |                                               |
|                           |     Humidity    |      226      |                                               |
|    **TK-AmbientLight**    |   AmbientLight  |      223      |                                               |
|     **TK-AirQuality**     |    IAQ Index    |      233      |                                               |
|                           |   Temperature   |      234      |                                               |
|                           |     Humidity    |      235      |                                               |
|                           |   AirPressure   |      236      |                                               |
| **TK-SoundPressureLevel** |       SPL       |      237      |                                               |
|      **TK-Humidity**      |     Humidity    |      238      |                                               |
|      **TK-AnalogIn**      |     AnalogIn    |      239      |                                               |
| **TK-Particulate Matter** |       PM1.0     |      240      |               Timestamped(int)                |
|                           |       PM2.5     |      241      |               Timestamped(int)                |
|                           |       PM10      |      242      |               Timestamped(int)                |
|     **TK-Dual0-20mA**     |   Solar-Light   |      243      |               Timestamped(int)                |
|     **TK-Thermoouple**    |   Radiant Temp  |      244      |               Timestamped(int)                |
|        **TK-PTC**         |     Air Temp    |      245      |               Timestamped(int)                |
|        **ATMOS22**        |    North Wind   |      246      |               Timestamped(float)              |
|                           |     East Wind   |      247      |               Timestamped(float)              |
|                           |     Gust Wind   |      248      |               Timestamped(float)              |
|                           |     Air Temp    |      249      |               Timestamped(float)              |
|                           |   XOrientation  |      250      |               Timestamped(float)              |
|                           |   YOrientation  |      251      |               Timestamped(float)              |
|                           |    NullValue    |      252      |               Timestamped(float)              |
|       **PupilLabs**       |  LSL-SampleTime |      220      |                                               |
|                           | LSL-SampleArray |      221      |                                               |
|                           |                 |               |                                               |

