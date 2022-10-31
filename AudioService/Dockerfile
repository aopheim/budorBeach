FROM python:3.9-slim-bullseye

RUN apt-get update 
RUN apt-get install gcc libportaudio2 libportaudiocpp0 portaudio19-dev libsndfile1-dev -y
RUN pip3 install pyaudio wave bottle

COPY ./recordAudioLinux.py ./

CMD [ "python", "-u", "recordAudioLinux.py" ]