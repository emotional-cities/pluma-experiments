import pandas as pd

from pluma.stream import Stream, StreamType
from pluma.io.ubx import load_ubx_stream, filter_ubx_event

class UbxStream(Stream):

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
		return filter_ubx_event(self.data, event)

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




