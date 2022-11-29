FROM arm32v7/python:3.7-buster

ENV LLVM_CONFIG=/usr/bin/llvm-config-11
RUN apt-get update \
    && apt-get install -y --no-install-recommends ffmpeg gcc build-essential cmake libatlas-base-dev clang llvm-11 llvm-11-dev && rm -rf /var/lib/apt/lists/* \  
    && pip install --upgrade pip \
    && pip install scipy==1.7.3 numpy==1.21.4 librosa==0.9.1 bottle==0.12.23 tflite==2.10.0 --compile --extra-index-url https://www.piwheels.org/simple --no-cache-dir \
    && apt-get auto-remove -y \
    && rm -rf /var/lib/apt/lists/*

# Import all scripts
COPY . .
CMD [ "python", "server.py" ]
