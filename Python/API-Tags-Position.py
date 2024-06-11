import cv2
import numpy as np
from fastapi import FastAPI, Response
from fastapi.responses import StreamingResponse
import uvicorn
import threading
import json

app = FastAPI()

desired_aruco_dictionary = "DICT_4X4_50"
ARUCO_DICT = {
    "DICT_4X4_50": cv2.aruco.DICT_4X4_50,
    "DICT_4X4_100": cv2.aruco.DICT_4X4_100,
    "DICT_4X4_250": cv2.aruco.DICT_4X4_250,
    "DICT_4X4_1000": cv2.aruco.DICT_4X4_1000,
    "DICT_5X5_50": cv2.aruco.DICT_5X5_50,
    "DICT_5X5_100": cv2.aruco.DICT_5X5_100,
    "DICT_5X5_250": cv2.aruco.DICT_5X5_250,
    "DICT_5X5_1000": cv2.aruco.DICT_5X5_1000,
    "DICT_6X6_50": cv2.aruco.DICT_6X6_50,
    "DICT_6X6_100": cv2.aruco.DICT_6X6_100,
    "DICT_6X6_250": cv2.aruco.DICT_6X6_250,
    "DICT_6X6_1000": cv2.aruco.DICT_6X6_1000,
    "DICT_7X7_50": cv2.aruco.DICT_7X7_50,
    "DICT_7X7_100": cv2.aruco.DICT_7X7_100,
    "DICT_7X7_250": cv2.aruco.DICT_7X7_250,
    "DICT_7X7_1000": cv2.aruco.DICT_7X7_1000,
    "DICT_ARUCO_ORIGINAL": cv2.aruco.DICT_ARUCO_ORIGINAL
}

# Define HSV ranges for different dice colors
color_ranges = {
    "red": ([0, 120, 70], [10, 255, 180]),
    "pink": ([145, 50, 160], [180, 160, 255]),
    "green": ([30, 23, 97], [57, 255, 255]),
    "blue": ([90, 80, 170], [140, 255, 255]),
    "darkBlue": ([100, 20, 90], [160, 255, 255]),
    "darkGreen": ([54, 24, 100], [85, 255, 255]),
    "yellow": ([10, 100, 100], [30, 255, 255]),
}

# Global variables to store data
detected_tags = []
detected_dice = {color: 0 for color in color_ranges}
dots_count = {color: 0 for color in color_ranges}

# Video capture object
cap = cv2.VideoCapture(0)


