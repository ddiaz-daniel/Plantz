import cv2
import numpy as np
import requests


def detect_all_dice(frame, color_ranges):
    # Convert frame to HSV color space
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # Loop through all color ranges
    for color_name, color_range in color_ranges.items():
        # Threshold the HSV image based on current color for dice
        lower_dice = np.array(color_range["lower_dice"])
        upper_dice = np.array(color_range["upper_dice"])
        mask_dice = cv2.inRange(hsv, lower_dice, upper_dice)

        # Find contours in the masked image for dice
        contours_dice, _ = cv2.findContours(
            mask_dice, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        # Loop over the dice contours
        for contour in contours_dice:
            # Get the area of each contour
            area = cv2.contourArea(contour)

            # If the contour area is within a certain range, it's likely a dice
            if 400 < area < 4000:
                # Extract the region of interest (ROI) which contains the dice
                x, y, w, h = cv2.boundingRect(contour)

                # Draw a bounding box around the dice
                cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

                # Extract region of interest for dot detection
                roi = hsv[y:y + h, x:x + w]

                # Threshold the ROI for dot color
                lower_dot = np.array(color_range["lower_dot"])
                upper_dot = np.array(color_range["upper_dot"])
                mask_dot = cv2.inRange(roi, lower_dot, upper_dot)

                # Find contours in the masked image for dots
                contours_dot, _ = cv2.findContours(
                    mask_dot, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

                # Filter out small dots and draw larger ones
                for dot_contour in contours_dot:
                    area_dots = cv2.contourArea(dot_contour)
                    if 5 < area_dots < 200:
                        # Get the bounding box of each dot contour
                        dot_x, dot_y, dot_w, dot_h = cv2.boundingRect(
                            dot_contour)

                        # Draw a red dot covering the entire area of each detected dot
                        cv2.rectangle(frame, (x + dot_x, y + dot_y),
                                      (x + dot_x + dot_w, y + dot_y + dot_h), (0, 0, 255), 2)

                # Print the number of dots and the color of the dice
                cv2.putText(frame, f"{len(contours_dot)}", (x, y - 10),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)

    # Display the frame with bounding boxes and dots for all colors
    cv2.imshow('Dice Detection', frame)
    cv2.waitKey(1)  # Adjust the delay time according to your preference


# Define color ranges for dice and dots for each color
color_ranges = {
    "blue": {"lower_dice": (77, 123, 85), "upper_dice": (104, 255, 255),
             "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
    "darkBlue": {"lower_dice": (106, 180, 42), "upper_dice": (117, 255, 121),
                 "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
    "yellow": {"lower_dice": (0, 127, 111), "upper_dice": (34, 255, 255),
               "lower_dot": (0, 0, 0), "upper_dot": (180, 255, 30)},
    "pink": {"lower_dice": (166, 98, 126), "upper_dice": (255, 188, 255),
             "lower_dot": (0, 127, 111), "upper_dot":  (34, 255, 255)},
    "green": {"lower_dice": (31, 113, 52), "upper_dice": (58, 255, 255),
              "lower_dot": (0, 0, 0), "upper_dot": (180, 255, 30)},
    "darkGreen": {"lower_dice": (44, 113, 52), "upper_dice": (85, 255, 146),
                  "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
    "red": {"lower_dice": (0, 59, 0), "upper_dice": (30, 255, 144),
            "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
    "black": {"lower_dice": (0, 0, 0), "upper_dice": (180, 255, 30),
              "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
}
video_url = 'http://192.168.137.4:8080/shot.jpg'

while True:
    # Capture frame-by-frame
    img_resp = requests.get(video_url)
    img_arr = np.array(bytearray(img_resp.content), dtype=np.uint8)
    frame = cv2.imdecode(img_arr, -1)

    # Check if the frame is valid
    if not frame is None:
        # Detect all dice in the frame
        detect_all_dice(frame, color_ranges)
    else:
        print('Error loading the frame')

    # Exit if 'q' is pressed
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break


# Release the video stream
cv2.destroyAllWindows()
