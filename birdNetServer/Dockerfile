FROM arm64v8/python:3.9.18
ENV LLVM_CONFIG=/usr/bin/llvm-config-14
RUN apt update && apt upgrade -y && apt install -y \
    cmake \ 
    ffmpeg \ 
    gdal-bin \  
    inotify-tools \ 
    libopenblas-dev \ 
    libatlas-base-dev \ 
    libc6 \ 
    libgcc-s1 \
    libhdf5-dev \ 
    libhdf5-serial-dev \
    llvm-14 \ 
    llvm-14-dev 
COPY ./deb/libffi7_3.3-6_arm64.deb /usr/src/app/
RUN apt install /usr/src/app/libffi7_3.3-6_arm64.deb

# COPY ./whl/numba-0.58.1-cp39-cp39-linux_armv7l.whl /usr/src/app/
# COPY ./whl/tflite-2.10.0-py2.py3-none-any.whl /usr/src/app/
COPY ./requirements.txt .
RUN pip3 install -r requirements.txt --extra-index-url=https://www.piwheels.org/simple
COPY . .

CMD [ "python", "server.py" ]