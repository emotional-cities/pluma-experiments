import numpy as np
import pandas as pd
import datetime
import os
from utils.ubx import read_ubx_file
from dotmap import DotMap
import warnings

'''IO import functions
'''
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
    seconds = _HARP_T0 + pd.to_timedelta(seconds, 's')
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


def get_stream_path(streamID, root = '', suffix = 'Streams_', ext = ''):
    _suffix = f'{suffix}{streamID}{ext}'
    return os.path.join(root, _suffix)

def load_harp_stream(streamID, root = '', throwFileError = True):
    path = get_stream_path(streamID, root)
    if os.path.isfile(path):
        data = read_harp_bin(path)
        return data
    else:
        if throwFileError:
            raise FileExistsError
        else:
            warnings.warn(f'Harp stream with Id {streamID} not found')
            return pd.DataFrame()

def load_ubx_stream(root = ''):
    bin_file = load_ubx_bin(root = root)
    csv_file = load_ubx_harp_ts(root = root)
    if (bin_file['Class'].values == csv_file['Class'].values).all():
        bin_file['Seconds'] = csv_file.index
        bin_file = bin_file.set_index('Seconds')
        return bin_file
    else:
        raise ValueError('Misalignment found between CSV and UBX arrays.')

def load_ubx_bin(filename = 'ubx.bin', root = ''):
    return read_ubx_file(os.path.join(root, filename))

def load_ubx_harp_ts(filename = 'ubx_harp_ts.csv', root = ''):
    df = pd.read_csv(os.path.join(root, filename), header = None, names = ('Seconds', 'Class', 'Identity'))
    df['Seconds'] = _HARP_T0 + pd.to_timedelta(df['Seconds'].values, 's')
    df.set_index('Seconds', inplace=True)
    return df

_accelerometer_header = [
    'Orientation.X', 'Orientation.Y', 'Orientation.Z',
    'Gyroscope.X', 'Gyroscope.Y', 'Gyroscope.Z',
    'LinearAccl.X', 'LinearAccl.Y', 'LinearAccl.Z',
    'Magnetometer.X', 'Magnetometer.Y', 'Magnetometer.Z',
    'Accl.X', 'Accl.Y', 'Accl.Z',
    'Gravitiy.X', 'Gravitiy.Y', 'Gravitiy.Z',
    'SysCalibEnabled', 'GyroCalibEnabled','AccCalibEnabled', 'MagCalibEnabled',
    'Temperature', 'Seconds', 'SoftwareTimestamp']

def load_accelerometer(filename = 'Accelarometer.csv', root = ''):
    df = pd.read_csv(os.path.join(root, filename), header = None, names= _accelerometer_header)
    df['Seconds'] = _HARP_T0 + pd.to_timedelta(df['Seconds'].values, 's')
    df.set_index('Seconds', inplace=True)
    return df

def load_empatica(filename = 'empatica_harp_ts.csv', root = ''):
    df = pd.read_csv(os.path.join(root, filename), names= ['Message', 'Seconds'], delimiter=',', header=1)
    df['Seconds'] = _HARP_T0 + pd.to_timedelta(df['Seconds'].values, 's')
    df.set_index('Seconds', inplace=True)
    df['StreamId'] = df['Message'].apply(lambda x: x.split(' ')[0])
    _dict = {}
    for label, group in df.groupby('StreamId'):
        _dict[label] = parse_empatica_stream(group)
    return DotMap(_dict)

def parse_empatica_stream(empatica_stream):
    stream_id = empatica_stream['Message'][0].split(' ')[0]
    if stream_id == 'E4_Acc':
        df_labels = ['Stream', 'E4_Seconds', 'AccX', 'AccY', 'AccZ']
    elif stream_id in ['E4_Hr', 'E4_Bvp','E4_Gsr', 'E4_Battery', 'E4_Ibi', 'E4_Tag', 'E4_Temperature']:
        df_labels = ['Stream', 'E4_Seconds', 'Value']
    elif stream_id == 'R':
        df_labels = ['Stream', 'Event', 'StreamSubscription', 'Status']
    else:
        raise (f'Unexpected empatica stream id label: {stream_id}. No parse is currently set.')
    df = empatica_stream['Message'].str.split(pat = ' ')
    df.columns = df_labels
    return df

def load_microphone(filename = 'Microphone.bin', root = ''):
    micdata = np.fromfile(os.path.join(root, filename), dtype='int16').reshape(-1,2)
    return micdata
