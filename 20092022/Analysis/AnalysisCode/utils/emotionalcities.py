import os
import numpy as np
import pandas as pd
import datetime
from scipy import signal

from utils.streams import _stream_labels
import utils.dataloader as dataloader

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
