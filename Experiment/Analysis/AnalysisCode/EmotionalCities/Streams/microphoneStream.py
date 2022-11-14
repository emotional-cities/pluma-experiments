from EmotionalCities.Streams.stream import Stream, StreamType
from EmotionalCities.IO.microphone import load_microphone

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
