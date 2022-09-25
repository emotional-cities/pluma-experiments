from dotmap import DotMap
import pandas as pd
from utils.EmotionalCitiesStreams import HarpStream
import utils.dataloader

def populate_streams(root = ''):

  streams = DotMap()
  streams.BioData.EnableStreams = HarpStream(32, 'BioData', 'EnableStreams', root = root)
  streams.BioData.DisableStreams = HarpStream(33, 'BioData', 'DisableStreams', root = root)
  streams.BioData.ECG = HarpStream(35, 'BioData', 'ECG', root = root)
  streams.BioData.GSR = HarpStream(36, 'BioData', 'GSR', root = root)
  streams.BioData.Accelarometer = HarpStream(37, 'BioData', 'Accelarometer', root = root)
  streams.BioData.DigitalIn = HarpStream(38, 'BioData', 'DigitalIn', root = root)
  streams.BioData.Set = HarpStream(39, 'BioData', 'Set', root = root)
  streams.BioData.Clear = HarpStream(40, 'BioData', 'Clear', root = root)

  streams.PupilLabs.LSLSampleTime = HarpStream(220, 'PupilLabs', 'LSLSampleTime', root = root)
  streams.PupilLabs.LSLSampleArray = HarpStream(221, 'PupilLabs', 'LSLSampleArray', root = root)

  streams.Microphone.BufferIndex =  HarpStream(222, 'Microphone', 'BufferIndex', root = root)

  streams.TK.AmbientLight.AmbientLight = HarpStream(223, 'TK', 'AmbientLight.AmbientLight', root = root)

  streams.TK.CO2V2.CO2Conc = HarpStream(224, 'TK', 'CO2V2.CO2Conc', root = root)
  streams.TK.CO2V2.Temperature = HarpStream(225, 'TK', 'CO2V2.Temperature', root = root)
  streams.TK.CO2V2.Humidity = HarpStream(226, 'TK', 'CO2V2.Humidity', root = root)

  streams.TK.GPS.Latitude = HarpStream(227, 'TK', 'GPS.Latitude', root = root)
  streams.TK.GPS.Longitude = HarpStream(228, 'TK', 'GPS.Latitude', root = root)
  streams.TK.GPS.Altitude = HarpStream(229, 'TK', 'GPS.Latitude', root = root)
  streams.TK.GPS.Data = HarpStream(230, 'TK', 'GPS.Latitude', root = root)
  streams.TK.GPS.Time = HarpStream(231, 'TK', 'GPS.Latitude', root = root)
  streams.TK.GPS.HasFix = HarpStream(232, 'TK', 'GPS.Latitude', root = root)

  streams.TK.AirQuality.IAQIndex = HarpStream(233, 'TK', 'AirQuality.IAQIndex', root = root)
  streams.TK.AirQuality.Temperature = HarpStream(234, 'TK', 'AirQuality.IAQIndex', root = root)
  streams.TK.AirQuality.Humidity = HarpStream(235, 'TK', 'AirQuality.IAQIndex', root = root)
  streams.TK.AirQuality.AirPressure = HarpStream(236, 'TK', 'AirQuality.IAQIndex', root = root)

  streams.TK.SoundPressureLevel.SPL = HarpStream(237, 'TK', 'SoundPressureLevel.SPL', root = root)

  streams.TK.Humidity.Humidity = HarpStream(238, 'TK', 'Humidity.Humidity', root = root)

  streams.TK.AnalogIn.Voltage = HarpStream(239, 'TK', 'AnalogIn.Voltage', root = root)

  streams.UBX = utils.dataloader.load_ubx_stream(root = root)
  streams.Accelerometer = utils.dataloader.load_accelerometer(root = root)
  streams.Empatica = utils.dataloader.load_empatica(root = root)
  streams.Microphone = utils.dataloader.load_microphone(root = root)

  return streams
