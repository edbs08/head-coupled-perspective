import cv2
import numpy as np
import socket
import time
import math

UDP_IP = "127.0.0.1"
UDP_PORT = 5065

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Tracking
#import dlib
import sys


#Cascade filters
face_cascade = cv2.CascadeClassifier('./data/haarcascade_frontalface_alt.xml')
eye_cascade = cv2.CascadeClassifier('./data/haarcascade_eye.xml')

#Start capturing from webcam
cap = cv2.VideoCapture(0)
 
# Create MultiTracker object
multiTracker = cv2.MultiTracker_create()
tracker = cv2.TrackerMOSSE_create()
#tracker_face = dlib.correlation_tracker()


#The variable we use to keep track of the fact whether we are
#currently using the dlib tracker
trackingFace = 0
detectingFace = 0
x = 0
y = 0
w = 0
h = 0

scaling_factor = 0.5
right = 0
left = 0
up = 0
down = 0

old_x = 0
old_y = 0
x_pos = 60
y_pos = 60

delta = 2

while True:
    ret, frame = cap.read()
    frame = cv2.resize(frame, None, fx=scaling_factor, fy=scaling_factor, interpolation=cv2.INTER_AREA)
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    t_x = 0;
    t_y = 0;
    if trackingFace == 0:
        # Tracking face
        face_rects = face_cascade.detectMultiScale(gray, 1.3, 5)

        #Tracking eyes
        eyes_rects = eye_cascade.detectMultiScale(gray, 1.3, 5)

        maxArea = 0;
        for (_x,_y,_w,_h) in face_rects:
            if  _w*_h > maxArea:
                x = _x
                y = _y
                w = _w
                h = _h
                maxArea = w*h
                detectingFace = 1

        if detectingFace == 1:          
            #Initialize the tracker with the biggest face (we assume is the correct one and the closest one)
            multiTracker.add(tracker, frame, (x, y, w, h))
            #tracker_face.start_track(frame, dlib.rectangle(x, y, x+w, y+h))

            #Set the indicator variable such that we know the
            #tracker_face is tracking a region in the image
            trackingFace = 1

    else:
        # get updated location of objects in subsequent frames
        success, boxes = multiTracker.update(frame)

        #If the tracking quality is good enough, determine the
        #updated position of the tracked region and draw the
        #rectangle
        if not success:
            trackingFace = 0
            detectingFace = 0
        
        else:
            # draw tracked objects
            t_x = int(boxes[0][0])
            t_y = int(boxes[0][1])
            t_w = int(boxes[0][2])
            t_h = int(boxes[0][3])
            
            cv2.rectangle(frame, (t_x, t_y), (t_x + t_w , t_y + t_h), (0,255,0) ,2)
            #cv2.rectangle(frame, p1, p2, (0,255,0), 2)

            #cv2.rectangle(frame, (x,y), (x+w,y+h), (0,255,0), 3)

            #a = old_face_rects[0].x
            #b = face_rects[0].x
            if old_x < t_x-delta:
                right = 1
            elif old_x > t_x+delta:
                right = -1
            else:
                right = 0

            if old_y < t_y-delta:
                down = 1
            elif old_y > t_y+delta:
                down = -1
            else:
                down = 0

            x_pos = x_pos+2*right
            y_pos = y_pos+2*down
            cv2.rectangle(frame, (x_pos,y_pos), (x_pos+20,y_pos+20), (0,0,255), 3)
            
            old_x = t_x
            old_y = t_y

    cv2.imshow('Face Detector', frame)

    c = cv2.waitKey(1)
    if c == 27:
        break
    
    sock.sendto( format(t_x).encode('utf-8'), (UDP_IP, UDP_PORT) )
    print("_"*10, "Sent x ",t_x, "_"*10)
       
    sock.sendto( format(t_y).encode('utf-8'), (UDP_IP, UDP_PORT) )
    print("_"*10, "Sent y ",t_y, "_"*10)
cap.release()
cv2.destroyAllWindows()