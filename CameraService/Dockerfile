FROM arm64v8/python:3.9.18
RUN apt update && apt upgrade -y
RUN pip install poetry==1.8
ENV POETRY_NO_INTERACTION=1 \
    POETRY_VIRTUALENVS_IN_PROJECT=1 \
    POETRY_VIRTUALENVS_CREATE=1 \
    POETRY_CACHE_DIR=/tmp/poetry_cache
WORKDIR /app
COPY pyproject.toml poetry.lock ./
RUN poetry install --no-root && rm -rf ${POETRY_CACHE_DIR}
COPY server.py ./
CMD [ "poetry", "run", "python", "server.py" ]