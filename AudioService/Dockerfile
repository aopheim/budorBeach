FROM arm32v7/python:3.9-slim-bullseye

RUN apt-get update \
    && apt-get install gcc libportaudio2 libportaudiocpp0 portaudio19-dev libsndfile1-dev -y \ 
    && pip3 install pyaudio wave bottle --no-cache-dir
COPY recordAudioLinux.py .

CMD [ "python", "-u", "recordAudioLinux.py" ]