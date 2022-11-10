import numpy as np

class SyncTimestamp:
    def __init__(self, ts_array, seconds_conversion = (lambda x: x)) -> None:
        self.ts_array = ts_array
        self.seconds_conversion_factor = seconds_conversion
        self.zero()
        self.to_seconds()
        self.max_pairs = len(ts_array) - 1

    def to_seconds(self):
        if self.seconds_conversion_factor is not None:
            self.ts_array = self.seconds_conversion_factor(self.ts_array)

    def zero(self, idx = 0):
        self.ts_array = self.ts_array - self.ts_array[idx]

    def get_pair(self, pair_idx, order = 1):
        if pair_idx > self.max_pairs + (order - 1):
            raise ValueError ("Pair index is larger than the size of the array")
        else:
            return self.ts_array[pair_idx], self.ts_array[pair_idx + order], self.ts_array[pair_idx + order] - self.ts_array[pair_idx]

    def get_diff(self):
        return np.diff(self.ts_array)

    def compute_diff_matrix(self):
        d = np.expand_dims(self.ts_array, 1) - self.ts_array
        #return np.tril(d, k = 0)
        return d.transpose()

    def find_closest_pair(self, pair_diff_seconds, dt_error = 0.001, method = 'leq', return_first = True, min_pair_index = 0):
        available_methods = ('leq', 'eq')

        diff_mat = np.abs(self.compute_diff_matrix() - pair_diff_seconds)
        diff_mat = diff_mat[min_pair_index:, min_pair_index:]
        if dt_error is not None:
            if not np.any(diff_mat < dt_error):
                raise ValueError("All values above the minimum allowed dt_error.")

        if method == 'leq':
            arg_min = np.argwhere(diff_mat < dt_error)
        elif method == 'eq':
            arg_min = np.argwhere(diff_mat == np.min(diff_mat))
        else:
            raise ValueError(f"Method not known. Available methods are: {available_methods}")

        if return_first:
            return arg_min[0] + min_pair_index
        else:
            return [x + min_pair_index for x in arg_min]

    def __repr__(self) -> str:
        return str(self.ts_array)
    def __str__(self) -> str:
        return str(self.ts_array)
