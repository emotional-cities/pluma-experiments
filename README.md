# Pluma Experiments

This repository contains data acquisition and benchmark workflows for the wearable data collection unit.

## Hardware

The wearable data collection unit (Pluma) integrates the following hardware components:

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
- Run the Brickv 2.4.22 from the startup menu. 

## Enobio EEG

### Execution Notes

- Open NIC2 application 
- Press use USB devices 
- Ensure that the device is connected with usb proper plug to the computer 
- Power on the device by pressing the side button.
- Press scan for devices.
- Select the device from the available that appear.
- Click use this device.
- Press the human face image on the left pannel.
- Select EEG_Benchmarks.
- Press Load Protocol.
- Wait for the synchronizing 
- Press Play to save Data 
- Start Boinsai workflow.

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

### To run the protocol:

  0. Make sure the cellphone is connected to the same network as the computer;
  1. Turn on the companion phone and open the `Invisible Companion` App;
  2. Select `Wearer`, click `Adjust` and follow the calibration procedure;
  3. Hit `Apply` and confirm by looking at the cellphone;
  4. The App shuold now remain open to keep the `Zyre Server` active;

  5. From Bonsai, just hit `Start` and it should be able to automatically start streaming the video data.

## Bonsai data logging

Most of the data currently being saved in Bonsai is packaged in a HARP message format. For each different event (different address) a new .bin file will be created.

## Synchronization

Synchronization is either being achieved at the software level (Bonsai) by timestamping a given sample with the latest timestamp available from the HARP device or through a hardware-level TTL strategy.

To achieve this, Bonsai is randomly toggling a digital output in the HARP behavior board every 8-16 seconds for 100ms. This signal is currently being logged in the GPS module, Bricklet's analog input. In the future we could use this to burn an LED in various camera streams if needed.

# Current event codes

## [Firmware](https://github.com/emotional-cities/pluma/blob/main/Firmware/EmotionalCities/app_ios_and_regs.h)

