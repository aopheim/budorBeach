FROM arm32v7/python:3.7-slim-buster

# Install required packages while keeping the image small
RUN apt-get update \
    && apt-get install -y --no-install-recommends ffmpeg gcc build-essential cmake && rm -rf /var/lib/apt/lists/* \  
    && pip install --upgrade pip \
    && pip install scipy==1.7.3 numpy==1.21.4 librosa==0.9.1 bottle==0.12.23 tflite==2.10.0 --no-deps --compile --extra-index-url https://www.piwheels.org/simple --no-cache-dir \
    && apt-get auto-remove -y \
    && rm -rf /var/lib/apt/lists/*

# Import all scripts
COPY . ./

# Add entry point to run the script
ENTRYPOINT [ "python3" ]
CMD [ "server.py" ]
