from flask import Flask, Response, jsonify
import cv2
import threading
import numpy as np

app = Flask(__name__)

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

dictionary = cv2.aruco.getPredefinedDictionary(
    ARUCO_DICT[desired_aruco_dictionary])
parameters = cv2.aruco.DetectorParameters()

detected_markers = []
detected_dice = []
frame_lock = threading.Lock()
current_frame = None
dice_min_area = 200
dice_max_area = 2000


def detect_all_dice(frame, color_ranges):
    """Detect dice and their numbers in the given frame."""
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    dice_info = []

    for color_name, color_range in color_ranges.items():
        lower_dice = np.array(color_range["lower_dice"])
        upper_dice = np.array(color_range["upper_dice"])
        mask_dice = cv2.inRange(hsv, lower_dice, upper_dice)

        contours_dice, _ = cv2.findContours(
            mask_dice, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        for contour in contours_dice:
            area = cv2.contourArea(contour)
            if dice_min_area < area < dice_max_area:
                x, y, w, h = cv2.boundingRect(contour)
                cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

                roi = hsv[y:y + h, x:x + w]
                lower_dot = np.array(color_range["lower_dot"])
                upper_dot = np.array(color_range["upper_dot"])
                mask_dot = cv2.inRange(roi, lower_dot, upper_dot)

                contours_dot, _ = cv2.findContours(
                    mask_dot, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

                dots_count = sum(
                    1 for dot_contour in contours_dot if 1 < cv2.contourArea(dot_contour) < 20)

                cv2.putText(frame, f"{dots_count}", (x, y - 10),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.5, color_range["upper_dot"], 2)

                dice_info.append({
                    "color": color_name,
                    "dots": dots_count,
                    "position": {"x": x, "y": y, "width": w, "height": h}
                })

    return dice_info


color_ranges = {
    "blue": {"lower_dice": (77, 123, 85), "upper_dice": (104, 255, 255),
             "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
    "darkBlue": {"lower_dice": (106, 180, 42), "upper_dice": (117, 255, 121),
                 "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
    "yellow": {"lower_dice": (0, 127, 111), "upper_dice": (34, 255, 255),
               "lower_dot": (0, 0, 0), "upper_dot": (180, 255, 30)},
    "pink": {"lower_dice": (166, 98, 126), "upper_dice": (255, 188, 255),
             "lower_dot": (0, 127, 111), "upper_dot": (34, 255, 255)},
    "green": {"lower_dice": (31, 113, 52), "upper_dice": (58, 255, 255),
              "lower_dot": (0, 0, 0), "upper_dot": (180, 255, 30)},
    "darkGreen": {"lower_dice": (44, 113, 52), "upper_dice": (85, 255, 146),
                  "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
    "red": {"lower_dice": (0, 59, 0), "upper_dice": (30, 255, 144),
            "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
    "black": {"lower_dice": (0, 0, 0), "upper_dice": (180, 255, 30),
              "lower_dot": (0, 0, 170), "upper_dot": (179, 80, 255)},
}


def detect_markers_and_dice():
    """Continuously capture frames and detect ArUco markers and dice."""
    global detected_markers, detected_dice, current_frame
    cap = cv2.VideoCapture(0)
    detector = cv2.aruco.ArucoDetector(dictionary, parameters)

    while True:
        success, frame = cap.read()
        if not success:
            continue

        (corners, ids, rejected) = detector.detectMarkers(frame)
        markers = []
        if ids is not None:
            ids = ids.flatten()
            for (marker_corner, marker_id) in zip(corners, ids):
                corners = marker_corner.reshape((4, 2))
                (top_left, top_right, bottom_right, bottom_left) = corners

                top_right = (int(top_right[0]), int(top_right[1]))
                bottom_right = (int(bottom_right[0]), int(bottom_right[1]))
                bottom_left = (int(bottom_left[0]), int(bottom_left[1]))
                top_left = (int(top_left[0]), int(top_left[1]))

                cv2.line(frame, top_left, top_right, (0, 255, 0), 2)
                cv2.line(frame, top_right, bottom_right, (0, 255, 0), 2)
                cv2.line(frame, bottom_right, bottom_left, (0, 255, 0), 2)
                cv2.line(frame, bottom_left, top_left, (0, 255, 0), 2)

                cv2.putText(frame, str(marker_id), (top_left[0], top_left[1] - 15),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)

                center_x = int((top_left[0] + bottom_right[0]) / 2.0)
                center_y = int((top_left[1] + bottom_right[1]) / 2.0)

                markers.append({
                    "id": int(marker_id),
                    "center": {"x": center_x, "y": center_y}
                })

        dice_info = detect_all_dice(frame, color_ranges)

        with frame_lock:
            detected_markers = markers
            detected_dice = dice_info
            current_frame = frame.copy()


t = threading.Thread(target=detect_markers_and_dice)
t.daemon = True
t.start()


@app.route('/video_feed')
def video_feed():
    def generate():
        while True:
            with frame_lock:
                if current_frame is None:
                    continue
                ret, buffer = cv2.imencode('.jpg', current_frame)
                frame = buffer.tobytes()
            yield (b'--frame\r\n'
                   b'Content-Type: image/jpeg\r\n\r\n' + frame + b'\r\n')
    return Response(generate(), mimetype='multipart/x-mixed-replace; boundary=frame')


@app.route('/get_markers_dice', methods=['GET'])
def get_markers_dice():
    with frame_lock:
        markers = detected_markers
        dice = detected_dice
    return jsonify({"markers": markers, "dice": dice})


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
