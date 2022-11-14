import os
import warnings

import pyubx2 as ubx
import pandas as pd

from EmotionalCities.IO.constants import _HARP_T0

def read_ubx_file(path):
    '''
    Outputs a dataframe with messages from UBX file.
    '''
    print(f'Opening file {path}...')
    out = []
    with open(path, 'rb') as fstream:
        out = read(fstream)

    df = pd.DataFrame({'Message':out})
    df['Identity'] = df['Message'].apply(lambda x: x.identity)
    df['Class'] = df['Message'].apply(lambda x: x.identity.split('-')[0])
    df['Id'] = df['Message'].apply(lambda x: x.identity.split('-')[1])
    df['Length'] = df['Message'].apply(lambda x: x.length)

    print('Done.')
    return df

def filter_ubx_event(df, event):
    '''
    Outputs a dataframe containing the messages from a single event
    '''
    return df.loc[df['Identity'] == event, :]


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
    try:
        df = read_ubx_file(os.path.join(root, filename))
    except FileNotFoundError:
        warnings.warn(f'UBX stream file {filename} could not be found.')
        return
    except FileExistsError:
        warnings.warn(f'UBX stream file {filename} could not be found.')
        return
    return df

def load_ubx_harp_ts(filename = 'ubx_harp_ts.csv', root = ''):
    try:
        df = pd.read_csv(os.path.join(root, filename), header = None, names = ('Seconds', 'Class', 'Identity'))
    except FileNotFoundError:
        warnings.warn(f'UBX stream alignment file {filename} could not be found.')
        return
    except FileExistsError:
        warnings.warn(f'UBX stream alignment file {filename} could not be found.')
        return
    df['Seconds'] = _HARP_T0 + pd.to_timedelta(df['Seconds'].values, 's')
    df.set_index('Seconds', inplace=True)
    return df


def errhandler(err):
    '''
    Handles errors output by iterator.
    '''
    print(f'\nERROR: {err}\n')


def read(stream, errorhandler = errhandler, protfilter=2, quitonerror = 3, validate = True, msgmode = 0):
    '''
    Reads and parses UBX message data from stream.
    '''
    ubr = ubx.UBXReader(
        stream,
        protfilter=protfilter,
        quitonerror=quitonerror,
        validate=validate,
        msgmode=msgmode,
        parsebitfield=True,
    )
    return [ parsed_data for (_, parsed_data) in ubr.iterate(
        quitonerror=quitonerror, errorhandler=errorhandler
    )]

## TODO

#http://docs.ros.org/en/kinetic/api/ublox_msgs/html/msg/EsfMEAS.html
#https://cdn.sparkfun.com/assets/learn_tutorials/1/1/7/2/ZED-F9R_Interfacedescription__UBX-19056845_.pdf  @page 57
#integration document @p36 https://content.u-blox.com/sites/default/files/ZED-F9R_Integrationmanual_UBX-20039643.pdf?hash=undefined&_ga=2.212516247.229517523.1660666345-1786438948.1649933107
#dataType = 5 : z-axis gyroscope angular rate deg/s *2^-12 (signed 24bit)
#dataType = 11 : speed m/s * 1e-3 (signed)
#dataType = 12 : Gyro temperature (deg celsiues 1e-2) signed
#dataType = 13 : y-axis gyroscope angular rate deg/s *2^-12 (signed 24bit)
#dataType = 14 : x-axis gyroscope angular rate  deg/s *2^-12 (signed 24bit)
#dataType = 16 : Gyro x acc (m/s^2 *2^-10) signed
#dataType = 17 : Gyro y acc (m/s^2 *2^-10) signed
#dataType = 18 : Gyro z acc (m/s^2 *2^-10) signed

#def get_gyro_from_ubx(message):
#    return pd.Series([e, f, g])

## Parse accelerometer data

# ESF_MEAS = filter_event(ubx_data, "ESF-MEAS")

# _dataTypeDict = {
#     5 : 'angular_rate_z',
#     8 : 'left_wheel_tick',
#     9 : 'right_wheel_tick',
#     10 : 'speed_tick',
#     11 : 'speed',
#     12: 'temperature',
#     13 : 'angular_rate_y',
#     14 : 'angular_rate_x',
#     16 : 'gyro_acc_x',
#     17 : 'gyro_acc_y',
#     18 : 'gyro_acc_z'
# }

# [ESF_MEAS.assign(x = np.NAN) for x in _dataTypeDict.values()]


# for ii, row in ESF_MEAS.iterrows():
#     message = row['Message']
#     n = message.numMeas
#     for i in range(n):
#         datatype = getattr(message, f'dataType_0{i+1}')
#         datafield = getattr(message, f'dataField_0{i+1}')
#         ESF_MEAS.loc[ii,_dataTypeDict[datatype]] = datafield
# print(ESF_MEAS)
# acc = ESF_MEAS[~ESF_MEAS["gyro_acc_z"].isnull()]

# plt.figure()
# plt.plot(acc.index.values, acc.gyro_acc_z.values)
# plt.show()

# acc