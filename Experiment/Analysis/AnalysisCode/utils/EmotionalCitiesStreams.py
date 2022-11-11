from utils.dataloader import load_harp_stream, load_ubx_stream, load_accelerometer, load_empatica, load_microphone
import pandas as pd
import utils.ubx
import matplotlib.pyplot as plt
import datetime
import numpy as np
from enum import Enum
from utils.dataloader import _HARP_T0


class StreamType(Enum):
	NONE = None
	UBX = 'UbxStream'
	HARP = 'HarpStream'
	ACCELEROMETER = 'AccelerometerStream'
	MICROPHONE = 'MicrophoneStream'
	EMPATICA = 'EmpaticaStream'


class Stream:
	"""_summary_
	"""
	def __init__(self, device, streamlabel, root = '', data = None, autoload = True):
		self.device = device
		self.streamlabel = streamlabel
		self.rootfolder = root
		self.data = data
		self.autoload = autoload
		self.georeference = None
		self.streamtype = StreamType.NONE

	def plot(self, col = None , **kwargs):
		thisfigure = plt.figure()
		label = self.device + "." + self.streamlabel
		if col is None:
			plt.plot(self.data, **kwargs, label = label)
		else:
			plt.plot(self.data.loc[col], **kwargs, label = label)
		plt.xlabel("Time")
		plt.ylabel("Sensor value (a.u.)")
		plt.title(label)
		return thisfigure

	def slice(self, start = None, end = None):
		if (start is not None) & (end is not None):
			return self.data.loc[start:end]
		elif (start is None) & (end is not None):
			return self.data.loc[:end]
		elif (start is not None) & (end is None):
			return self.data.loc[start:]
		else:
			return self.data

	@staticmethod
	def resample_temporospatial(input_data, georeference,
                    sampling_dt = datetime.timedelta(seconds = 2)):
		resampled = georeference.loc[:,"Lat":"Height"].resample(sampling_dt, origin='start').mean()
		resampled['Data'] = np.NAN
		for i in np.arange(len(resampled)-1):
			resampled['Data'].iloc[i] = (input_data[
				(input_data.index >= resampled.index[i]) &
				(input_data.index < resampled.index[i+1])]).mean()
		resampled['Data'].iloc[i+1] = (input_data[input_data.index >= resampled.index[i+1]]).mean()
		return resampled

class HarpStream(Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, eventcode, **kw):
		super(HarpStream,self).__init__(**kw)
		self.eventcode = eventcode
		self.streamtype = StreamType.HARP
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_harp_stream(self.eventcode, root = self.rootfolder, throwFileError=False)

	def __str__(self):
		return f'Harp stream from device {self.device}, stream {self.streamlabel}({self.eventcode})'

	@staticmethod
	def to_seconds(index):
		return (index - np.datetime64(_HARP_T0)) / np.timedelta64(1, 's')

	@staticmethod
	def from_seconds(index):
		return (_HARP_T0 + pd.to_timedelta(index, 's')).values

class UbxStream(Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, **kw):
		super(UbxStream,self).__init__(**kw)
		self.positiondata = None
		self.clock_calib_model = None # Store the model here
		self.streamtype = StreamType.UBX
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_ubx_stream(root = self.rootfolder)

	def filter_event(self, event):
		return utils.ubx.filter_ubx_event(self.data, event)

	def parseposition(self, event = "NAV-HPPOSLLH", calibrate_clock = True):
		NavData = self.filter_event(event)
		NavData.insert(NavData.shape[1], "Lat", NavData.apply(lambda x : x.Message.lat, axis = 1), False)
		NavData.insert(NavData.shape[1], "Lon", NavData.apply(lambda x : x.Message.lon, axis = 1), False)
		NavData.insert(NavData.shape[1], "Height", NavData.apply(lambda x : x.Message.height, axis = 1), False)
		NavData.insert(NavData.shape[1], "Time_iTow", NavData.apply(lambda x : x.Message.iTOW, axis = 1), False)
		if calibrate_clock == True:
			iTow = NavData["Time_iTow"].values.reshape(-1,1)
			iTowCorrected = self.calibrate_itow(iTow)
			iTowCorrected = pd.DataFrame(HarpStream.from_seconds(iTowCorrected))
			iTowCorrected.columns = ["Seconds"]
			NavData.set_index(iTowCorrected["Seconds"], inplace = True)
		self.positiondata = NavData
		return NavData

	def calibrate_itow(self, input_itow_array):
		model = self.clock_calib_model
		calibrated_itow = model.predict(input_itow_array)
		return calibrated_itow

	def __str__(self):
		return f'Ubx stream from device {self.device}, stream {self.streamlabel}'

class AccelerometerStream(Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, **kw):
		super(AccelerometerStream,self).__init__(**kw)
		self.streamtype = StreamType.ACCELEROMETER
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_accelerometer(root = self.rootfolder)

	def __str__(self):
		return f'Accelerometer stream from device {self.device}, stream {self.streamlabel}'

class MicrophoneStream (Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, **kw):
		super(MicrophoneStream,self).__init__(**kw)
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_microphone(root = self.rootfolder)

	def __str__(self):
		return f'Microphone stream from device {self.device}, stream {self.streamlabel}'
class EmpaticaStream(Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, **kw):
		super(EmpaticaStream,self).__init__(**kw)
		self.streamtype = StreamType.EMPATICA
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_empatica(root = self.rootfolder)

	def __str__(self):
		return f'Empatica stream from device {self.device}, stream {self.streamlabel}'