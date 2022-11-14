import os
import warnings

import numpy as np

def load_microphone(filename : str = 'Microphone.bin', root : str = '') -> np.array:
    """Loads microphone waveform data from a file into a numpy array.

    Args:
        filename (str, optional): Input file name to target. Defaults to 'Microphone.bin'.
        root (str, optional): Root path where filename is expected to be found. Defaults to ''.

    Returns:
        np.array: Array with raw waveform data from the microphone stream.
    """
    try:
        micdata = np.fromfile(os.path.join(root, filename), dtype='int16').reshape(-1,2)
    except FileExistsError:
        warnings.warn(f'Microphone stream file {filename} could not be found.')
    except FileNotFoundError:
        warnings.warn(f'Microphone stream file {filename} could not be found.')
    return micdata
