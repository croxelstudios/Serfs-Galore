using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//simple script for taking screenshots and recording sequences of screenshots (whilst maintaining output FPS of recording)
//features
// - Specify screenshot scale (double, triple, etc)
// - Capture single screenshots and video
// - Record video across scene changes
// - captures are nicely organised and don't overwrite one-another
//
// yw <3

public class Cappy : MonoBehaviour {

	//sets output FPS of captured recordings
	public int captureFPS = 30;

	//size of captured recordings relative to current screen size
	public int captureScale = 2;

    //set these to choose how cappy is controlled
    [SerializeField]
    KeyCode recordKey = KeyCode.Alpha9;
    [SerializeField]
    KeyCode screenshotKey = KeyCode.Alpha0;

    //whether to record audio events to be replayed at realtime later
    public bool recordAudioEvents = false;

	void Start(){
		//reminder in case you forget to delete/disable the cappy game object and don't want to ship with it :P
		Debug.Log("CAPPY EXISTS IN SCENE - DELETE ME BEFORE MAKING FINAL GAME BUILD");

		//one cappy instance at a time, can record across scene changes with this too
		if (Cappy.cappy != null){
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		Cappy.cappy = this;
	}

	void Update () {
		//screenshots
		if (Input.GetKeyDown(screenshotKey)){
			if (!System.IO.Directory.Exists("CappyScreenshots")) System.IO.Directory.CreateDirectory("CappyScreenshots");
			string screenshotname = string.Format("{0}/Screenshot_{1:D04}.png", "CappyScreenshots", NowString(true));
			ScreenCapture.CaptureScreenshot(screenshotname,captureScale);
			Debug.Log( "Screenshot captured and saved to: " + screenshotname );
		}
		//recording
		if (recording){
			string framename = string.Format("{0}/Cap_{1:D08}.png", videoPath, frameNum);
			ScreenCapture.CaptureScreenshot(framename,captureScale);
			RecordCam();

			frameNum++;
						recordingTime += Time.deltaTime;


			Debug.Log("CAPPY REC: [" + frameNum.ToString() + " frames] [" + FrameCountToPlaybackTime() + " Playback time at " + captureFPS.ToString() + " FPS]");
		}
		if (Input.GetKeyDown(recordKey)){
			if (!recording){
				StartRecording();
			}else{
				StopRecording();
			}
		}
	}

	public void StartRecording(){
		if (recording){
			Debug.Log("Cappy - Already recording!");
			return;
		}
				string savePath = NowString(false);

				recordingTime = 0f;

				audioTimes = new List<int>();
				audioTimesF = new List<float>();
				audioClips = new List<string>();
				audioPositions = new List<Vector3>();
				camTimes = new List<int>();
				camTimesF = new List<float>();
				camPositions = new List<Vector3>();
				camRotations = new List<Vector3>();
				audioDataLabel = savePath;//NowString(false);
				audioSavePath = savePath;//"";

				videoPath = "CappyRecording_" + savePath;//NowString(false);

				audioSavePath = videoPath;
		if (!System.IO.Directory.Exists(videoPath)) System.IO.Directory.CreateDirectory(videoPath);
		recording = true;
		frameNum = 0;
		Debug.Log("Recording started");
		Time.captureFramerate = captureFPS;
	}
	public void StopRecording(){
		if (!recording) return;


				if (recordAudioEvents) SaveAudioEvents();


		Debug.Log("Recording finished - " + frameNum.ToString() + " frames saved to: " + videoPath);
		recording = false;
		Time.captureFramerate = 0;
	}

		// to record audio need:
		// - clip name & start frame
		// - position (every frame after for the duration of clip)
		// - volume "
		// - audio clip bypass settings
		// - audio listener position/rotation for entire recording duration
		public void RecordAudioClip(string clipName, Vector3 clipPos){
				if (!recordAudioEvents || !recording) return;
				audioTimes.Add(frameNum);
				audioTimesF.Add(recordingTime);
				audioClips.Add(clipName);
				audioPositions.Add(clipPos);
		}
		private void RecordCam(){
				if (!recordAudioEvents || !recording) return;
				camTimes.Add(frameNum);
				camTimesF.Add(recordingTime);
				camPositions.Add(Camera.main.transform.position);
				camRotations.Add(Camera.main.transform.eulerAngles);
		}

		private void SaveAudioEvents(){
				string saveString = "";
				for (int i=0; i<=frameNum; i++){
						for (int k=0; k<camTimes.Count; k++){
								if (i==camTimes[k]){
										saveString += i.ToString() + ",CAM," + camPositions[k].x.ToString()+"," + camPositions[k].y.ToString()+"," + camPositions[k].z.ToString()+"," + camRotations[k].x.ToString()+"," + camRotations[k].y.ToString()+"," + camRotations[k].z.ToString() +","+camTimesF[k].ToString()+ "\n";
								}
						}
						for (int k=0; k<audioTimes.Count; k++){
								if (i==audioTimes[k]){
										saveString += i.ToString() + ",AUDIO," + audioClips[k] + "," + audioPositions[k].x.ToString()+"," + audioPositions[k].y.ToString()+"," + audioPositions[k].z.ToString()+","+audioTimesF[k].ToString()+"\n";
								}
						}
				}
				saveString += frameNum.ToString() + ",END,"+recordingTime.ToString()+"\n";
				saveString += frameNum.ToString() + ",SAVEPATH,"+audioSavePath;

				Debug.Log("AUDIO RECORD: \n" + saveString);

				string filename = string.Format("{0}/AudioData_{1}.txt", videoPath, audioDataLabel);
				PlayerPrefs.SetString("CappyRecordedAudioDataPath",filename);
				System.IO.File.WriteAllText(filename, saveString);
		}

		private string audioDataLabel = "";
		private string audioSavePath = "";
		private List<int> audioTimes = new List<int>();
		private List<float> audioTimesF = new List<float>();
		private List<string> audioClips = new List<string>();
		private List<Vector3> audioPositions = new List<Vector3>();
		private List<int> camTimes = new List<int>();
		private List<float> camTimesF = new List<float>();
		private List<Vector3> camPositions = new List<Vector3>();
		private List<Vector3> camRotations = new List<Vector3>();

		private float recordingTime = 0f;


	private bool recording = false;
	public static Cappy cappy;
	private string videoPath;
	private int frameNum = 0;

	string NowString(bool includeLevelTime){
		//get current time as string, to include in filenames so shots are dated and not overwriting one-another
		string nowString = System.DateTime.Now.ToString();
		nowString = nowString.Replace("\\","");
		nowString = nowString.Replace("/","");
		nowString = nowString.Replace(" ","-");
		nowString = nowString.Replace(":","");
		if (includeLevelTime) nowString += "-" + Mathf.RoundToInt(Time.timeSinceLevelLoad*100f).ToString();
		return nowString;

	}

	string FrameCountToPlaybackTime(){

		float playbackSeconds = (float)frameNum / (float)captureFPS;

		int minutes = 0;
		int seconds = Mathf.FloorToInt(playbackSeconds);
		while(seconds>60){
			playbackSeconds-=60f;
			seconds -=60;
			minutes++;
		}

		string retStr = playbackSeconds.ToString("F");
		if (seconds.ToString().Length==1) retStr = "0" + retStr;
		return minutes.ToString() + ":" +retStr;

	}
}
