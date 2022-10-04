from utils.dataloader import load_harp_stream, load_ubx_stream, load_accelerometer, load_empatica, load_microphone
import utils.ubx
import matplotlib.pyplot as plt
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

	def add_georeference(self, df):
		if (isinstance(df, UbxStream)):
			self.georeference = df.parseposition()
		else:
			raise "Reference stream must be a UbxStream class."

	def spatial_position(self, interpolate = True, interpolate_type = 'linear'):
		"""_summary_

		Returns:
			_type_: _description_
		"""
		#The idea is to return the position of the sample.
		#Return for each entry in pandas. Interpolate?


class HarpStream(Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, eventcode, **kw):
		super(HarpStream,self).__init__(**kw)
		self.eventcode = eventcode
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_harp_stream(self.eventcode, root = self.rootfolder, throwFileError=False)

	def __str__(self):
		return f'Harp stream from device {self.device}, stream {self.streamlabel}({self.eventcode})'

class UbxStream(Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, **kw):
		super(UbxStream,self).__init__(**kw)
		self.positiondata = None
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_ubx_stream(root = self.rootfolder)

	def filter_event(self, event):
		return utils.ubx.filter_ubx_event(self.data, event)

	def parseposition(self, event = "NAV-HPPOSLLH"):
		NavData = self.filter_event(event)
		NavData.insert(NavData.shape[1], "Lat", NavData.apply(lambda x : x.Message.lat, axis = 1), False)
		NavData.insert(NavData.shape[1], "Lon", NavData.apply(lambda x : x.Message.lon, axis = 1), False)
		NavData.insert(NavData.shape[1], "Height", NavData.apply(lambda x : x.Message.height, axis = 1), False)
		NavData.insert(NavData.shape[1], "Time", NavData.apply(lambda x : x.Message.iTOW, axis = 1), False)

	def __str__(self):
		return f'Ubx stream from device {self.device}, stream {self.streamlabel}'

class AccelerometerStream(Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, **kw):
		super(AccelerometerStream,self).__init__(**kw)
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
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_empatica(root = self.rootfolder)

	def __str__(self):
		return f'Empatica stream from device {self.device}, stream {self.streamlabel}'