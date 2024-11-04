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

  0. In the backpack computer activate the hotspot with SSID Backpack;
  1. Ensure the hotspot power saving is disabled;
  2. Ensure the cellphone is connected to the backpack computer hotspot (named Backpack);
  3. Turn on the companion phone and open the `Invisible Companion` App;
  4. Select `Wearer`, click `Adjust` and follow the calibration procedure;
  5. On the cellphone, hit `Apply` and confirm the eye gaze marker position matches the expected gaze;
  6. The App should now remain open to keep the `Zyre Server` active;
  7. From Bonsai, hit `Start` and it should automatically start streaming the video data.

## Bonsai data logging

Most of the data currently being saved in Bonsai is packaged in a HARP message format. For each different event (different address) a new .bin file will be created.

## Synchronization

Synchronization is either being achieved at the software level (Bonsai) by timestamping a given sample with the latest timestamp available from the HARP device or through a hardware-level TTL strategy.

To achieve this, Bonsai is randomly toggling a digital output in the HARP behavior board every 8-16 seconds for 100ms. This signal is currently being logged in the GPS module, Bricklet's analog input. In the future we could use this to burn an LED in various camera streams if needed.

# Current event codes

## [Firmware](https://github.com/emotional-cities/pluma/blob/main/Firmware/EmotionalCities/app_ios_and_regs.h)

