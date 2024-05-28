import cv2
import numpy as np

# Callback function for trackbars


def update_min_contour_area(x):
    global min_contour_area
    min_contour_area = x
    # Call detect_blue_dice function to update the count and display the result
    detect_blue_dice(image_path)


def detect_blue_dice(image_path):
    # Load the image
    image = cv2.imread(image_path)
    image_resized = cv2.resize(image, (0, 0), fx=0.5, fy=0.5)

    # Convert image to HSV color space
    hsv = cv2.cvtColor(image_resized, cv2.COLOR_BGR2HSV)

    # Define range of blue color in HSV
    lower_blue = (66, 56, 36)
    upper_blue = (156, 193, 184)

    # Threshold the HSV image to get only blue colors
    mask_blue = cv2.inRange(hsv, lower_blue, upper_blue)

    # Find contours in the masked image
    contours, _ = cv2.findContours(
        mask_blue, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    # Loop over the contours
    for contour in contours:
        # Get the area of each contour
        area = cv2.contourArea(contour)

        # If the contour area is large enough, it's likely a dice
        if area > min_contour_area:
            # Extract the region of interest (ROI) which contains the dice
            x, y, w, h = cv2.boundingRect(contour)
            roi_blue = image_resized[y:y+h, x:x+w]

            # Convert the ROI to grayscale
            gray = cv2.cvtColor(roi_blue, cv2.COLOR_BGR2GRAY)

            # Apply a threshold to detect white dots
            _, mask_white = cv2.threshold(gray, 200, 255, cv2.THRESH_BINARY)

            # Find contours in the masked image
            contours_white, _ = cv2.findContours(
                mask_white, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

            # Count the number of white dots
            dot_count = len(contours_white)

            # Draw a bounding box around the dice
            cv2.rectangle(image_resized, (x, y),
                          (x + w, y + h), (0, 255, 0), 2)

            # Display the number of white dots
            cv2.putText(image_resized, f"Dot count: {dot_count}", (
                x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)

    # Display the image with bounding boxes
    cv2.imshow('Blue Dice Detection', image_resized)
    cv2.waitKey(1)


# Initialize minimum contour area
min_contour_area = 100

# Load the image
image_path = 'dice2.jpg'

# Create a window
cv2.namedWindow('Blue Dice Detection')

# Create a trackbar for adjusting the minimum contour area
cv2.createTrackbar('Min Contour Area', 'Blue Dice Detection',
                   min_contour_area, 5000, update_min_contour_area)

# Call the function initially
detect_blue_dice(image_path)

# Wait for a key press and close the window when any key is pressed
cv2.waitKey(0)
cv2.destroyAllWindows()
