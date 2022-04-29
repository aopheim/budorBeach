import sounddevice as sd
import scipy.io.wavfile as wf 

print('Starting recording...')

fs = 44100  # Sample rate
seconds = 3  # Duration of recording

myrecording = sd.rec(int(seconds * fs), samplerate=fs, channels=2)
sd.wait()  # Wait until recording is finished
print('Writing file...')
wf.write('output.wav', fs, myrecording)  # Save as WAV file 
print('Finished!')