import numpy as np
import pandas as pd

from EmotionalCities.Streams.stream import Stream, StreamType
from EmotionalCities.IO.harp import load_harp_stream, _HARP_T0

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
