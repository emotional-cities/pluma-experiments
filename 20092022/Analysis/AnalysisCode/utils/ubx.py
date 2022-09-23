import pyubx2 as ubx
import pandas as pd


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
