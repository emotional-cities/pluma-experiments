import os
import warnings

import numpy as np

def load_microphone(filename = 'Microphone.bin', root = ''):
    try:
        micdata = np.fromfile(os.path.join(root, filename), dtype='int16').reshape(-1,2)
    except FileExistsError:
        warnings.warn(f'Microphone stream file {filename} could not be found.')
    except FileNotFoundError:
        warnings.warn(f'Microphone stream file {filename} could not be found.')
    return micdata
