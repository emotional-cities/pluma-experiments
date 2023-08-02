# Travel CheckList

## Equipment List

### [Backpack](https://www.hp.com/us-en/shop/tech-takes/hp-vr-backpack-g2-review)
- [ ] Computer.
- [ ] Battery Charger + batt dock
- [ ] Batteries ( 4 units ).
- [ ] Power supply unit.
- [ ] Harness with all sensors attached (including the environmental sensors in the pole).
- [ ] External touch screen.

### [Eye tracking - Pupil Labs](https://pupil-labs.com/products/invisible/tech-specs/)
- [ ] Pupil labs case with:
  - [ ] Phone;
  - [ ] Glasses;
  - [ ] Front camera;
  - [ ] USB-C to USB-C cable.
- [ ] Pupil labs phone charger.

### [EEG Enobio 32](https://neuroelectrics.com/solutions/enobio/32)
- [ ] Neoprene headcap
- [ ] Electrode cable set for Enobio 32 (set is made by 3 independent cable connectors)
- [ ] Eeg device 
- [ ] Specific Enobio charger with USB mini.
- [ ] Enobio USB cable.

### [Empatica](https://support.empatica.com/hc/en-us/articles/202581999-E4-wristband-technical-specifications)
- [ ] Wristband.
- [ ] Docking station.
- [ ] Micro Usb charging cable.

### [Binaural sound recording](https://www.thomann.de/gb/soundman_okmii_incl_adapter_a3.htm)
- [ ] Soundman binaural microphone
- [ ] Battery phantom power unit.
- [ ] External [sound blaster G3](https://en.creative.com/p/sound-blaster/sound-blaster-g3) sound card.
- [ ] Additional 6 volts battery.
  
### [ECG](https://learn.sparkfun.com/tutorials/ad8232-heart-rate-monitor-hookup-guide/all)
- [ ] ECG black, red and blue cable.
- [ ] Set of [electrodes pads](https://thepihut.com/products/muscle-sensor-surface-emg-electrodes-h124sg-covidien) enough for all subjects multiplied by test locations multiplied by 3

### [Hard Transport case](https://www.pelican.com/)
- [ ] Packing Material
- [ ] Fit everything inside minus batteries
  
### Accessories 
- [ ] Ipad or equivalent tablet to remote in the computer.
- [ ] Access to the Internet, possible solutions:
   - [ ] **Backpack computer acting as an access point sharing the internect connection from a 3rd party phone**.
   - [ ] Pupil labs phone pprovidingg access point and connected to the Internet by using simd data card.
   - [ ] Access point with simd data card where pupil labs and Ipad connect by wifi and backpack connect by ethernet. May also be a USB 5g 
 - [ ] Zip ties.
 - [ ] Multitool.

## Setup before going on site
- [ ] Bateries
   - [ ] charge backpack computer
   - [ ] charge batery packs.
   - [ ] charge enubio NE
   - [ ] Charge pupil labs smartphone
   - [ ] Check (change?) mic batery.
   - [ ] Charge empatica 
- [ ] pupil labs
   - [ ] tape side camera camera to pupil glasees temple arm.
   - [ ] check camera expusure auto



## Setup
- [ ] Turn on equipment, check networks
- [ ] Empatica:
   - [ ] Turn on E4 streaming server
   - [ ] Turn on empatica
   - [ ] check if paired on E4 streaming server
   - [ ] Fit wristband tight
- [ ] Pupil Labs
   - [ ] connect usb cable, tape cable on the glasses temple tip.
   - [ ] start app [Invisible Companion](https://play.google.com/store/apps/details?id=com.pupillabs.invisiblecomp)
   - [ ] perform the eye calibration
- [ ] Soundman binaural microphone
   - [ ] Connect mic to pre amp. 
   - [ ] Check preamp light.
- [ ] BackPack
   - [ ] Bonsai check if receiving data from all sensors
      - [ ] tinkerforge 
      - [ ] wind sensor 
      - [ ] ubx gps 
      - [ ] accelarometer 
      - [ ] sound 
      - [ ] empatica 
      - [ ] pupil labs
  - [ ] Fit backpack on subject back
  - [ ] ecg
     - [ ] Fit gel electrodes (blue on left nipple, black on right nipple, red on belly alligned with black)
     - [ ] check ecg signal.
  - [ ] Stop bonsai
  - [ ] EEG
    - [ ] Fit eeg cap
    - [ ] Fit ear pinch reference electrode
    - [ ] Connect usb cable
    - [ ] Check flex cables from NE to eeg cap 
    - [ ] Turn on Ne
    - [ ] Start nic 
      - [ ] choose usb device 
      - [ ] start protocol 
      - [ ] allow syncronizing
      - [ ] wiggle red electrodes
  - [ ] Fit pupli glasses
  - [ ] Fit binaural mic earplugs

 ## Trials 
 - [ ] start recordinng EEG
 - [ ] press play on bonsai start
 - [ ] stop eeg
 - [ ] press Q on bonsai
 - [ ] stop bonsai 

## Remove setup
- [ ] remove binaural mic earplugs
- [ ] remove pupil glasses
- [ ] EEG
  - [ ] turn of NE
  - [ ] remove usb cable
  - [ ] remove ear pinch reference electrodes
  - [ ] remove eeg cap
- [ ] remove ecg electrodes.
- [ ] remove empatica wirstband
- [ ] remove backpack

### Support 
- [ ] Ipad or equivalent to remote in the computer.
- [ ] Access point where pupil labs Ipad and backpack should connect.
 