[//]: [(https://www.tablesgenerator.com/markdown_tables#)]

|         **Device**        |      **Stream**       | **Code** |    **Rate**    |                    **Obs**                    |
|:-------------------------:|:---------------------:|:--------:|:--------------:|:---------------------------------------------:|
|        **BioData**        |     EnableStreams     |    32    |       -        |  Enable Oximeter, ECG, GSR or Accelerometer   |
|                           |     DisableStreams    |    33    |       -        |  Enable Oximeter, ECG, GSR or Accelerometer   |
|                           |          ECG          |    35    |     1 kHz      |  [ECG][1] and Photodiode stream (mv)          |
|                           |          GSR          |    36    |     4  Hz      |  GSR stream                                   |
|                           |     Accelerometer     |    37    |     50 Hz      |  Accelerometer polling trigger                |
|                           |     Digital Inputs    |    38    |       -        |  GPS lock (0x1) and Auxiliary input (0x2)     |
|                           |    SynchPulse (Set)   |    39    |       -        |  Rising edge of pseudo-random TTL sequence    |
|                           |   SynchPulse (Clear)  |    40    |       -        |  Falling edge of pseudo-random TTL sequence   |
|       **Microphone**      |         Audio         |     -    |    44.1 kHz    |  Raw audio data saved to Microphone.bin       |
|                           |      BufferIndex      |   222    |    *10 Khz     |  Multiply by buffer size to get sample index  |
|         **TK-GPS**        |        Latitude       |   227    |      1 Hz      |  Depends on having GPS signal [GPS][2]        |
|                           |       Longitude       |   228    |      1 Hz      |  Depends on having GPS signal                 |
|                           |        Altitude       |   229    |      1 Hz      |  Depends on having GPS signal                 |
|                           |          Data         |   230    |      1 Hz      |  Date from tinkerforge GPS device             |
|                           |          Time         |   231    |      1 Hz      |  Time from tinkerforge GPS device             |
|                           |         HasFix        |   232    |      1 Hz      |  Depends on having GPS signal                 |
|        **TK-CO2V2**       |        CO2Conc        |   224    |       -        |  This sensor is not being used                |
|                           |      Temperature      |   225    |       -        |  This sensor is not being used                |
|                           |        Humidity       |   226    |       -        |  This sensor is not being used                |
|    **TK-AmbientLight**    |      AmbientLight     |   223    |       -        |  This sensor is not being used                |
|     **TK-AirQuality**     |       IAQ Index       |   233    |      1 Hz      |           [IAQ][3]                            |
|                           |      Temperature      |   234    |      1 Hz      |  Measured at the position of the sensor       |
|                           |        Humidity       |   235    |      1 Hz      |  Measured at the position of the sensor       |
|                           |      AirPressure      |   236    |      1 Hz      |  Measured at the position of the sensor       |
| **TK-SoundPressureLevel** |          SPL          |   237    |    100 Hz      |   Output db*10  [Sound Pressure Bricklet][4]  |
|      **TK-Humidity**      |        Humidity       |   238    |                |   Output Rh% * 100 [Humidity v2][5]           |
|      **TK-AnalogIn**      |        AnalogIn       |   239    |      100 Hz    |                                               |
| **TK-Particulate Matter** |          PM1.0        |   240    |      100 Hz    |          Timestamped(int) [µg/m³][6]          |
|                           |          PM2.5        |   241    |      100 Hz    |          Timestamped(int) [µg/m³][6]          |
|                           |          PM10         |   242    |      100 Hz    |          Timestamped(int) [µg/m³][6]          |
|     **TK-Dual0-20mA**     |      Solar-Light      |   243    |      100 Hz    |          Timestamped(int) [mA x 1000000][7]   |
|     **TK-Thermoouple**    |      Radiant Temp     |   244    |      100 Hz    |          Timestamped(int) [°C x 100][8]       |
|        **TK-PTC**         |        Air Temp       |   245    |      100 Hz    |          Timestamped(int) [°C x 100][9]       |
|        **ATMOS22**        |       North Wind      |   246    |      ~2 Hz     |          Timestamped(float) [m/s][10]         |
|                           |        East Wind      |   247    |      ~2 Hz     |          Timestamped(float) [m/s][10]         |
|                           |        Gust Wind      |   248    |      ~2 Hz     |          Timestamped(float) [m/s][10]         |
|                           |        Air Temp       |   249    |      ~2 Hz     |          Timestamped(float) [°C][10]          |
|                           |      XOrientation     |   250    |      ~2 Hz     |          Timestamped(float) [Angle (°)][10]   |
|                           |      YOrientation     |   251    |      ~2 Hz     |          Timestamped(float) [Angle (°)][10]   |
|                           |        NullValue      |   252    |      ~2 Hz     |          Timestamped(float)                   |
|  **BNO055-Accelerometer** |     Orientation X     |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     Orientation Y     |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     Orientation Z     |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     Gyroscope X       |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     Gyroscope Y       |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     Gyroscope Z       |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     LinearAccl X      |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     LinearAccl Y      |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     LinearAccl Z      |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     Magnetometer X    |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     Magnetometer Y    |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     Magnetometer Z    |    -     |      50 Hz     |            [Angle (°)][11]                    |
|                           |     Accl X            |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     Accl Y            |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     Accl Z            |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     Gravity X         |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     Gravity Y         |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     Gravity Z         |    -     |      50 Hz     |            [m/s²][11]                         |
|                           |     SysCalibEnabled   |    -     |      50 Hz     |            [Boolean][11]                      |
|                           |     GyroCalibEnabled  |    -     |      50 Hz     |            [Boolean][11]                      |
|                           |     AccCalibEnabled   |    -     |      50 Hz     |            [Boolean][11]                      |
|                           |     MagCalibEnabled   |    -     |      50 Hz     |            [Boolean][11]                      |
|                           |     SysCalibEnabled   |    -     |      50 Hz     |            [Boolean][11]                      |
|                           |     Temperature       |    -     |      50 Hz     |            Not used(??)                       |
|                           |     SoftwareTimestamp |    -     |      50 Hz     |            TimeStamp                          |
|     [**Empatica-E4**][12] |     E4_Acc            |    -     |    31.5 Hz     |    Axis: X(usb);Y(strap);Z(bottom) [m/s²][13] |
|                           |     E4_Battery        |    -     |    0.05 Hz     |      % of battery float [(0.0 to 1.0)][13]    |
|                           |     E4_Bvp            |    -     |      64 Hz     |      [Reflection of green and red light][14]  |
|                           |     E4_Gsr            |    -     |       4 Hz     |            [microsiemens][13]                 |
|                           |     E4_Hr             |    -     |    1.56 Hz     |            [beats per minute][13]             |
|                           |     E4_Ibi            |    -     |    1.56 Hz     |   Heart inter beat interval [in seconds][13]  |
|                           |     E4_Temperature    |    -     |       4 Hz     |        Wrist surface temperature in [ᵒC][13]  |
|                           |     E4_Tag            |    -     |       -        |        [Wristband button pressed][13]         |
|                           |     R                 |    -     |       -        |        Responses to commands sent to E4       |
|                           |     E4_Seconds        |    -     |       -        | Every stream is timestamped with E4 timestamp |
|       **PupilLabs**       | WorldCamera (Decoded) |   209    |      32 Hz     |            Timestamped(Boolean HasFrame?)     |
|                           |    WorldCamera (Raw)  |   210    |      32 Hz     |            Timestamped(FrameNumber)           |
|                           |         IMU           |   211    |       -        |    Not in use Timestamped(FrameNumber)        |
|                           |         Gaze          |   212    |      250 Hz    |            Timestamped(FrameNumber)           |
|                           |         Audio         |   213    |       -        |    Not in use Timestamped(FrameNumber)        |
|                           |         Key           |   214    |       -        |    Not in use Timestamped(FrameNumber)        |
|       **UBX**             |         UBX           |          |                |                                               |
|       **Omnicept**        |      EyeTracking      |   190    |                |            Timestamped(long[])                |
|                           |       HeartRate       |   191    |                |            Timestamped(long[])                |
|                           |          IMU          |   192    |                |            Timestamped(long[])                |
|                           |         Mouth         |   193    |                |            Timestamped(long[])                |
|     **VRTransform**       |       VrTimestamp     |   180    |                |            Timestamped(long)                  |
|    **Georeference**       |       VrTimestamp     |   181    |                |            Timestamped(long)                  |
|   **ScreenGrabImage**     |       VrTimestamp     |   182    |                |            Timestamped(int)                   |
|    **UnityNewScene**      |       VrTimestamp     |   185    |                |            Timestamped(long)                  |
|       **UnityITI**        |       VrTimestamp     |   186    |                |            Timestamped(long)                  |
|  **PointToOriginWorld**   |       VrTimestamp     |   187    |                |            Timestamped(long)                  |
|   **PointToOriginMap**    |       VrTimestamp     |   188    |                |            Timestamped(long)                  |

# Simulation 
The VR simulation environment is in **VR-Alfama** folder, to set up the Unity environment follow the instructions in the README.md inside **VR-Alfama**  


[1]: https://neurogears.sharepoint.com/:b:/s/EmotionalCities/EYOX02N88hRHnUCdREf_kq0BEoxvZY92nHfPOPZmq7Ua3Q?e=xWQPvN  
[2]: https://www.tinkerforge.com/en/doc/Hardware/Bricklets/GPS_V2.html  
[3]: https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Air_Quality.html  
[4]: https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Sound_Pressure_Level.html  
[5]: https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Humidity_V2.html  
[6]: https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Particulate_Matter.html  
[7]: https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Industrial_Dual_020mA_V2.html#industrial-dual-0-20ma-v2-bricklet  
[8]: https://www.tinkerforge.com/en/doc/Hardware/Bricklets/Thermocouple_V2.html  
[9]: https://www.tinkerforge.com/en/doc/Hardware/Bricklets/PTC_V2.html  
[10]: https://metergroup.com/products/atmos-22/atmos-22-support/   
[11]: https://learn.adafruit.com/adafruit-bno055-absolute-orientation-sensor/overview  
[12]: https://box.empatica.com/documentation/20141119_E4_TechSpecs.pdf  
[13]: https://developer.empatica.com/windows-streaming-server-data.html  
[14]: https://support.empatica.com/hc/en-us/articles/360029719792-E4-data-BVP-expected-signal  
