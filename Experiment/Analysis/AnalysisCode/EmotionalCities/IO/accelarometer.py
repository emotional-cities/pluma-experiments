import os
import warnings

import pandas as pd
from EmotionalCities.IO.constants import _HARP_T0

_accelerometer_header = [
    'Orientation.X', 'Orientation.Y', 'Orientation.Z',
    'Gyroscope.X', 'Gyroscope.Y', 'Gyroscope.Z',
    'LinearAccl.X', 'LinearAccl.Y', 'LinearAccl.Z',
    'Magnetometer.X', 'Magnetometer.Y', 'Magnetometer.Z',
    'Accl.X', 'Accl.Y', 'Accl.Z',
    'Gravitiy.X', 'Gravitiy.Y', 'Gravitiy.Z',
    'SysCalibEnabled', 'GyroCalibEnabled','AccCalibEnabled', 'MagCalibEnabled',
    'Temperature', 'Seconds', 'SoftwareTimestamp']

def load_accelerometer(filename = 'accelerometer.csv', root = ''):
    try:
        df = pd.read_csv(os.path.join(root, filename), header = None, names= _accelerometer_header)
    except FileNotFoundError:
        warnings.warn(f'Accelerometer stream file {filename} could not be found.')
        return
    except FileExistsError:
        warnings.warn(f'Accelerometer stream file {filename} could not be found.')
        return
    df['Seconds'] = _HARP_T0 + pd.to_timedelta(df['Seconds'].values, 's')
    df['SoftwareTimestamp'] =  _HARP_T0 + pd.to_timedelta(df['SoftwareTimestamp'].values, 's')
    df.set_index('Seconds', inplace=True)
    return df