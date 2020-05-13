# VR Hiking Application
- This project consits of two parts
	- Server to count steps
		- Capture camera image
		- Detect ankles of body using Pose Net
		- Count steps
		- Serve the number of steps via http using Flask
	- Unity application
		- Get the number of steps by accessing the server
		- Display hiking view

## Environment
- Windows 10
- Unity 2018.4.22f1 Personal
- Oculus Rift S
- Edge TPU
- USB Camera

## Server side
```
conda create -n py37_edgetpu_pose_cnt  python=3.7
conda activate py37_edgetpu_pose_cnt
pip install https://dl.google.com/coral/python/tflite_runtime-2.1.0-cp37-cp37m-win_amd64.whl
pip install https://dl.google.com/coral/edgetpu_api/edgetpu-2.13.0-cp37-cp37m-win_amd64.whl
pip install opencv-python
pip install flask

cd VR_hiking\StepCountServer
python StepCountServer.py
```

You may need to modify pose_engine.py. This may be fixed in the future.

```
diff --git a/pose_engine.py b/pose_engine.py
index 0c745d4..82664ad 100644
--- a/pose_engine.py
+++ b/pose_engine.py
@@ -100,7 +100,7 @@ class PoseEngine(BasicEngine):
         self._output_offsets = [0]
         for size in self.get_all_output_tensors_sizes():
             offset += size
-            self._output_offsets.append(offset)
+            self._output_offsets.append(int(offset))

     def DetectPosesInImage(self, img):
         """Detects poses in a given image.
```



