from os import device_encoding
import pandas as pd
from utils.dataloader import load_harp_stream


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

# class UbxStream:
#     class B(A):
#     def __init__(self,b,**kw):
#         self.b=b
#         super(B,self).__init__(**kw)