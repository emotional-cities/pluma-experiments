from dotmap import DotMap
import pandas as pd

"""Stores the correspondence between stream labels and values
"""

_stream_labels = DotMap()

_stream_labels.BioData_EnableStream = 32
_stream_labels.BioData_DisableStream = 33
_stream_labels.BioData_ECG = 35
_stream_labels.BioData_GSR = 36
_stream_labels.BioData_Accelarometer = 37
_stream_labels.BioData_DigitalIn = 38
_stream_labels.BioData_Set = 39
_stream_labels.BioData_Clear = 40

_stream_labels.PupilLabs_LSLSampleTime = 220
_stream_labels.PupilLabs_LSLSampleArray = 221

_stream_labels.Microphone_BufferIndex = 222

_stream_labels.TK_AmbientLight_AmbientLight = 223

_stream_labels.TK_CO2V2_CO2Conc = 224
_stream_labels.TK_CO2V2_Temperature = 225
_stream_labels.TK_CO2V2_Humidity = 226

_stream_labels.TK_GPS_Latitude = 227
_stream_labels.TK_GPS_Longitude = 228
_stream_labels.TK_GPS_Altitude = 229
_stream_labels.TK_GPS_Data = 230
_stream_labels.TK_GPS_Time = 231
_stream_labels.TK_GPS_HasFix = 232

_stream_labels.TK_AirQuality_IAQIndex = 233
_stream_labels.TK_AirQuality_Temperature = 234
_stream_labels.TK_AirQuality_Humidity = 235
_stream_labels.TK_AirQuality_AirPressure = 236

_stream_labels.TK_SoundPressureLevel_SPL = 237

_stream_labels.TK_Humidity_Humidity = 238

_stream_labels.TK_AnalogIn_AnalogIn = 238

#_stream_labels_inv = {v: k for k, v in _stream_labels.toDict().items()}

class HarpStream:
  def __init__(self, code: int, label: str, data: pd.DataFrame):
    self.code = code
    self.label = label
    self.data = data