def detect_dice_and_aruco(frame):
    global detected_tags, detected_dice, dots_count
    detected_tags = []
    detected_dice = {color: 0 for color in color_ranges}
    dots_count = {color: 0 for color in color_ranges}

    # Convert BGR to HSV
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # Define lower and upper HSV range for contour detection
    lower_hsv = np.array([0, 54, 63])
    upper_hsv = np.array([179, 255, 255])

    # Threshold the HSV image to get only desired colors
    mask = cv2.inRange(hsv, lower_hsv, upper_hsv)

    # Find contours with hierarchy
    contours, _ = cv2.findContours(
        mask, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

    # Filter contours based on minimum area
    filtered_contours = [
        cnt for cnt in contours if cv2.contourArea(cnt) >= 500]

    result = frame.copy()
    for i, contour in enumerate(filtered_contours):
        cv2.drawContours(result, [contour], -1, (0, 255, 0), 2)

        contour_mask = np.zeros_like(mask)
        cv2.drawContours(
            contour_mask, [contour], -1, 255, thickness=cv2.FILLED)

        contour_region = cv2.bitwise_and(frame, frame, mask=contour_mask)
        contour_hsv = cv2.cvtColor(contour_region, cv2.COLOR_BGR2HSV)

        # Detect color of dice
        dice_color = None
        for color, (lower, upper) in color_ranges.items():
            lower = np.array(lower, dtype="uint8")
            upper = np.array(upper, dtype="uint8")
            color_mask = cv2.inRange(contour_hsv, lower, upper)
            if cv2.countNonZero(color_mask) > 0:
                dice_color = color
                detected_dice[color] += 1

                # Detect dots inside the dice
                dots_mask = cv2.inRange(contour_hsv, lower, upper)
                # invert the mask
                dots_mask = cv2.bitwise_not(dots_mask)
                dots_contours, _ = cv2.findContours(
                    dots_mask, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
                dots_count[dice_color] = len(
                    [cnt for cnt in dots_contours if cv2.contourArea(cnt) < 70 and cv2.contourArea(cnt) > 12])
                break

        M = cv2.moments(contour)
        if M["m00"] != 0:
            cx = int(M["m10"] / M["m00"])
            cy = int(M["m01"] / M["m00"])
            if dice_color:
                # write color and number of dots
                cv2.putText(result, f"{dice_color} {dots_count[dice_color]}", (cx, cy),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)

    # ArUco detection
    dictionary = cv2.aruco.getPredefinedDictionary(
        ARUCO_DICT[desired_aruco_dictionary])
    parameters = cv2.aruco.DetectorParameters()
    detector = cv2.aruco.ArucoDetector(dictionary, parameters)
    (corners, ids, rejected) = detector.detectMarkers(frame)

    if ids is not None:
        ids = ids.flatten()
        for (marker_corner, marker_id) in zip(corners, ids):
            corners = marker_corner.reshape((4, 2))
            (top_left, top_right, bottom_right, bottom_left) = corners

            top_right = (int(top_right[0]), int(top_right[1]))
            bottom_right = (int(bottom_right[0]), int(bottom_right[1]))
            bottom_left = (int(bottom_left[0]), int(bottom_left[1]))
            top_left = (int(top_left[0]), int(top_left[1]))

            cv2.line(result, top_left, top_right, (0, 255, 0), 2)
            cv2.line(result, top_right, bottom_right, (0, 255, 0), 2)
            cv2.line(result, bottom_right, bottom_left, (0, 255, 0), 2)
            cv2.line(result, bottom_left, top_left, (0, 255, 0), 2)

            cv2.putText(result, str(marker_id), (top_left[0], top_left[1] - 15),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)

            center_x = int((top_left[0] + bottom_right[0]) / 2.0)
            center_y = int((top_left[1] + bottom_right[1]) / 2.0)

            detected_tags.append({
                "id": int(marker_id),
                "position": {"x": center_x, "y": center_y}
            })

    return result


def generate_frames():
    while True:
        ret, frame = cap.read()
        if not ret:
            break
        frame = detect_dice_and_aruco(frame)
        ret, buffer = cv2.imencode('.jpg', frame)
        frame = buffer.tobytes()
        yield (b'--frame\r\n'
               b'Content-Type: image/jpeg\r\n\r\n' + frame + b'\r\n')


@app.get("/stream")
def video_feed():
    return StreamingResponse(generate_frames(), media_type="multipart/x-mixed-replace; boundary=frame")


@app.get("/data")
def get_data():
    return Response(content=json.dumps({"tags": detected_tags, "dice": detected_dice, "dots": dots_count}), media_type="application/json")


@app.get("/getPosition")
def get_position():
    return Response(content=json.dumps({"tags": detected_tags}), media_type="application/json")


@app.get("/getSeeds")
def get_seeds():
    return Response(content=json.dumps({"dice": detected_dice, "dots": dots_count}), media_type="application/json")


def start_fastapi():
    uvicorn.run(app, host="0.0.0.0", port=8000)


if __name__ == "__main__":
    fastapi_thread = threading.Thread(target=start_fastapi)
    fastapi_thread.start()

    while True:
        ret, frame = cap.read()
        if not ret:
            break
        cv2.imshow('Result', detect_dice_and_aruco(frame))
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    cap.release()
    cv2.destroyAllWindows()
