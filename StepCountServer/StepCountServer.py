from enum import IntEnum, auto
import threading
import time
import numpy as np
import cv2
from posenet import pose_engine
from flask import Flask, request, Markup

class StepCounter:
	class Status(IntEnum):
		LEFT_DOWN = 1
		RIGHT_DOWN = 2
	
	currentStatus = Status.LEFT_DOWN
	cnt_step = 0

	def __init__(self):
		pass

	def update(self, left_ankle, right_ankle):
		# print(left_ankle, right_ankle)
		MARGIN = 10
		if self.currentStatus == self.Status.LEFT_DOWN:
			if right_ankle > left_ankle + MARGIN:
				self.currentStatus = self.Status.RIGHT_DOWN
				self.cnt_step += 1
		if self.currentStatus == self.Status.RIGHT_DOWN:
			if left_ankle > right_ankle + MARGIN:
				self.currentStatus = self.Status.LEFT_DOWN
				self.cnt_step += 1
	def get_count(self):
		return self.cnt_step

step_counter = StepCounter()

def count_step():
	global step_counter

	# engine = PoseEngine("posenet/models/mobilenet/posenet_mobilenet_v1_075_721_1281_quant_decoder_edgetpu.tflite")
	engine = pose_engine.PoseEngine("posenet/models/mobilenet/posenet_mobilenet_v1_075_481_641_quant_decoder_edgetpu.tflite")
	cap = cv2.VideoCapture(0)
	cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
	cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

	while True:
		# capture image
		ret, img_org = cap.read()
		# img_org = cv2.resize(img_org, (641, 481))
		img = cv2.cvtColor(img_org, cv2.COLOR_BGR2RGB)
		img = img.astype(np.uint8)

		# run inference
		poses, inference_time = engine.DetectPosesInImage(img)

		# retrieve results
		for pose in poses:
			if pose.score >= 0.4:
				left_ankle = -1
				right_ankle = -1
				for label, keypoint in pose.keypoints.items():
					if keypoint.score > 0.1:
						cv2.circle(img_org, (keypoint.yx[1], keypoint.yx[0]), 5, (0, 255, 0), thickness=5)
						if label == "left ankle":
							left_ankle = keypoint.yx[0]
						if label == "right ankle":
							right_ankle = keypoint.yx[0]
				if left_ankle > 0 and right_ankle > 0:
					step_counter.update(left_ankle, right_ankle)
		cv2.putText(img_org, "Step=" + str(step_counter.get_count()), (0, 50), cv2.FONT_HERSHEY_PLAIN, 4, (0, 255, 0), 5, cv2.LINE_AA)
		cv2.imshow("image", img_org)
		key = cv2.waitKey(1)
		if key == 27: # ESC
			break

	cap.release()
	cv2.destroyAllWindows()


app = Flask(__name__)
@app.route('/')
def index():
	return "hello"

@app.route('/get_count', methods=['GET'])
def test():
	global step_counter
	return str(step_counter.get_count())

def server():
	app.run(debug=False)


if __name__ == "__main__":
	t1 = threading.Thread(target=count_step)
	t2 = threading.Thread(target=server)
	t2.setDaemon(True)
	t1.start()
	t2.start()
	t1.join()
	# t2.join()
