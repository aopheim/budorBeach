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

Using the above Dockerfile, built for arm32 builds, but gives error on startup:
Traceback (most recent call last):
File "//server.py", line 4, in <module>
from birdnetlib import Recording
File "/usr/local/lib/python3.9/site-packages/birdnetlib/**init**.py", line 1, in <module>
from birdnetlib.main import (
File "/usr/local/lib/python3.9/site-packages/birdnetlib/main.py", line 14, in <module>
import matplotlib.pyplot as plt
File "/usr/local/lib/python3.9/site-packages/matplotlib/**init**.py", line 161, in <module>
from . import \_api, \_version, cbook, \_docstring, rcsetup
File "/usr/local/lib/python3.9/site-packages/matplotlib/rcsetup.py", line 27, in <module>
from matplotlib.colors import Colormap, is_color_like
File "/usr/local/lib/python3.9/site-packages/matplotlib/colors.py", line 52, in <module>
from PIL import Image
File "/usr/local/lib/python3.9/site-packages/PIL/Image.py", line 84, in <module>
from . import \_imaging as core
ImportError: libtiff.so.5: cannot open shared object file: No such file or directory

This Dockerfile builds and runs, but gives the same libtiff error. Apparantly using 3.9.18-bullseye-slim made the build process not able to access llvm-14.
FROM arm32v7/python:3.9.18

ENV LLVM_CONFIG=/usr/bin/llvm-config-14
RUN apt update && apt install -y --no-install-recommends ffmpeg gcc build-essential cmake libatlas-base-dev clang llvm-14 llvm-14-dev libopenblas-dev && rm -rf /var/lib/apt/lists/\*
RUN pip3 install birdnetlib libtiff ninja numpy scipy librosa bottle resampy tflite-runtime==2.10.0 --compile --extra-index-url https://www.piwheels.org/simple --no-cache-dir
COPY . .

CMD [ "python", "server.py" ]

This Dockerfile builds. But gives the restarting (159) error on startup
FROM arm64v8/python:3.9.18
ENV LLVM_CONFIG=/usr/bin/llvm-config-14
RUN apt update && apt upgrade -y && apt install -y --no-install-recommends ffmpeg gcc build-essential cmake libatlas-base-dev clang libopenblas-dev && rm -rf /var/lib/apt/lists/\*
RUN pip3 install birdnetlib numpy==1.26.3 matplotlib ffmpeg ninja scipy librosa bottle resampy tflite-runtime
COPY . .

CMD [ "python", "server.py" ]

Trying to build image using modified version of this Dockerfile: https://github.com/joeweiss/birdnetlib-listener-device/blob/main/server/Dockerfile.rpi
Using arm32 python and tflite_runtime.whl
FROM arm32v7/python:3.9.18-bullseye
RUN apt update && apt upgrade -y && apt install -y inotify-tools ffmpeg cmake gdal-bin

COPY ./whl/numba-0.58.1-cp39-cp39-linux_armv7l.whl /usr/src/app/
COPY ./whl/tflite-2.10.0-py2.py3-none-any.whl /usr/src/app/
COPY ./requirements.txt .
RUN pip3 install -r requirements.txt
COPY . .

CMD [ "python", "server.py" ]

This Dockerfile builds, but gives runtime error libcblas.so.3: cannot open shared object file: No such file or directory
FROM arm32v7/python:3.9.18
ENV LLVM_CONFIG=/usr/bin/llvm-config-14
RUN apt update && apt upgrade -y && apt install -y inotify-tools ffmpeg cmake gdal-bin libopenblas-dev llvm-14 llvm-14-dev

COPY ./whl/numba-0.58.1-cp39-cp39-linux_armv7l.whl /usr/src/app/
COPY ./whl/tflite-2.10.0-py2.py3-none-any.whl /usr/src/app/
COPY ./requirements.txt .
RUN pip3 install -r requirements.txt --extra-index-url=https://www.piwheels.org/simple
COPY . .

CMD [ "python", "server.py" ]

With requirements:
appdirs==1.4.4
apprise==1.0.0
asgiref==3.5.2
attrs==22.1.0
audioread==2.1.9
bottle==0.12.25
birdnetlib==0.13.2
black==22.6.0
certifi==2022.6.15
cffi==1.15.1
charset-normalizer==2.1.0
click==8.1.3
colorama==0.4.4
darker==1.5.0
decorator==5.1.1
idna==3.3
importlib-metadata==4.12.0
iniconfig==1.1.1
joblib==1.1.0
librosa==0.9.2
llvmlite==0.41.1
mock==4.0.3
mypy-extensions==0.4.3
numba @ file:///usr/src/app/numba-0.58.1-cp39-cp39-linux_armv7l.whl
numpy==1.22
packaging==21.3
pathspec==0.9.0
platformdirs==2.5.2
pluggy==1.0.0
pooch==1.6.0
py==1.11.0
pycparser==2.21
pydub==0.25.1
pyparsing==3.0.9
pytest==7.1.2
python-dotenv==0.20.0
pytz==2022.2.1
requests==2.28.1
resampy==0.3.1

# scikit-learn==1.0.2

scipy==1.7.3
SoundFile==0.10.3.post1
sqlparse==0.4.2
tflite @ file:///usr/src/app/tflite-2.10.0-py2.py3-none-any.whl
threadpoolctl==3.1.0
toml==0.10.2
tomli==2.0.1
typed-ast==1.5.4
typing_extensions==4.3.0
urllib3==1.26.11
watchdog==2.1.9
zipp==3.8.1

This builds, giving error at startup: ImportError: libffi.so.7: cannot open shared object file: No such file or directory
Traceback (most recent call last):
File "//server.py", line 4, in <module>
from birdnetlib import Recording
File "/usr/local/lib/python3.9/site-packages/birdnetlib/**init**.py", line 1, in <module>
from birdnetlib.main import (
File "/usr/local/lib/python3.9/site-packages/birdnetlib/main.py", line 1, in <module>
import librosa
File "/usr/local/lib/python3.9/site-packages/librosa/**init**.py", line 209, in <module>
from . import core
File "/usr/local/lib/python3.9/site-packages/librosa/core/**init**.py", line 6, in <module>
from .audio import \* # pylint: disable=wildcard-import
File "/usr/local/lib/python3.9/site-packages/librosa/core/audio.py", line 8, in <module>
import soundfile as sf
File "/usr/local/lib/python3.9/site-packages/soundfile.py", line 17, in <module>
from \_soundfile import ffi as \_ffi
File "/usr/local/lib/python3.9/site-packages/\_soundfile.py", line 2, in <module>
import \_cffi_backend

FROM arm32v7/python:3.9.18
ENV LLVM_CONFIG=/usr/bin/llvm-config-14
RUN apt update && apt upgrade -y && apt install -y inotify-tools ffmpeg cmake gdal-bin libopenblas-dev llvm-14 llvm-14-dev libhdf5-dev libhdf5-serial-dev libatlas-base-dev

COPY ./whl/numba-0.58.1-cp39-cp39-linux_armv7l.whl /usr/src/app/
COPY ./whl/tflite-2.10.0-py2.py3-none-any.whl /usr/src/app/
COPY ./requirements.txt .
RUN pip3 install -r requirements.txt --extra-index-url=https://www.piwheels.org/simple
COPY . .

CMD [ "python", "server.py" ]

appdirs==1.4.4
apprise==1.0.0
asgiref==3.5.2
attrs==22.1.0
audioread==2.1.9
bottle==0.12.25
birdnetlib==0.13.2
black==22.6.0
certifi==2022.6.15
cffi==1.15.1
charset-normalizer==2.1.0
click==8.1.3
colorama==0.4.4
darker==1.5.0
decorator==5.1.1
idna==3.3
importlib-metadata==4.12.0
iniconfig==1.1.1
joblib==1.1.0
librosa==0.9.2
llvmlite==0.41.1
mock==4.0.3
mypy-extensions==0.4.3
numba @ file:///usr/src/app/numba-0.58.1-cp39-cp39-linux_armv7l.whl
numpy==1.26.0
packaging==21.3
pathspec==0.9.0
platformdirs==2.5.2
pluggy==1.0.0
pooch==1.6.0
py==1.11.0
pycparser==2.21
pydub==0.25.1
pyparsing==3.0.9
pytest==7.1.2
python-dotenv==0.20.0
pytz==2022.2.1
requests==2.28.1
resampy==0.3.1

# scikit-learn==1.0.2

scipy==1.11.4
SoundFile==0.10.3.post1
sqlparse==0.4.2
tflite @ file:///usr/src/app/tflite-2.10.0-py2.py3-none-any.whl
threadpoolctl==3.1.0
toml==0.10.2
tomli==2.0.1
typed-ast==1.5.4
typing_extensions==4.3.0
urllib3==1.26.11
watchdog==2.1.9
zipp==3.8.1

Installing the libffi package manually. This builds:
Gives the runtime errors described here:
https://community.element14.com/challenges-projects/design-challenges/pi-fest/b/blog/posts/songspire---experimenting-with-birdnet

FROM arm32v7/python:3.9.18
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
COPY ./deb/libffi7_3.3-6_armhf.deb /usr/src/app/
RUN apt install /usr/src/app/libffi7_3.3-6_armhf.deb

COPY ./whl/numba-0.58.1-cp39-cp39-linux_armv7l.whl /usr/src/app/
COPY ./whl/tflite-2.10.0-py2.py3-none-any.whl /usr/src/app/
COPY ./requirements.txt .
RUN pip3 install -r requirements.txt --extra-index-url=https://www.piwheels.org/simple
COPY . .

CMD [ "python", "server.py" ]

appdirs==1.4.4
apprise==1.0.0
asgiref==3.5.2
attrs==22.1.0
audioread==2.1.9
bottle==0.12.25
birdnetlib==0.13.2
black==22.6.0
certifi==2022.6.15
cffi==1.15.1
charset-normalizer==2.1.0
click==8.1.3
colorama==0.4.4
darker==1.5.0
decorator==5.1.1
idna==3.3
importlib-metadata==4.12.0
iniconfig==1.1.1
joblib==1.1.0
librosa==0.9.2
llvmlite==0.41.1
mock==4.0.3
mypy-extensions==0.4.3
numba @ file:///usr/src/app/numba-0.58.1-cp39-cp39-linux_armv7l.whl
numpy==1.26.0
packaging==21.3
pathspec==0.9.0
platformdirs==2.5.2
pluggy==1.0.0
pooch==1.6.0
py==1.11.0
pycparser==2.21
pydub==0.25.1
pyparsing==3.0.9
pytest==7.1.2
python-dotenv==0.20.0
pytz==2022.2.1
requests==2.28.1
resampy==0.3.1

# scikit-learn==1.0.2

scipy==1.11.4
SoundFile==0.10.3.post1
sqlparse==0.4.2
tflite @ file:///usr/src/app/tflite-2.10.0-py2.py3-none-any.whl
threadpoolctl==3.1.0
toml==0.10.2
tomli==2.0.1
typed-ast==1.5.4
typing_extensions==4.3.0
urllib3==1.26.11
watchdog==2.1.9
zipp==3.8.1

This thread seems to describe my issue: https://forums.balena.io/t/raspberry-pi-4-64-bit-with-balena-base-images-exitcode-159/63953/10
This also describes my issue: https://github.com/Ylianst/MeshCentral/issues/5108

Dette er status:
OS:
~/budorBeach $ hostnamectl
Static hostname: raspberrypi
Icon name: computer
Machine ID: 408fb5abbc764fb6a17f0a596296436f
Boot ID: 03c30b53629044b68a1e7d8f177a0b76
Operating System: Raspbian GNU/Linux 11 (bullseye)
Kernel: Linux 6.1.21-v8+
Architecture: arm64

Kernel: 64bit
User space: 32 bit

For Monday: Reinstall Docker. Follow this guide. Ensuring the 64 bit version is installed.
Info on multi-arch configuration in Debian: https://wiki.debian.org/Multiarch/HOWTO