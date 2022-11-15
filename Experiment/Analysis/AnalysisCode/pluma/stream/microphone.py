from pluma.stream import Stream, StreamType
from pluma.io.microphone import load_microphone

class MicrophoneStream (Stream):
	"""_summary_

	Args:
		Stream (_type_): _description_
	"""
	def __init__(self, **kw):
		super(MicrophoneStream,self).__init__(**kw)
		self.streamtype = StreamType.MICROPHONE
		if self.autoload:
			self.load()

	def load(self):
		self.data = load_microphone(root = self.rootfolder)

	def __str__(self):
		return f'Microphone stream from device {self.device}, stream {self.streamlabel}'
