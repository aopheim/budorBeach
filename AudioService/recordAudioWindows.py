import sounddevice as sd
import scipy.io.wavfile as wf 
import uuid
import os


fs = 44100  # Sample rate
seconds = 10  # Duration of recording
while True:
    myrecording = sd.rec(int(seconds * fs), samplerate=fs, channels=2)
    sd.wait()  # Wait until recording is finished
    fileName = uuid.uuid4().hex
    filePath = os.getenv('APPDATA') + '\BudorBeach\\' + fileName + '.wav' 
    wf.write(filePath, fs, myrecording)  # Save as WAV file 
    print('Saved recording to ', filePath)