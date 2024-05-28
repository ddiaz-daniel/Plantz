import cv2
import numpy as np
import requests


def apply_color_threshold(image, lower_threshold, upper_threshold):
    # Convert the image from BGR to HSV color space
    hsv_image = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)

    # Define the lower and upper bounds of the threshold
    lower_bound = np.array(lower_threshold)
    upper_bound = np.array(upper_threshold)

    # Create a mask using the threshold values
    mask = cv2.inRange(hsv_image, lower_bound, upper_bound)

    # Apply the mask to the original image
    result = cv2.bitwise_and(image, image, mask=mask)

    return result


def on_trackbar_change(frame):
    # Get current positions of all trackbars
    lower_hue = cv2.getTrackbarPos('Lower Hue', 'Threshold Settings')
    upper_hue = cv2.getTrackbarPos('Upper Hue', 'Threshold Settings')
    lower_saturation = cv2.getTrackbarPos(
        'Lower Saturation', 'Threshold Settings')
    upper_saturation = cv2.getTrackbarPos(
        'Upper Saturation', 'Threshold Settings')
    lower_value = cv2.getTrackbarPos('Lower Value', 'Threshold Settings')
    upper_value = cv2.getTrackbarPos('Upper Value', 'Threshold Settings')

    lower_threshold = [lower_hue, lower_saturation, lower_value]
    upper_threshold = [upper_hue, upper_saturation, upper_value]

    # Apply color thresholding
    result = apply_color_threshold(frame, lower_threshold, upper_threshold)

    # Display the result
    cv2.imshow('Color Thresholding', result)


# Create a window for trackbars
cv2.namedWindow('Threshold Settings')

# Create trackbars for color thresholding parameters
cv2.createTrackbar('Lower Hue', 'Threshold Settings',
                   0, 179, on_trackbar_change)
cv2.createTrackbar('Upper Hue', 'Threshold Settings',
                   179, 179, on_trackbar_change)
cv2.createTrackbar('Lower Saturation', 'Threshold Settings',
                   0, 255, on_trackbar_change)
cv2.createTrackbar('Upper Saturation', 'Threshold Settings',
                   255, 255, on_trackbar_change)
cv2.createTrackbar('Lower Value', 'Threshold Settings',
                   0, 255, on_trackbar_change)
cv2.createTrackbar('Upper Value', 'Threshold Settings',
                   255, 255, on_trackbar_change)

# Initialize with default values


video_url = 'http://192.168.137.4:8080/shot.jpg'

while True:
    # Capture frame-by-frame
    img_resp = requests.get(video_url)
    img_arr = np.array(bytearray(img_resp.content), dtype=np.uint8)
    frame = cv2.imdecode(img_arr, -1)

    # Check if the frame is valid
    if not frame is None:
        # Detect all dice in the frame
        on_trackbar_change(frame)
    else:
        print('Error loading the frame')

    # Exit if 'q' is pressed
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Wait for ESC key to exit
while True:
    if cv2.waitKey(1) & 0xFF == 27:
        break

cv2.destroyAllWindows()
