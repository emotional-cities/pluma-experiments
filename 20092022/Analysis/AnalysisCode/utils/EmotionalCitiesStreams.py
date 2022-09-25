from os import device_encoding
import pandas as pd
from utils.dataloader import load_harp_stream, load_ubx_stream, load_accelerometer, load_empatica, load_microphone

class Stream:
    def __init__(self, device, streamlabel, root = '', data = None, autoload = True):
        self.device = device
        self.streamlabel = streamlabel
        self.rootfolder = root
        self.data = data
        self.autoload = autoload

class HarpStream(Stream):
	def __init__(self, eventcode, **kw):
		super(HarpStream,self).__init__(**kw)
		self.eventcode = eventcode
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_harp_stream(self.eventcode, root = self.rootfolder, throwFileError=False)
class UbxStream(Stream):
	def __init__(self, **kw):
		super(UbxStream,self).__init__(**kw)
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_ubx_stream(root = self.rootfolder)

class AccelerometerStream(Stream):
	def __init__(self, **kw):
		super(AccelerometerStream,self).__init__(**kw)
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_accelerometer(root = self.rootfolder)

class MicrophoneStream (Stream):
	def __init__(self, **kw):
		super(MicrophoneStream,self).__init__(**kw)
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_microphone(root = self.rootfolder)

class EmpaticaStream(Stream):
	def __init__(self, **kw):
		super(EmpaticaStream,self).__init__(**kw)
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_empatica(root = self.rootfolder)