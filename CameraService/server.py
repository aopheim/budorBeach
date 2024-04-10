import os
import bottle
import json
from picamera import Picamera
from time import sleep


HOST='0.0.0.0'
PORT=8080

@bottle.route("/healthcheck", method="GET")
def healthcheck():
    """Checks the health of the running server.
    Returns:
        A json message.
    """
    return json.dumps({"msg": "Server is healthy."})


@bottle.route("/takeimage", method="POST")
def handleRequest():
    try:
        print("Hello from server.py!")
        filename = "test.jpg"
        camera = Picamera()
        sleep(2)
        camera.capture(filename)

        return json.dumps(data)

    except Exception as e:
        print("Error taking image")
        print(e)
        data = {"filename": filename}
        return json.dumps(data)
    


if __name__ == "__main__":
    # Run server
    print(f"CAMERA SERVER UP AND RUNNING! LISTENING ON {HOST}:{PORT}", flush=True)

    bottle.run(host=HOST, port=PORT, quiet=True)
    
