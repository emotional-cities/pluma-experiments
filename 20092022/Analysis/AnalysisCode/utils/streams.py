from dotmap import DotMap
from utils.EmotionalCitiesStreams import HarpStream, UbxStream, AccelerometerStream, EmpaticaStream, MicrophoneStream
import pickle
import os
class Dataset:

	def __init__(self, root, datasetlabel = '', ):
		self.rootfolder = root
		self.datasetlabel = datasetlabel
		self.georeference = None
		self.streams = None

	def export_streams(self, filename = None):
		if filename is None:
			filename = os.path.join(self.rootfolder, 'dataset.pickle')
		with open(filename, 'wb') as handle:
			pickle.dump(self.streams, handle, protocol=pickle.HIGHEST_PROTOCOL)

	def import_streams(self, filename = None):
		if filename is None:
			filename = os.path.join(self.rootfolder, 'dataset.pickle')
		with open(filename, 'rb') as handle:
				self.streams = pickle.load(handle)

	def populate_streams(self, root = None , autoload = False):
		if root is None:
			root = self.rootfolder

		streams = DotMap()
		# BioData streams
		streams.BioData.EnableStreams =               HarpStream(32, device = 'BioData', streamlabel = 'EnableStreams', root = root, autoload = autoload)
		streams.BioData.DisableStreams =              HarpStream(33, device = 'BioData', streamlabel = 'DisableStreams', root = root, autoload = autoload)
		streams.BioData.ECG =                         HarpStream(35, device = 'BioData', streamlabel = 'ECG', root = root, autoload = autoload)
		streams.BioData.GSR =                         HarpStream(36, device = 'BioData', streamlabel = 'GSR', root = root, autoload = autoload)
		streams.BioData.Accelarometer =               HarpStream(37, device = 'BioData', streamlabel = 'Accelarometer', root = root, autoload = autoload)
		streams.BioData.DigitalIn =                   HarpStream(38, device = 'BioData', streamlabel = 'DigitalIn', root = root, autoload = autoload)
		streams.BioData.Set =                         HarpStream(39, device = 'BioData', streamlabel = 'Set', root = root, autoload = autoload)
		streams.BioData.Clear =                       HarpStream(40, device = 'BioData', streamlabel = 'Clear', root = root, autoload = autoload)

		# PupilLabs streams
		streams.PupilLabs.LSLSampleTime =             HarpStream(220, device = 'PupilLabs', streamlabel = 'LSLSampleTime', root = root, autoload = autoload)
		streams.PupilLabs.LSLSampleArray =            HarpStream(221, device = 'PupilLabs', streamlabel = 'LSLSampleArray', root = root, autoload = autoload)


		# TinkerForge streams
		streams.TK.AmbientLight.AmbientLight =        HarpStream(223, device = 'TK', streamlabel = 'AmbientLight.AmbientLight', root = root, autoload = autoload)

		streams.TK.CO2V2.CO2Conc =                    HarpStream(224, device = 'TK', streamlabel = 'CO2V2.CO2Conc', root = root, autoload = autoload)
		streams.TK.CO2V2.Temperature =                HarpStream(225, device = 'TK', streamlabel = 'CO2V2.Temperature', root = root, autoload = autoload)
		streams.TK.CO2V2.Humidity =                   HarpStream(226, device = 'TK', streamlabel = 'CO2V2.Humidity', root = root, autoload = autoload)

		streams.TK.GPS.Latitude =                     HarpStream(227, device = 'TK', streamlabel = 'GPS.Latitude', root = root, autoload = autoload)
		streams.TK.GPS.Longitude =                    HarpStream(228, device = 'TK', streamlabel = 'GPS.Longitude', root = root, autoload = autoload)
		streams.TK.GPS.Altitude =                     HarpStream(229, device = 'TK', streamlabel = 'GPS.Altitude', root = root, autoload = autoload)
		streams.TK.GPS.Data =                         HarpStream(230, device = 'TK', streamlabel = 'GPS.Data', root = root, autoload = autoload)
		streams.TK.GPS.Time =                         HarpStream(231, device = 'TK', streamlabel = 'GPS.Time', root = root, autoload = autoload)
		streams.TK.GPS.HasFix =                       HarpStream(232, device = 'TK', streamlabel = 'GPS.HasFix', root = root, autoload = autoload)

		streams.TK.AirQuality.IAQIndex =              HarpStream(233, device = 'TK', streamlabel = 'AirQuality.IAQIndex', root = root, autoload = autoload)
		streams.TK.AirQuality.Temperature =           HarpStream(234, device = 'TK', streamlabel = 'AirQuality.Temperature', root = root, autoload = autoload)
		streams.TK.AirQuality.Humidity =              HarpStream(235, device = 'TK', streamlabel = 'AirQuality.Humidity', root = root, autoload = autoload)
		streams.TK.AirQuality.AirPressure =           HarpStream(236, device = 'TK', streamlabel = 'AirQuality.AirPressure', root = root, autoload = autoload)

		streams.TK.SoundPressureLevel.SPL =           HarpStream(237, device = 'TK', streamlabel = 'SoundPressureLevel.SPL', root = root, autoload = autoload)

		streams.TK.Humidity.Humidity =                HarpStream(238, device = 'TK', streamlabel = 'Humidity.Humidity', root = root, autoload = autoload)

		streams.TK.AnalogIn.Voltage =                 HarpStream(239, device = 'TK', streamlabel = 'AnalogIn.Voltage', root = root, autoload = autoload)

		# UBX streams
		streams.UBX =                                 UbxStream(device = 'UBX', streamlabel = 'UBX', root = root, autoload = autoload)

		# Accelerometer streams
		streams.Accelerometer =                       AccelerometerStream(device = 'Accelerometer', streamlabel = 'Accelerometer', root = root, autoload = autoload)

		# Empatica streams
		streams.Empatica =                            EmpaticaStream(device = 'Empatica', streamlabel = 'Empatica', root = root, autoload = autoload)

		# Microphone streams
		streams.Microphone.Audio =                    MicrophoneStream(device = 'Microphone', streamlabel = 'Audio', root = root, autoload = autoload)
		streams.Microphone.BufferIndex =              HarpStream(222, device = 'Microphone', streamlabel = 'BufferIndex', root = root, autoload = autoload)

		self.streams = streams