[//]: [(https://www.tablesgenerator.com/markdown_tables#)]

|         **Device**        |      **Stream**       | **Code** |   **Rate**    |                    **Obs**                    |
|:-------------------------:|:---------------------:|:--------:|:-------------:|:---------------------------------------------:|
|        **BioData**        |     EnableStreams     |    32    |       -       |  Enable Oximeter, ECG, GSR or Accelerometer   |
|                           |     DisableStreams    |    33    |       -       |  Enable Oximeter, ECG, GSR or Accelerometer   |
|                           |          ECG          |    35    |     1 kHz     |  [ECG](https://neurogears.sharepoint.com/:b:/s/EmotionalCities/EYOX02N88hRHnUCdREf_kq0BEoxvZY92nHfPOPZmq7Ua3Q?e=xWQPvN) and Photodiode stream @ 1 kHz (mv)       |
|                           |          GSR          |    36    |     4 kHz     |  GSR stream @ 4 Hz                            |
|                           |     Accelerometer     |    37    |     50 Hz     |  Accelerometer polling trigger @ 50 Hz        |
|                           |     Digital Inputs    |    38    |       -       |  GPS lock (0x1) and Auxiliary input (0x2)     |
|                           |    SynchPulse (Set)   |    39    |       -       |  Rising edge of pseudo-random TTL sequence    |
|                           |   SynchPulse (Clear)  |    40    |       -       |  Falling edge of pseudo-random TTL sequence   |
|       **Microphone**      |         Audio         |     -    |    44.1 kHz   |  Raw audio data saved to Microphone.bin       |
|                           |      BufferIndex      |   222    |    *10 Khz    |  Multiply by buffer size to get sample index  |
|         **TK-GPS**        |        Latitude       |   227    |      1 Hz     |  Depends on having GPS signal [GPS](https://www.tinkerforge.com/en/doc/Hardware/Bricklets/GPS_V2.html)           |
|                           |       Longitude       |   228    |      1 Hz     |  Depends on having GPS signal                 |
|                           |        Altitude       |   229    |      1 Hz     |  Depends on having GPS signal                 |
|                           |          Data         |   230    |      1 Hz     |  Date from tinkerforge GPS device             |
|                           |          Time         |   231    |      1 Hz     |  Time from tinkerforge GPS device             |
|                           |         HasFix        |   232    |      1 Hz     |  Depends on having GPS signal                 |
|        **TK-CO2V2**       |        CO2Conc        |   224    |       -       |  This sensor is not being used                |
|                           |      Temperature      |   225    |       -       |  This sensor is not being used                |
|                           |        Humidity       |   226    |       -       |  This sensor is not being used                |
|    **TK-AmbientLight**    |      AmbientLight     |   223    |       -       |  This sensor is not being used                |
|     **TK-AirQuality**     |       IAQ Index       |   233    |      1 Hz     |           [IAQ](https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Air_Quality.html)                               |
|                           |      Temperature      |   234    |      1 Hz     |  Measured at the position of the sensor       |
|                           |        Humidity       |   235    |      1 Hz     |  Measured at the position of the sensor       |
|                           |      AirPressure      |   236    |      1 Hz     |  Measured at the position of the sensor       |
| **TK-SoundPressureLevel** |          SPL          |   237    |    100 Hz     |   Output db*10  [Sound Pressure Bricklet](https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Sound_Pressure_Level.html)     |
|      **TK-Humidity**      |        Humidity       |   238    |               |   Output Rh% * 100 [Humidity v2](https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Humidity_V2.html)              |

|      **TK-AnalogIn**      |        AnalogIn       |   239    |               |                                               |
| **TK-Particulate Matter** |          PM1.0        |   240    |               |               Timestamped(int)                |
|                           |          PM2.5        |   241    |               |               Timestamped(int)                |
|                           |          PM10         |   242    |               |               Timestamped(int)                |
|     **TK-Dual0-20mA**     |      Solar-Light      |   243    |               |               Timestamped(int)                |
|     **TK-Thermoouple**    |      Radiant Temp     |   244    |               |               Timestamped(int)                |
|        **TK-PTC**         |        Air Temp       |   245    |               |               Timestamped(int)                |
|        **ATMOS22**        |       North Wind      |   246    |               |               Timestamped(float)              |
|                           |        East Wind      |   247    |               |               Timestamped(float)              |
|                           |        Gust Wind      |   248    |               |               Timestamped(float)              |
|                           |        Air Temp       |   249    |               |               Timestamped(float)              |
|                           |      XOrientation     |   250    |               |               Timestamped(float)              |
|                           |      YOrientation     |   251    |               |               Timestamped(float)              |
|                           |        NullValue      |   252    |               |               Timestamped(float)              |
|                           |                       |          |               |                                               |
|       **PupilLabs**       | WorldCamera (Decoded) |   209    |               |            Timestamped(HasFrame?)             |
|                           |    WorldCamera (Raw)  |   210    |               |            Timestamped(FrameNumber)           |
|                           |          IMU          |   211    |               |            Timestamped(FrameNumber)           |
|                           |         Gaze          |   212    |               |            Timestamped(FrameNumber)           |
|                           |         Audio         |   213    |               |            Timestamped(FrameNumber)           |
|                           |          Key          |   214    |               |            Timestamped(FrameNumber)           |
|       **Omnicept**        |      EyeTracking      |   215    |               |            Timestamped(long[])                |
|                           |       HeartRate       |   216    |               |            Timestamped(long[])                |
|                           |          IMU          |   217    |               |            Timestamped(long[])                |
|                           |         Mouth         |   218    |               |            Timestamped(long[])                |
|     **VRTransform**       |       VrTimestamp     |   219    |               |            Timestamped(long)                  |
|     **UnityImage**        |       VrTimestamp     |   220    |               |            Timestamped(long)                  |

# Simulation 
The VR simulation environment is in **VR-Alfama** folder, to set up the Unity enviroment follow the instructions in the README.md inside **VR-Alfama**