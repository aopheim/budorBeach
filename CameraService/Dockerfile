FROM balenalib/raspberrypi4-64

# Install required system packages
RUN install_packages \
    libcamera-dev \
    libcamera-apps \
    libcamera0 \
    libatlas-base-dev \
    libjasper-dev \
    libtiff5 \
    libharfftv0 \
    libwebp6 \
    libjasper1 \
    libjpeg62-turbo \
    zlib1g \
    python3 \
    v4l-utils

# Install Python dependencies
RUN pip install poetry==1.8

ENV POETRY_NO_INTERACTION=1 \
    POETRY_VIRTUALENVS_IN_PROJECT=1 \
    POETRY_VIRTUALENVS_CREATE=1 \
    POETRY_CACHE_DIR=/tmp/poetry_cache

WORKDIR /app
COPY pyproject.toml poetry.lock ./
RUN poetry install --no-root && rm -rf ${POETRY_CACHE_DIR}
COPY server.py ./

# Expose port
EXPOSE 8080

# Run the server
CMD [ "poetry", "run", "python", "server.py" ]