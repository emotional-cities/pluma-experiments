import os
import warnings

import numpy as np
import pandas as pd

from EmotionalCities.IO.constants import _HARP_T0

_SECONDS_PER_TICK = 32e-6

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

