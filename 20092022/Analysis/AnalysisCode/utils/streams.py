from dotmap import DotMap
from utils.EmotionalCitiesStreams import HarpStream, UbxStream, AccelerometerStream, EmpaticaStream, MicrophoneStream

def populate_streams(root = ''):

  streams = DotMap()
  streams.BioData.EnableStreams =               HarpStream(32, device = 'BioData', streamlabel = 'EnableStreams', root = root)
  streams.BioData.DisableStreams =              HarpStream(33, device = 'BioData', streamlabel = 'DisableStreams', root = root)
  streams.BioData.ECG =                         HarpStream(35, device = 'BioData', streamlabel = 'ECG', root = root)
  streams.BioData.GSR =                         HarpStream(36, device = 'BioData', streamlabel = 'GSR', root = root)
  streams.BioData.Accelarometer =               HarpStream(37, device = 'BioData', streamlabel = 'Accelarometer', root = root)
  streams.BioData.DigitalIn =                   HarpStream(38, device = 'BioData', streamlabel = 'DigitalIn', root = root)
  streams.BioData.Set =                         HarpStream(39, device = 'BioData', streamlabel = 'Set', root = root)
  streams.BioData.Clear =                       HarpStream(40, device = 'BioData', streamlabel = 'Clear', root = root)

  streams.PupilLabs.LSLSampleTime =             HarpStream(220, device = 'PupilLabs', streamlabel = 'LSLSampleTime', root = root)
  streams.PupilLabs.LSLSampleArray =            HarpStream(221, device = 'PupilLabs', streamlabel = 'LSLSampleArray', root = root)

  streams.Microphone.BufferIndex =              HarpStream(222, device = 'Microphone', streamlabel = 'BufferIndex', root = root)

  streams.TK.AmbientLight.AmbientLight =        HarpStream(223, device = 'TK', streamlabel = 'AmbientLight.AmbientLight', root = root)

  streams.TK.CO2V2.CO2Conc =                    HarpStream(224, device = 'TK', streamlabel = 'CO2V2.CO2Conc', root = root)
  streams.TK.CO2V2.Temperature =                HarpStream(225, device = 'TK', streamlabel = 'CO2V2.Temperature', root = root)
  streams.TK.CO2V2.Humidity =                   HarpStream(226, device = 'TK', streamlabel = 'CO2V2.Humidity', root = root)

  streams.TK.GPS.Latitude =                     HarpStream(227, device = 'TK', streamlabel = 'GPS.Latitude', root = root)
  streams.TK.GPS.Longitude =                    HarpStream(228, device = 'TK', streamlabel = 'GPS.Latitude', root = root)
  streams.TK.GPS.Altitude =                     HarpStream(229, device = 'TK', streamlabel = 'GPS.Latitude', root = root)
  streams.TK.GPS.Data =                         HarpStream(230, device = 'TK', streamlabel = 'GPS.Latitude', root = root)
  streams.TK.GPS.Time =                         HarpStream(231, device = 'TK', streamlabel = 'GPS.Latitude', root = root)
  streams.TK.GPS.HasFix =                       HarpStream(232, device = 'TK', streamlabel = 'GPS.Latitude', root = root)

  streams.TK.AirQuality.IAQIndex =              HarpStream(233, device = 'TK', streamlabel = 'AirQuality.IAQIndex', root = root)
  streams.TK.AirQuality.Temperature =           HarpStream(234, device = 'TK', streamlabel = 'AirQuality.IAQIndex', root = root)
  streams.TK.AirQuality.Humidity =              HarpStream(235, device = 'TK', streamlabel = 'AirQuality.IAQIndex', root = root)
  streams.TK.AirQuality.AirPressure =           HarpStream(236, device = 'TK', streamlabel = 'AirQuality.IAQIndex', root = root)

  streams.TK.SoundPressureLevel.SPL =           HarpStream(237, device = 'TK', streamlabel = 'SoundPressureLevel.SPL', root = root)

  streams.TK.Humidity.Humidity =                HarpStream(238, device = 'TK', streamlabel = 'Humidity.Humidity', root = root)

  streams.TK.AnalogIn.Voltage =                 HarpStream(239, device = 'TK', streamlabel = 'AnalogIn.Voltage', root = root)

  streams.UBX =                                 UbxStream(device = 'UBX', streamlabel = 'UBX', root = root)
  streams.Accelerometer =                       AccelerometerStream(device = 'Accelerometer', streamlabel = 'Accelerometer', root = root)
  streams.Empatica =                            EmpaticaStream(device = 'Empatica', streamlabel = 'Empatica', root = root)
  streams.Microphone =                          MicrophoneStream(device = 'Microphone', streamlabel = 'Microphone', root = root)

  return streams
