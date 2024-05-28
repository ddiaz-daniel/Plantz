import cv2
import numpy as np


def select_area(event, x, y, flags, param):
    global selected_area, selecting

    if event == cv2.EVENT_LBUTTONDOWN:
        selected_area = [(x, y)]
        selecting = True
    elif event == cv2.EVENT_LBUTTONUP:
        selected_area.append((x, y))
        selecting = False


# Read the image
image = cv2.imread('dice2.jpg')

# Resize the image by half for easier selection
resized_image = cv2.resize(image, (0, 0), fx=0.5, fy=0.5)

# Display the image and allow area selection
cv2.imshow('Select Dice Area', resized_image)
cv2.setMouseCallback('Select Dice Area', select_area)
cv2.waitKey(0)
cv2.destroyAllWindows()

# Convert the selected point to numpy array
selected_point = np.array(selected_area[0])

# Define the size of the area around the selected point
area_size = 50  # You can adjust this size as needed

# Extract the region of interest from the original image
roi = image[selected_point[1]-area_size:selected_point[1]+area_size,
            selected_point[0]-area_size:selected_point[0]+area_size]

# Convert the ROI to HSV color space
hsv_roi = cv2.cvtColor(roi, cv2.COLOR_BGR2HSV)

# Calculate the mean HSV values for the ROI
h_mean, s_mean, v_mean, _ = cv2.mean(hsv_roi)
dice_color = (h_mean, s_mean, v_mean)

# Print the HSV values for the selected area
print("HSV values for selected area:", dice_color)
