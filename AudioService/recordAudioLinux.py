from ctypes import *
import uuid
import wave
import pyaudio
import bottle
import datetime
from bottle import response
import json

# Following this guide: https://makersportal.com/blog/2018/8/23/recording-audio-on-the-raspberry-pi-with-python-and-a-usb-microphone

# Suppressing error messages as suggested here: https://stackoverflow.com/questions/7088672/pyaudio-working-but-spits-out-error-messages-each-time
# From alsa-lib Git 3fd4ab9be0db7c7430ebd258f2717a976381715d
# $ grep -rn snd_lib_error_handler_t
# include/error.h:59:typedef void (*snd_lib_error_handler_t)(const char *file, int line, const char *function, int err, const char *fmt, ...) /* __attribute__ ((format (printf, 5, 6))) */;
# Define our error handler type
ERROR_HANDLER_FUNC = CFUNCTYPE(
    None, c_char_p, c_int, c_char_p, c_int, c_char_p)


def py_error_handler(filename, line, function, err, fmt):
    pass

@bottle.route('/health', method='GET')
def healthcheck():
    data = {'msg': 'Healthy'}
    return json.dumps(data)

@bottle.route('/record', method='POST')
def makeRecording():
    print('Starting recording at ', datetime.datetime.now())
    record()
    print('Recording ended at ', datetime.datetime.now())
    data = {'msg': 'OK'}
    return json.dumps(data)

def record():
    c_error_handler = ERROR_HANDLER_FUNC(py_error_handler)

    asound = cdll.LoadLibrary('libasound.so')
    # Set error handler
    asound.snd_lib_error_set_handler(c_error_handler)

    form_1 = pyaudio.paInt16  # 16-bit resolution
    chans = 1  # 1 channel
    samp_rate = 44100  # 44.1kHz sampling rate
    chunk = 4096  # 2^12 samples for buffer
    record_secs = 10  # seconds to record
    dev_index = 1  # device index found by p.get_device_info_by_index(ii)
    wav_output_filename = '/audioRecordings/' + \
        str(uuid.uuid1()) + '.wav'  # name of .wav file

    audio = pyaudio.PyAudio()  # create pyaudio instantiation
    # create pyaudio stream
    stream = audio.open(format=form_1, rate=samp_rate, channels=chans,
                        input_device_index=dev_index, input=True,
                        frames_per_buffer=chunk)
    frames = []

    # loop through stream and append audio chunks to frame array
    for ii in range(0, int((samp_rate/chunk)*record_secs)):
        # Ignoring overflow exceptions: https://stackoverflow.com/questions/10733903/pyaudio-input-overflowed
        data = stream.read(chunk, exception_on_overflow=False)
        frames.append(data)

    # stop the stream, close it, and terminate the pyaudio instantiation
    stream.stop_stream()
    stream.close()
    audio.terminate()

    # save the audio frames as .wav file
    wavefile = wave.open(wav_output_filename, 'wb')
    wavefile.setnchannels(chans)
    wavefile.setsampwidth(audio.get_sample_size(form_1))
    wavefile.setframerate(samp_rate)
    wavefile.writeframes(b''.join(frames))
    wavefile.close()

    response.status = 200
    return response


if __name__ == '__main__':
    # Run server
    host = '0.0.0.0'
    port = '4000'
    print('UP AND RUNNING! LISTENING ON {}:{}'.format(host, port), flush=True)
    bottle.run(host=host, port=port, quiet=True)