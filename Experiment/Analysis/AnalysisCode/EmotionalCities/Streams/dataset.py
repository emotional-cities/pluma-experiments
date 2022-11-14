import pickle
import os

from EmotionalCities.Schemas.schema import build_schema
from EmotionalCities.Streams.Synchronization.ubx_harp import get_clockcalibration_ubx_to_harp_clock

from EmotionalCities.Streams.stream import StreamType

from EmotionalCities.Visualization import maps

class Dataset:

    def __init__(self, root, datasetlabel = '', ):
        self.rootfolder = root
        self.datasetlabel = datasetlabel
        self.georeference = None
        self.streams = None

    def add_ubx_georeference(self, ubxstream = None, event = "NAV-HPPOSLLH", calibrate_clock = True):
        if ubxstream is None:
            try:
                navdata = self.streams.UBX.parseposition(event = event, calibrate_clock = calibrate_clock)
            except:
                raise ImportError('Could not load Ubx stream.')
        else:
            if not(ubxstream.streamtype ==  StreamType.UBX):
                raise TypeError("Reference must be a UbxStream")
            else:
                navdata = ubxstream.parseposition(event = event, calibrate_clock = calibrate_clock)
        self.georeference = navdata

    def export_streams(self, filename = None):
        if filename is None:
            filename = os.path.join(self.rootfolder, 'dataset.pickle')
        with open(filename, 'wb') as handle:
            pickle.dump(self.streams, handle, protocol=pickle.HIGHEST_PROTOCOL)

    def import_streams(self, filename = None):
        if filename is None:
            filename = os.path.join(self.rootfolder, 'dataset.pickle')
        with open(filename, 'rb') as handle:
                self.streams = pickle.load(handle)

    def populate_streams(self, root = None , autoload = False):
        if root is None:
            root = self.rootfolder
        self.streams = build_schema(root = root, autoload = autoload)

    def calibrate_ubx_to_harp(self, bitmask = 3, dt_error = 0.002, plot_diagnosis = False, r2_min_qc = 0.99):
        self.streams.UBX.clock_calib_model = get_clockcalibration_ubx_to_harp_clock(ubx_stream=self.streams.UBX,
                                               harp_sync=self.streams.BioData.Set.data,
                                               bitmask = bitmask, dt_error = dt_error,
                                               r2_min_qc = r2_min_qc, plot_diagnosis=plot_diagnosis)

    def showmap(self, **kwargs):
        temp_df = self.georeference.assign(Data = 1)
        maps.showmap(temp_df, **kwargs)