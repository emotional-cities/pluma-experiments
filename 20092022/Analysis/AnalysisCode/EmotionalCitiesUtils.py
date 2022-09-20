import os
import numpy as np
import pandas as pd
import datetime
from scipy import signal

_SECONDS_PER_TICK = 32e-6
_HARP_T0 = datetime.datetime(1904, 1, 1)

_payloadtypes = {
    1 : np.dtype(np.uint8),
    2 : np.dtype(np.uint16),
    4 : np.dtype(np.uint32),
    8 : np.dtype(np.uint64),
    129 : np.dtype(np.int8),
    130 : np.dtype(np.int16),
    132 : np.dtype(np.int32),
    136 : np.dtype(np.int64),
    68 : np.dtype(np.float32)
}

_stream_labels = {
    'BioData_EnableStream' : 32,
    'BioData_DisableStream' : 33,
    'Biodata_ECG' : 35,
    'Biodata_GSR': 36,
    'BioData_DigitalIn' : 38,
    'BioData_Set' : 39,
    'BioData_Clear' : 40,

    'Microphone_BufferIndex' : 222,

    'TK-GPS_Latitude' : 227,
    'TK-GPS_Longitude' : 228,
    'TK-GPS_Altitude' : 229,
    'TK-GPS_Data' : 230,
    'TK-GPS_Time' : 231,
    'TK-GPS_HasFix' : 232,

    'TK-CO2V2_CO2Conc' : 224,
    'TK-CO2V2_Temperature' : 225,
    'TK-CO2V2_Humidity' : 226,

    'TK-AmbientLight_AmbientLight' : 223,

    'TK-AirQuality_IAQIndex' :233,
    'TK-AirQuality_Temperature' : 234,
    'TK-AirQuality_Humidity' : 235,
    'TK-AirQuality_AirPressure' : 236,

    'TK-SoundPressureLevel_SPL' : 237,

    'TK-Humidity_Humidity' : 238,

    'TK-AnalogIn_AnalogIn' : 239,

    'PupilLabs_LSLSampleTime' : 220,
    'PupilLabs_LSLSampleArray' : 221,
}

def read_harp_bin(file, time_offset = 0):
    '''Reads data from the specified Harp binary file.'''
    data = np.fromfile(file, dtype=np.uint8)
    if len(data) == 0:
        return None

    stride = data[1] + 2
    length = len(data) // stride
    payloadsize = stride - 12
    payloadtype = _payloadtypes[data[4] & ~0x10]
    elementsize = payloadtype.itemsize
    payloadshape = (length, payloadsize // elementsize)
    seconds = np.ndarray(length, dtype=np.uint32, buffer=data, offset=5, strides=stride)
    ticks = np.ndarray(length, dtype=np.uint16, buffer=data, offset=9, strides=stride)
    seconds = ticks * _SECONDS_PER_TICK + seconds
    seconds += time_offset
    seconds = pd.to_timedelta(seconds, 's')
    seconds.name = 'Seconds'

    payload = np.ndarray(
        payloadshape,
        dtype=payloadtype,
        buffer=data, offset=11,
        strides=(stride, elementsize))

    if payload.shape[1] ==  1:
        return pd.DataFrame(payload, index=seconds, columns = ['Value'])

    else:
        return pd.DataFrame(payload, index=seconds, columns = ['Value' + str(x) for x in np.arange(payload.shape[1])])


def get_stream_path(streamID, root = '', suffix = 'Streams_', ext = '.bin', inferFromDict = True):

    if inferFromDict is True:
        if streamID in _stream_labels:
            streamID = _stream_labels[streamID]
        else:
            raise Exception('{key} key not found in _stream_labels dictionary.'.format(key = streamID))

    suffix = suffix + '{id}' + ext
    return os.path.join(root, (suffix.format(id = streamID)))

def normalize_data(data_in):
    return (data_in - np.min(data_in))/ (np.max(data_in - np.min(data_in)))


_acc_colNames = ['Orientation.X','Orientation.Y','Orientation.Z',
'Gyroscope.X','Gyroscope.Y','Gyroscope.Z',
'LinearAccl.X','LinearAccl.Y','LinearAccl.Z',
'Magnetometer.X','Magnetometer.Y','Magnetometer.Z',
'Accl.X','Accl.Y','Accl.Z',
'Gravity.X','Gravity.Y','Gravity.Z',
'SysCalib','GyroCalib','AcclCalib','MagCalib',
'Temperature'
]

def load_accelarometer(path, header = None, colNames = None):
    if header is None:
        return  pd.read_csv(path, header = header, names = _acc_colNames)
    else:
        return  pd.read_csv(path, header = header, names = colNames)

def butter_highpass(cutoff, fs, order=5):
    nyq = 0.5 * fs
    normal_cutoff = cutoff / nyq
    b, a = signal.butter(order, normal_cutoff, btype='high', analog=False)
    return b, a

def butter_highpass_filter(data, cutoff, fs, order=5):
    b, a = butter_highpass(cutoff, fs, order=order)
    y = signal.filtfilt(b, a, data)
    return y