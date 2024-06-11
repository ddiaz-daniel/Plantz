import cv2
import numpy as np

# Global variables for trackbars
green_lower_h = 31
green_lower_s = 113
green_lower_v = 52
green_upper_h = 85
green_upper_s = 255
green_upper_v = 255


def on_green_lower_h(val):
    global green_lower_h
    green_lower_h = val


def on_green_lower_s(val):
    global green_lower_s
    green_lower_s = val


def on_green_lower_v(val):
    global green_lower_v
    green_lower_v = val


def on_green_upper_h(val):
    global green_upper_h
    green_upper_h = val


def on_green_upper_s(val):
    global green_upper_s
    green_upper_s = val


def on_green_upper_v(val):
    global green_upper_v
    green_upper_v = val


def main():
    cap = cv2.VideoCapture(0)
    cv2.namedWindow('Video')

    # Create trackbars for adjusting green color ranges
    cv2.createTrackbar('Lower H', 'Video', green_lower_h,
                       179, on_green_lower_h)
    cv2.createTrackbar('Lower S', 'Video', green_lower_s,
                       255, on_green_lower_s)
    cv2.createTrackbar('Lower V', 'Video', green_lower_v,
                       255, on_green_lower_v)
    cv2.createTrackbar('Upper H', 'Video', green_upper_h,
                       179, on_green_upper_h)
    cv2.createTrackbar('Upper S', 'Video', green_upper_s,
                       255, on_green_upper_s)
    cv2.createTrackbar('Upper V', 'Video', green_upper_v,
                       255, on_green_upper_v)

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        # Get current trackbar positions
        lower_h = cv2.getTrackbarPos('Lower H', 'Video')
        lower_s = cv2.getTrackbarPos('Lower S', 'Video')
        lower_v = cv2.getTrackbarPos('Lower V', 'Video')
        upper_h = cv2.getTrackbarPos('Upper H', 'Video')
        upper_s = cv2.getTrackbarPos('Upper S', 'Video')
        upper_v = cv2.getTrackbarPos('Upper V', 'Video')

        # Define lower and upper HSV range for green
        lower_green = np.array([lower_h, lower_s, lower_v])
        upper_green = np.array([upper_h, upper_s, upper_v])

        # Convert BGR to HSV
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        # Threshold the HSV image to get only green colors
        mask = cv2.inRange(hsv, lower_green, upper_green)

        # Bitwise-AND mask and original image
        res = cv2.bitwise_and(frame, frame, mask=mask)

        cv2.imshow('Video', frame)
        cv2.imshow('Mask', mask)
        cv2.imshow('Result', res)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()
