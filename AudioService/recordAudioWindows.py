import sounddevice as sd
import scipy.io.wavfile as wf 
import uuid


fs = 44100  # Sample rate
seconds = 10  # Duration of recording
while True:
    print('Starting recording...')
    myrecording = sd.rec(int(seconds * fs), samplerate=fs, channels=2)
    sd.wait()  # Wait until recording is finished
    print('Writing file...')
    fileName = uuid.uuid4().hex
    wf.write(fileName + '.wav', fs, myrecording)  # Save as WAV file 
    print('Finished!')