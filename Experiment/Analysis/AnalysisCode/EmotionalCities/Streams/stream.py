import datetime

import numpy as np
from enum import Enum
import matplotlib.pyplot as plt

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
		if self.data.empty:
			raise ValueError("Input dataframe is empty.")
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