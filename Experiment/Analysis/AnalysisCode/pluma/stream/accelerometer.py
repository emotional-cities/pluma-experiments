from pluma.stream import Stream, StreamType
from pluma.io.accelerometer import load_accelerometer


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