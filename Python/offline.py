import cv2
import numpy as np


# Color ranges for different dice and dot colors using double mask
color_ranges = {
    "yellow": {"ranges": [(0, 127, 111), (34, 255, 255)], "color": (0, 255, 255)},
    "red": {"ranges": [(0, 70, 50), (10, 255, 255), (170, 70, 50), (180, 255, 255)], "color": (0, 0, 255)},
    "pink": {"ranges": [(166, 98, 126), (255, 188, 255)], "color": (203, 192, 255)},
    "green": {"ranges": [(24, 34, 0), (66, 255, 255)], "color": (0, 255, 0)},
    "skyBlue": {"ranges": [(77, 123, 85), (104, 255, 255)], "color": (255, 255, 0)},
    "darkBlue": {"ranges": [(84, 72, 53), (124, 255, 121)], "color": (255, 0, 0)},
    "darkGreen": {"ranges": [(61, 46, 52), (91, 255, 146)], "color": (0, 128, 0)},
}


dot_color_ranges = {
    "yellow": (0, 0, 0, 180, 255, 30),  # Black dots
    "red": (0, 0, 170, 179, 80, 255),  # White dots
    "pink": (0, 0, 170, 179, 80, 255),  # White dots
    "green": (0, 0, 0, 180, 255, 30),  # Black dots
    "skyBlue": (0, 0, 170, 179, 80, 255),  # White dots
    "darkBlue": (0, 0, 170, 179, 100, 255)  # White dots
}

# Global variables for trackbars
binary_threshold = 200
min_contour_area = 50
max_contour_area = 500


def detect_color(frame, ranges):
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    mask = cv2.inRange(hsv, ranges[0], ranges[1])
    if len(ranges) > 2:
        mask2 = cv2.inRange(hsv, ranges[2], ranges[3])
        mask = cv2.bitwise_or(mask, mask2)
    return mask


def find_dice_contour(mask):
    contours, _ = cv2.findContours(
        mask, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    if contours:
        contours = sorted(contours, key=cv2.contourArea, reverse=True)
        return contours[0]
    return None


def get_squares_image(frame, contour):
    rect = cv2.minAreaRect(contour)
    box = cv2.boxPoints(rect)
    box = np.int0(box)

    width = int(rect[1][0])
    height = int(rect[1][1])

    src_pts = box.astype("float32")
    dst_pts = np.array([[0, height-1],
                        [0, 0],
                        [width-1, 0],
                        [width-1, height-1]], dtype="float32")

    M = cv2.getPerspectiveTransform(src_pts, dst_pts)
    warped = cv2.warpPerspective(frame, M, (width, height))

    if height > width:
        warped = cv2.rotate(warped, cv2.ROTATE_90_CLOCKWISE)

    return warped


def count_dots(dice_img, lower_dot, upper_dot, binary_threshold, min_contour_area, max_contour_area, dot_color='white'):
    hsv = cv2.cvtColor(dice_img, cv2.COLOR_BGR2HSV)
    dot_mask = cv2.inRange(hsv, lower_dot, upper_dot)

    if dot_color == 'black':
        gray = cv2.bitwise_not(cv2.bitwise_and(cv2.cvtColor(
            dice_img, cv2.COLOR_BGR2GRAY), cv2.cvtColor(dice_img, cv2.COLOR_BGR2GRAY), mask=dot_mask))
    else:
        gray = cv2.bitwise_and(cv2.cvtColor(dice_img, cv2.COLOR_BGR2GRAY), cv2.cvtColor(
            dice_img, cv2.COLOR_BGR2GRAY), mask=dot_mask)

    _, binary = cv2.threshold(gray, binary_threshold, 240, cv2.THRESH_BINARY)
    contours, _ = cv2.findContours(
        binary, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

    # Filter contours based on the specified area range
    dots = [cnt for cnt in contours if min_contour_area <
            cv2.contourArea(cnt) < max_contour_area]
    return len(dots), binary


def on_binary_trackbar(val):
    global binary_threshold
    binary_threshold = val


def on_min_area_trackbar(val):
    global min_contour_area
    min_contour_area = val


def on_max_area_trackbar(val):
    global max_contour_area
    max_contour_area = val


def main():
    global binary_threshold, min_contour_area, max_contour_area

    cap = cv2.VideoCapture(0)
    cv2.namedWindow('Video')
    cv2.createTrackbar('Binary Threshold', 'Video',
                       binary_threshold, 255, on_binary_trackbar)
    cv2.createTrackbar('Min Contour Area', 'Video',
                       min_contour_area, 50, on_min_area_trackbar)
    cv2.createTrackbar('Max Contour Area', 'Video',
                       max_contour_area, 50, on_max_area_trackbar)

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        for color, data in color_ranges.items():
            ranges = data['ranges']
            color_value = data['color']
            lower_dice_1 = np.array(ranges[0])
            upper_dice_1 = np.array(ranges[1])
            if len(ranges) > 2:
                lower_dice_2 = np.array(ranges[2])
                upper_dice_2 = np.array(ranges[3])
            else:
                lower_dice_2 = None
                upper_dice_2 = None

            lower_dot = np.array(dot_color_ranges[color][:3])
            upper_dot = np.array(dot_color_ranges[color][3:])
            dot_color = 'black' if color in ['green', 'yellow'] else 'white'

            mask = detect_color(frame, ranges)
            dice_contour = find_dice_contour(mask)

            if dice_contour is not None:
                squared_dice_img = get_squares_image(frame, dice_contour)
                num_dots, binary = count_dots(
                    squared_dice_img, lower_dot, upper_dot, binary_threshold, min_contour_area, max_contour_area, dot_color)

                # Annotate the image
                cv2.putText(frame, f'{color.capitalize()} Dice: Dots: {num_dots}', (10, 30 + 30 * list(
                    color_ranges.keys()).index(color)), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 0, 0), 2)
                cv2.drawContours(frame, [cv2.boxPoints(cv2.minAreaRect(dice_contour)).astype(
                    int)], 0, color_ranges[color]['color'], 2)  # Use color_ranges[color]['color'] for contour color
                cv2.imshow(f'{color.capitalize()} Squared Dice',
                           squared_dice_img)
                cv2.imshow(f'{color.capitalize()} Binary', binary)

        cv2.imshow('Video', frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()
