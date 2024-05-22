#!/bin/bash
cd ../../images
current_date=$(date +"%Y-%m-%d")
mkdir -p "${current_date}"
current_time=$(date +"%H-%M-%S")
filename="${current_date}/${current_time}.jpg"
libcamera-still -o "$filename"
