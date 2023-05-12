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
 - Pupillabs pupil invisible
 - Harp-triggered I2C Accelarometer

## Walker fake monitor
In order to have a monitor that remote applications can target, we must emulate a fake hardware display. We are currently using the `IddSampleDriver` [(Instructions and download of Release 0.0.1 here.)](https://github.com/roshkins/IddSampleDriver/releases/tag/0.0.1)
## Tinker Forge 

### Install notes 
 - Bonsai reads from Tinkerforge trhough the Brick Deamon, so you should follow the Brick Daemon Installation on Windows https://www.tinkerforge.com/en/doc/Software/Brickd_Install_Windows.html#brickd-install-windows

 - Is also usefull to have the Brick viewer in order to be able to check the tinkerforge system and view outputs/inputs to all sensors connected. https://www.tinkerforge.com/en/doc/Software/Brickv.html
 - On Bonsai side you should install the tinkerforge nuget package.

### Execution Notes 
- Open Bonsai and insert a CreateBrickConnection node, check the port and host, but the default values should be ok for a local system with a clena inhstallation.
- connect that node to all specific tinkerforge sensor or actuator nodes that you have connected in your system, confure them properly (different components have differet set of settings). Don't forget to give the proper Uid (there is a dropdown that only shows compatible sensors to easy your life).

## Pupil Labs Pupil Invisible 0MQ
The Pupil Invisible Companion app uses the [NDSI v4 protocol](https://docs.pupil-labs.com/invisible/real-time-api/legacy-api/) to publish video streams from the world/eye cameras as well as gaze data and user-events. 
The app acts as a [Zyre](https://github.com/zeromq/zyre) host that contains data streams from individual sensors with a [specified protocol](https://github.com/pupil-labs/pyndsi/blob/v1.0/ndsi-commspec.md). 
Sensors on the Zyre host define a data endpoint (for receiving sensor data) and a command endpoint (for controlling the sensor, e.g. enabling streaming) both of which use 0MQ messaging. A sensor's data can be read with a SUB socket connected to the the data endpoint, and controlled with a PUSH socket connected to the command endpoint.

The general sequence for communicating with the Pupil Invisible is as follows:
- A Bonsai.Zyre ZyreNode attempts to the companion app which can be found with the Zyre group name pupil-mobile-v4.
- On a successful connection, the companion app will WHISPER all currently available sensors to the Bonsai ZyreNode, including their data/command endpoints.
- Bonsai filters the WHISPER messages according to which sensors are required for data collection.
- For those sensors a 0MQ message is sent with a PUSH socket to the command endpoint to activate streaming.
- For those sensors a SUB socket is used to read out their values.

In general, all sensor data streams are composed of three NetMQFrames for each sample:
- Sensor UID
- The data header
- The raw binary data payload (can be parsed using the data header).

The PupilInterface package provides some specific functionality for interfacing with the messages received from the pupil data streams. 
- The sensor information received from the Zyre group is received as JSON which needs to be parsed. The PupilSensor operator extracts and parses JSON in Zyre NetMQFrames to give the required sensor data for streaming.
- The world camera data is received as individual binary frames of H264 encoded data. The DecodeByteFrame operator instantiates a frame-by-frame ffmpeg decoder that decodes each binary frame into an image.

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
|                           |                 |               |                                               |
|       **PupilLabs**       | WorldCamera(Decoded)  |      209      |            Timestamped(HasFrame?)           |
|                           |    WorldCamera  |      210      |            Timestamped(FrameNumber)           |
|                           |        IMU      |      211      |            Timestamped(FrameNumber)           |
|                           |       Gaze      |      212      |            Timestamped(FrameNumber)           |
|                           |       Audio     |      213      |            Timestamped(FrameNumber)           |
|                           |        Key      |      214      |            Timestamped(FrameNumber)           |
