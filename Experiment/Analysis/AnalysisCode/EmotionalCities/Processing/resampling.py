import datetime
import numpy as np

def resample_temporospatial(input_data, georeference,
                    sampling_dt = datetime.timedelta(seconds = 2)):

		if input_data.empty:
			raise ValueError("Input dataframe is empty.")

		resampled = georeference.loc[:,"Lat":"Height"].resample(sampling_dt, origin='start').mean()
		resampled['Data'] = np.NAN
		for i in np.arange(len(resampled)-1):
			resampled['Data'].iloc[i] = (input_data[
				(input_data.index >= resampled.index[i]) &
				(input_data.index < resampled.index[i+1])]).mean()
		resampled['Data'].iloc[i+1] = (input_data[input_data.index >= resampled.index[i+1]]).mean()
		return resampled

def resample_temporospatial_circ(input_data, georeference,sampling_dt = datetime.timedelta(seconds = 2)):
    resampled = georeference.loc[:,"Lat":"Height"].resample(sampling_dt, origin='start').mean()
    resampled['Data'] = np.NAN
    for i in np.arange(len(resampled)-1):
        resampled['Data'].iloc[i] = circular_mean((input_data[
            (input_data.index >= resampled.index[i]) &
            (input_data.index < resampled.index[i+1])]))
    resampled['Data'].iloc[i+1] = circular_mean(input_data[input_data.index >= resampled.index[i+1]])
    return resampled

from scipy.stats import circmean
def circular_mean(x):
    return round(np.rad2deg(circmean(np.deg2rad(x))),2)