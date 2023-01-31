# LSL Jitter Experiment

## Experiment
The experiment aims to determine the temporal jitter in LSL timestamps using different postprocessing clock correction
methods. LSL streams on a local network by default do not perform any clock correction, but StreamInlets can be configured
to use NTP to continuously correct the local clocks of LSL streams running on different machines on a network.

The experiment Bonsai workflow is in src/lsl-jitter-test and is run on two separate machines connected on a WiFi
network. These machines are also connected to two separate Harp Expander devices. These devices are connected via a 
digital output line and a clock synchroniser. A random timer on the first maching triggers the first Harp Expander to 
write to its digital output line, and simultaneously push a sample to its StreamOutlet containing the Harp timestamp of 
the write operation. The second machine reads the digital output and triggers its own StreamOutlet with the time of
the read operation. On either a 3rd machine (or one of the first two machines), StreamInlets read and Zip the two
LSL timestamps + Harp timestamps produced.

Since the LSL timestamps are produced simultaneously by the shared HARP trigger, the difference between them can
be used to estimate the error in LSL network timestamp correction. The data in received-timestamps-<proc-all>/
<proc-clksync>/<proc-all> represents timestamps collected with different StreamInlet processing options:
- proc-none: no clock correction is performed
- proc-clksync: the LSL network attempts to synchronise the clocks between the machines but performs no other correction.
- proc-all: the LSL network performs all postprocessing options available, including clock sync, dejitter, and
monotonisation of the timestamps.

Analysis of the data is found in Analysis/analysis.ipynb