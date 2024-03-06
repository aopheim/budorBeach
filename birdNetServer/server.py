import os
import bottle
import json
from birdnetlib import Recording
from birdnetlib.analyzer import Analyzer
from datetime import date


HOST='0.0.0.0'
PORT=8080

@bottle.route("/healthcheck", method="GET")
def healthcheck():
    """Checks the health of the running server.
    Returns:
        A json message.
    """
    return json.dumps({"msg": "Server is healthy."})


@bottle.route("/analyze", method="POST")
def handleRequest():
    # Get request payload
    # Analyze file
    try:
        mdata = json.loads(bottle.request.forms.get("meta", {}))
        filePath = mdata["FilePath"]
        print(f"Received analyze request:\n{mdata}")
        analyzer = Analyzer()
        recording = Recording(
            analyzer,
            path=filePath,
            lat=mdata["Lat"],
            lon=mdata["Lon"],
            date = date.fromisocalendar(date.today().year, mdata["Week"], 1),
            min_conf=0.25
        )
        print('Sending recording to analyzer')
        recording.analyze()
        
        # Parse results
        if any(recording.detections):
            
            # Prepare response
            data = {"msg": "success", "results": recording.detections }

            return json.dumps(data)

        else:
            return json.dumps({"msg": "Error during analysis."})

    except Exception as e:
        # Write error log
        print(f"Error: Cannot analyze file {filePath}. \n{e}", flush=True)

        data = {"msg": f"Error during analysis: {e}"}

        return json.dumps(data)
    


if __name__ == "__main__":
    # Run server
    print(f"UP AND RUNNING! LISTENING ON {HOST}:{PORT}", flush=True)

    bottle.run(host=HOST, port=PORT, quiet=True)
    
