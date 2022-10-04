from dotmap import DotMap
from utils.EmotionalCitiesStreams import HarpStream, UbxStream, AccelerometerStream, EmpaticaStream, MicrophoneStream
import pickle
import os
import tilemapbase as tmb
import numpy as np
import matplotlib.pyplot as plt
class Dataset:

	def __init__(self, root, datasetlabel = '', ):
		self.rootfolder = root
		self.datasetlabel = datasetlabel
		self.georeference = None
		self.streams = None
		self.georeference = None

	def add_georeference(self, ubxstream = None, event = 'NAV-HPPOSLLH'):
		if ubxstream is None:
			try:
				NavData = self.streams.UBX.filter_event(event)
			except:
				raise 'Could not load Ubx stream.'
		else:
			if not isinstance(ubxstream, UbxStream):
				raise "Reference must be a UbxStream class instance"
			else:
				NavData = ubxstream.filter_event(event)
		NavData.insert(NavData.shape[1], "Lat", NavData.apply(lambda x : x.Message.lat, axis = 1), False)
		NavData.insert(NavData.shape[1], "Lon", NavData.apply(lambda x : x.Message.lon, axis = 1), False)
		NavData.insert(NavData.shape[1], "Height", NavData.apply(lambda x : x.Message.height, axis = 1), False)
		NavData.insert(NavData.shape[1], "Time", NavData.apply(lambda x : x.Message.iTOW, axis = 1), False)
		self.georeference = NavData

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


	### Visualization ###
	def showmap(self,
            NavData = None,
            figsize = (20,20),
            with_scaling = 0.6, to_aspect= (4/3),
            cmap = 'jet', markersize = 15, colorscale_override = None):
		if NavData is None:
			NavData = self.georeference #plot only the georeference basically
			NavData = NavData.assign(Data=1)
		else:
			if 'Data' not in NavData.columns:
				raise 'NavData input must have a "Data" Column. Use Stream.resample_temporospatial() for an example.'

		#import geopandas as gpd
		#coord = gpd.points_from_xy(NavData['Lon'], NavData['Lat'], NavData['Height'])
		#gdf = gpd.GeoDataFrame(geometry=coord, crs='epsg:4326')

		fig, ax = plt.subplots(1,1)
		fig.set_size_inches(figsize)
		tiles = tmb.tiles.build_OSM()
		extent = tmb.Extent.from_lonlat(np.min(NavData['Lon'].values), np.max(NavData['Lon'].values),
                                        np.min(NavData['Lat'].values), np.max(NavData['Lat'].values))
		extent = extent.to_aspect(to_aspect).with_scaling(with_scaling)
		ax.xaxis.set_visible(False)
		ax.yaxis.set_visible(False)
		plotter = tmb.Plotter(extent, tiles, width=600)
		plotter.plot(ax)

		path = [tmb.project(x,y) for x,y in zip(NavData['Lon'].values, NavData['Lat'].values)]
		x, y = zip(*path)

		if colorscale_override is None:
			colorscale_override = NavData['Data'].values
		ax.scatter(x, y, c = colorscale_override, s = markersize, cmap = cmap)
		return fig