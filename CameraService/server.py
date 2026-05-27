import os
import bottle
import json
import logging
from pathlib import Path
from time import sleep
from picamera2 import Picamera2
from picamera2.encoders import JpegEncoder
from libcamera import controls

HOST = '0.0.0.0'
PORT = 8080

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# Global camera instance
camera = None

def init_camera():
    """Initialize the camera once at startup."""
    global camera
    try:
        if camera is None:
            logger.info("Initializing Picamera2...")
            camera = Picamera2()
            logger.info("Camera initialized successfully")
        return True
    except Exception as e:
        logger.error(f"Failed to initialize camera: {e}")
        return False

@bottle.route("/healthcheck", method="GET")
def healthcheck():
    """Checks the health of the running server."""
    try:
        if camera is None:
            return json.dumps({
                "status": "warning",
                "msg": "Server is running but camera not yet initialized"
            }), 200
        return json.dumps({
            "status": "ok",
            "msg": "Server is healthy and camera is ready"
        }), 200
    except Exception as e:
        logger.error(f"Health check failed: {e}")
        return json.dumps({
            "status": "error",
            "msg": str(e)
        }), 500

@bottle.route("/takeimage", method="POST")
def take_image():
    """Take an image with the camera.
    
    Expected JSON body:
    {
        "filename": "/path/to/output.jpg",
        "iso": 800,
        "shutter_speed": 0
    }
    """
    try:
        body = bottle.request.json or {}
        filename = body.get('filename', '/tmp/image.jpg')
        iso = body.get('iso', 800)
        shutter_speed = body.get('shutter_speed', 0)
        
        logger.info(f"Taking image: {filename} (ISO: {iso}, Shutter: {shutter_speed})")
        
        # Ensure directory exists
        output_path = Path(filename)
        output_path.parent.mkdir(parents=True, exist_ok=True)
        
        if camera is None:
            raise RuntimeError("Camera not initialized")
        
        # Configure camera settings
        camera_config = camera.create_still_configuration(main={"size": (3280, 2464)})
        camera.configure(camera_config)
        
        # Set camera controls
        camera.set_controls({
            controls.AnalogueGain: iso / 100.0,  # Convert ISO to analogue gain
            controls.ExposureTime: shutter_speed,
        })
        
        # Start camera and capture
        camera.start()
        sleep(0.5)  # Camera warm-up
        
        # Capture and save
        camera.capture_file(filename, name="main")
        camera.stop()
        
        logger.info(f"Image saved successfully: {filename}")
        return json.dumps({
            "status": "success",
            "filename": filename,
            "size": os.path.getsize(filename) if os.path.exists(filename) else 0
        }), 200
    
    except Exception as e:
        logger.error(f"Error taking image: {e}", exc_info=True)
        return json.dumps({
            "status": "error",
            "msg": str(e)
        }), 500

@bottle.error(500)
def error500(err):
    """Handle 500 errors."""
    logger.error(f"Server error: {err.body}")
    return json.dumps({
        "status": "error",
        "msg": "Internal server error"
    })

if __name__ == "__main__":
    logger.info("Starting Camera Server...")
    
    # Initialize camera at startup
    if not init_camera():
        logger.warning("Camera initialization failed, but server will continue. "
                      "Camera will be initialized on first image request.")
    
    logger.info(f"Camera Server listening on {HOST}:{PORT}")
    bottle.run(host=HOST, port=PORT, debug=False, quiet=False)
    
