from os import device_encoding
import pandas as pd
from utils.dataloader import load_harp_stream


class HarpStream:
	def __init__(self, eventcode, device, streamlabel, root = '', data = None):
		self.eventcode = eventcode
		self.device = device
		self.streamlabel = streamlabel
		self.rootfolder = root

		if data is None:
			self.data = self._load_harp_stream()
		else:
			self.data = data

	def _load_harp_stream(self):
		return load_harp_stream(self.eventcode, root = self.rootfolder, throwFileError=False)

