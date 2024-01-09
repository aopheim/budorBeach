# Troubleshooting making Docker image of BirdNetServer

- Using Python image arm32v7/python:3.11 gives continuous restart of the container with exit code 159
- Using image arm64v8/python:3.11 gives the same restart error.
- Same error using python:3.9-slim image. As discussed here: https://github.com/kahst/BirdNET-Analyzer/issues/119
- Using latest known working Dockerfile (from budorBeach repo). This is using arm32v7/python:3.7-buster. It builds and runs! Gives errors
  TypeError: 'type' object is not subscriptable
  Traceback (most recent call last):
  File "server.py", line 14, in <module>
  import analyze
  File "/analyze.py", line 13, in <module>
  import audio
  File "/audio.py", line 5, in <module>
  import config as cfg
  File "/config.py", line 72, in <module>
  ALLOWED_FILETYPES: list[str] = ['wav', 'flac', 'mp3', 'ogg', 'm4a', 'wma', 'aiff', 'aif']

Found some answers indicating that the Python version needs to be updated. Trying that...

This Dockerfile builds and runs!
FROM arm32v7/python:3.9.18

ENV LLVM*CONFIG=/usr/bin/llvm-config-14
RUN apt-get update \
 && apt-get install -y --no-install-recommends ffmpeg gcc build-essential cmake libatlas-base-dev clang llvm-14 llvm-14-dev && rm -rf /var/lib/apt/lists/* \  
 && pip install --upgrade pip \
 && pip install scipy==1.7.3 numpy==1.21.4 librosa==0.9.1 bottle==0.12.23 tflite==2.10.0 --compile --extra-index-url https://www.piwheels.org/simple --no-cache-dir \
 && pip install tflite-runtime==2.10.0 --compile --no-cache-dir \
 && apt-get auto-remove -y \
 && rm -rf /var/lib/apt/lists/\_

# Import all scripts

COPY . .
CMD [ "python", "server.py" ]

But gives runtime error
RuntimeError: module compiled against API version 0xf but this version of numpy is 0xe

Trying to upgrade numpy...

This Dockerfile runs, but fails with "Error during analysis: libffi.so.7: cannot open shared object file: No such file or directory"
FROM arm32v7/python:3.9.18

ENV LLVM*CONFIG=/usr/bin/llvm-config-14
RUN apt-get update \
 && apt-get install -y --no-install-recommends ffmpeg gcc build-essential cmake libatlas-base-dev clang llvm-14 llvm-14-dev && rm -rf /var/lib/apt/lists/* \  
 && pip install --upgrade pip \
 && pip install scipy numpy librosa bottle tflite --compile --extra-index-url https://www.piwheels.org/simple --no-cache-dir \
 && pip install tflite-runtime --compile --no-cache-dir \
 && apt-get auto-remove -y \
 && rm -rf /var/lib/apt/lists/\_

# Import all scripts

COPY . .
CMD [ "python", "server.py" ]

This Dockerfile also fails with the Error during analysis: libffi.so.7: cannot open shared object file: No such file or directory
FROM arm32v7/python:3.9.18

ENV LLVM_CONFIG=/usr/bin/llvm-config-14
RUN apt-get update && apt-get install -y --no-install-recommends ffmpeg gcc build-essential cmake libatlas-base-dev clang llvm-14 llvm-14-dev libopenblas-dev && rm -rf /var/lib/apt/lists/\*
RUN pip3 install ninja numpy scipy librosa bottle resampy tflite-runtime==2.10.0 --compile --extra-index-url https://www.piwheels.org/simple --no-cache-dir
COPY . .

CMD [ "python", "server.py" ]
