import numpy as np
import pandas as pd

from scipy import signal

from EmotionalCities.Processing import utils

def heartrate_from_ecg(ecg_data_stream,
                       fs = 250, skip_slice = 4, max_heartrate_bpm = 200.0,
                       peak_height = 800, smooth_win = 10, invert = False):
    ## Load biodata

    ecg = ecg_data_stream.data
    if invert:
        ecg = ecg * (-1.0)
    ecg = ecg["Value0"].iloc[np.arange(len(ecg))[::skip_slice]].astype(np.float64) # sensor acquires at 250hz but saves at 1khz

    resampled = utils.butter_highpass_filter(ecg,5,fs)

    #Assume maximum heartrate of max_heartrate_bpm
    peaks, _  = signal.find_peaks(resampled, height = peak_height, distance = (1.0/(max_heartrate_bpm/60.0)) * fs )
    heartrate = (fs/np.diff(peaks)) * 60
    heartrate = np.convolve(heartrate, np.ones(smooth_win), 'same') / smooth_win
    heartrate[0:int(smooth_win/2)+1] = np.NaN # pad the invalid portion of the conv with NaN
    heartrate[-int(smooth_win/2)-1:-1] = np.NaN
    df = pd.DataFrame(index = ecg.index[peaks[1:]], copy = True)
    df['HeartRate'] = heartrate
    return (df, resampled)