from utils.dataloader import load_harp_stream, load_ubx_stream, load_accelerometer, load_empatica, load_microphone
import utils.ubx
class Stream:
	"""_summary_
	"""
    def __init__(self, device, streamlabel, root = '', data = None, autoload = True):
        self.device = device
        self.streamlabel = streamlabel
        self.rootfolder = root
        self.data = data
        self.autoload = autoload


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
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_ubx_stream(root = self.rootfolder)

	def filter_ubx_event(self, event):
		return utils.ubx.filter_ubx_event(self.data, event)

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