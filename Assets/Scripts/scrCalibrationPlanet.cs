using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class scrCalibrationPlanet : ADOBase
{
	private enum InputType
	{
		None,
		Keyboard,
		Mouse,
		Joystick,
		Touch,
		Mixed
	}

	private enum Feedback
	{
		OK,
		Inconsistent,
		Sparse
	}

	private enum Rank
	{
		E,
		D,
		C,
		B,
		A,
		S
	}

	[CompilerGenerated]
	private sealed class _003CUploadDataCo_003Ed__56 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public scrCalibrationPlanet _003C_003E4__this;

		private UnityWebRequest _003Crequest_003E5__2;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CUploadDataCo_003Ed__56(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			scrCalibrationPlanet scrCalibrationPlanet = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				scrCalibrationPlanet.sendButton.enabled = false;
				_003Crequest_003E5__2 = UnityWebRequest.Post("https://7thbe.at/api/submit-calibration", new List<IMultipartFormSection>
				{
					new MultipartFormDataSection("platform", _003CUploadDataCo_003Eg__ValidateString_007C56_0(ADOBase.platform.ToString())),
					new MultipartFormDataSection("operating_system", _003CUploadDataCo_003Eg__ValidateString_007C56_0(SystemInfo.operatingSystem.ToString())),
					new MultipartFormDataSection("device_model", _003CUploadDataCo_003Eg__ValidateString_007C56_0(SystemInfo.deviceModel.ToString())),
					new MultipartFormDataSection("input_type", _003CUploadDataCo_003Eg__ValidateString_007C56_0(scrCalibrationPlanet.inputType.ToString())),
					new MultipartFormDataSection("output_type", _003CUploadDataCo_003Eg__ValidateString_007C56_0(scrConductor.currentPreset.outputType.ToString())),
					new MultipartFormDataSection("output_name", _003CUploadDataCo_003Eg__ValidateString_007C56_0(scrCalibrationPlanet.exportOutputName)),
					new MultipartFormDataSection("input_offset", _003CUploadDataCo_003Eg__ValidateString_007C56_0(scrCalibrationPlanet.exportInputOffset)),
					new MultipartFormDataSection("priority", "1"),
					new MultipartFormDataSection("confident", "true"),
					new MultipartFormDataSection("standard_deviation", _003CUploadDataCo_003Eg__ValidateString_007C56_0(scrCalibrationPlanet.exportStandardDeviation))
				});
				scrCalibrationPlanet.sendImage.color = scrCalibrationPlanet.loadingColor;
				scrCalibrationPlanet.sendButtonLabel.text = RDString.Get("calibration.uploadingProcess");
				scrCalibrationPlanet.sendButtonLabel.color = Color.black;
				_003C_003E2__current = _003Crequest_003E5__2.SendWebRequest();
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_003Crequest_003E5__2.HasConnectionError())
				{
					scrCalibrationPlanet.sendImage.color = scrCalibrationPlanet.errorColor;
					scrCalibrationPlanet.sendButton.enabled = true;
					scrCalibrationPlanet.sendButtonLabel.text = RDString.Get("calibration.uploadingError", new Dictionary<string, object>
					{
						{
							"error",
							_003Crequest_003E5__2.error
						}
					});
					scrCalibrationPlanet.sendButtonLabel.color = Color.white;
					UnityEngine.Debug.Log(_003Crequest_003E5__2.error);
				}
				else
				{
					scrCalibrationPlanet.sendImage.color = scrCalibrationPlanet.successColor;
					scrCalibrationPlanet.sendButtonLabel.text = RDString.Get("calibration.uploadingSuccess");
					scrCalibrationPlanet.sendButtonLabel.color = Color.black;
					UnityEngine.Debug.Log("Form upload complete!");
				}
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	public List<double> listOffsets = new List<double>();

	public float radius = 1f;

	public double angleRadians;

	public double averageTimeOffset;

	public double averageAngleOffset;

	public double standardDeviation;

	public float keyCooldown;

	public static scrCalibrationPlanet instance;

	public new scrConductor conductor;

	public EventSystem eventSystem;

	public GameObject otherplanet;

	public GameObject calibrax;

	public Transform ring;

	public Text txtLastOffset;

	public Text txtMessage;

	public Text txtResults;

	public AudioClip applause;

	public AudioClip desktopCalibrationMusic;

	public TrailRenderer planetTrailRenderer;

	public RectTransform finishButton;

	[Header("Details Panel")]
	public RectTransform detailsPanel;

	public RectTransform detailsTab;

	public Button detailsTabButton;

	public Text calibrationInfo;

	public Button sendButton;

	public Image sendImage;

	public Text sendButtonLabel;

	public Color loadingColor;

	public Color successColor;

	public Color errorColor;

	public RectTransform activityIndicator;

	private InputType inputType;

	private bool showingInformationPanel;

	private int currentMessageNumber;

	private int attempts;

	private bool theresDataToSend;

	private string exportInputOffset;

	private string exportStandardDeviation;

	private string exportOutputName;

	private void Awake()
	{
		ADOBase.Startup();
		detailsPanel.AnchorPosX(0f - detailsTab.sizeDelta.x);
		planetTrailRenderer.enabled = false;
		txtMessage.text = "";
		txtLastOffset.text = "";
		txtResults.text = "";
		SetMessageNumber(0);
		AudioListener.volume = (float)Persistence.GetGlobalVolume() * 0.1f;
		calibrationInfo.SetLocalizedFont();
		instance = this;
	}

	private void Start()
	{
		conductor = scrConductor.instance;
		scrMisc.Disappear(this);
		GCS.hasVisitedCalibration = true;
		txtMessage.SetLocalizedFont();
		txtResults.SetLocalizedFont();
		txtLastOffset.SetLocalizedFont();
		if (!ADOBase.isMobile)
		{
			conductor.song.clip = desktopCalibrationMusic;
			conductor.bpm = 130f;
		}
	}

	private void SetMessageNumber(int n)
	{
		currentMessageNumber = n;
		if ((n == 0 || n == 2) && !ADOBase.isMobile)
		{
			txtMessage.text = RDString.Get($"calibration.{n}.legacy");
		}
		else
		{
			txtMessage.text = RDString.Get("calibration." + currentMessageNumber.ToString());
		}
		if (currentMessageNumber <= 2)
		{
			txtMessage.text = txtMessage.text.Replace("[output]", "<b>" + scrConductor.currentPreset.ReadableOutputName() + "</b>");
		}
	}

	private void PostSong()
	{
		scrCamera.instance.GetComponent<Camera>().backgroundColor = Color.white;
		scrMisc.FadeOut(this, 0f, 0.5f, shouldscale: false);
		planetTrailRenderer.enabled = false;
		switch (CheckConsistency())
		{
		case Feedback.OK:
		{
			txtLastOffset.text = "";
			SetMessageNumber(2);
			if (standardDeviation < 0.015)
			{
				conductor.song.PlayOneShot(applause);
			}
			Rank value = Rank.D;
			if (standardDeviation < 0.01)
			{
				value = Rank.S;
			}
			else if (standardDeviation < 0.015)
			{
				value = Rank.A;
			}
			else if (standardDeviation < 0.02)
			{
				value = Rank.B;
			}
			else if (standardDeviation < 0.025)
			{
				value = Rank.C;
			}
			else if (standardDeviation < (ADOBase.isMobile ? 0.06 : 0.05))
			{
				value = Rank.D;
			}
			PlayerPrefs.SetInt("maxcalibrationrank", (int)value);
			PlayerPrefs.Save();
			double num = Math.Round(averageTimeOffset * 1000.0);
			string text = num.ToString() + RDString.Get("editor.unit.ms");
			string text2 = value.ToString();
			txtResults.text = RDString.Get("calibration.offset") + " <b>" + text + "</b>\n" + RDString.Get("calibration.skill") + " <b>" + text2 + "</b>\n" + RDString.Get("calibration.rank" + text2);
			exportOutputName = scrConductor.currentPreset.outputName;
			string[] array = new string[3]
			{
				"AirPods Pro",
				"AirPods",
				"Powerbeats Pro"
			};
			foreach (string value2 in array)
			{
				if (exportOutputName.Contains(value2))
				{
					exportOutputName = value2;
					break;
				}
			}
			exportInputOffset = num.ToString();
			exportStandardDeviation = (standardDeviation * 1000.0).ToString("0.000");
			calibrationInfo.text = RDString.Get("calibration.inputOffset") + ": " + exportInputOffset + "ms\n" + RDString.Get("calibration.standardDeviation") + ": " + exportStandardDeviation + "ms\n" + RDString.Get("calibration.operatingSystem") + ": " + SystemInfo.operatingSystem + "\n" + RDString.Get("calibration.deviceModel") + ": " + SystemInfo.deviceModel + "\n" + RDString.Get("calibration.inputType") + ": " + inputType.ToString() + "\n" + string.Format("{0}: {1}\n", RDString.Get("calibration.outputType"), scrConductor.currentPreset.outputType) + RDString.Get("calibration.outputName") + ": " + exportOutputName + "\n";
			scrConductor.currentPreset.inputOffset = Mathf.RoundToInt((float)(averageTimeOffset * 1000.0));
			scrConductor.SaveCurrentPreset();
			if (ADOBase.isMobile)
			{
				ShowDetailsTab(show: true);
			}
			break;
		}
		case Feedback.Inconsistent:
			SetMessageNumber(3);
			attempts++;
			break;
		case Feedback.Sparse:
			SetMessageNumber(4);
			attempts++;
			break;
		}
		if (attempts >= 3)
		{
			SetMessageNumber(5);
			scrCamera.instance.GetComponent<Camera>().backgroundColor = Color.white;
		}
		keyCooldown = 1f;
	}

	private void Update()
	{
		angleRadians = 1.5707963705062866 + (conductor.songposition_minusi + (double)(scrConductor.calibration_i * conductor.song.pitch)) / conductor.crotchet * 3.1415927410125732;
		Vector3 position = otherplanet.transform.position;
		base.transform.position = new Vector3(position.x + Mathf.Sin((float)angleRadians) * radius, position.y + Mathf.Cos((float)angleRadians) * radius, position.z);
		ring.Rotate(Vector3.back, -30f * Time.unscaledDeltaTime);
		if (keyCooldown <= 0f)
		{
			InputType inputTypeForDown = GetInputTypeForDown();
			bool num = inputTypeForDown == InputType.Keyboard;
			bool flag = inputTypeForDown == InputType.Mouse && eventSystem.currentSelectedGameObject == null;
			bool flag2 = inputTypeForDown == InputType.Touch && eventSystem.currentSelectedGameObject == null;
			if (num | flag | flag2)
			{
				if (currentMessageNumber == 0)
				{
					conductor.StartMusic(PostSong);
					scrMisc.FadeIn(this, 0f, 0.5f, 0.5f, shouldscale: true);
					planetTrailRenderer.enabled = true;
					SetMessageNumber(currentMessageNumber + 1);
				}
				else if (currentMessageNumber == 1)
				{
					if (inputType == InputType.None)
					{
						inputType = inputTypeForDown;
						printe("input type is: " + inputType.ToString());
					}
					else if (inputType != InputType.Mixed && inputType != inputTypeForDown)
					{
						printe("calibration got dirty!");
						inputType = InputType.Mixed;
					}
					PutDataPoint();
				}
				else if (currentMessageNumber == 2)
				{
					Quit();
				}
				else if (currentMessageNumber == 3 || currentMessageNumber == 4)
				{
					CleanSlate();
				}
				else if (currentMessageNumber == 5)
				{
					scrConductor.UpdateCurrentAudioOutput();
					Quit();
				}
			}
		}
		if (keyCooldown > 0f)
		{
			keyCooldown -= Time.unscaledDeltaTime;
		}
	}

	private void LateUpdate()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Q) || RDInput.cancelPress)
		{
			Quit();
		}
	}

	private void CleanSlate()
	{
		listOffsets.Clear();
		SetMessageNumber(1);
		conductor.StartMusic(PostSong);
		scrMisc.FadeIn(this, 0f, 0.5f, 0.5f, shouldscale: true);
		planetTrailRenderer.enabled = true;
		inputType = InputType.None;
		GameObject[] array = GameObject.FindGameObjectsWithTag("X");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
		ShowDetailsTab(show: false);
	}

	private void PutDataPoint()
	{
		scrMisc.Vibrate(50L);
		UnityEngine.Object.Instantiate(calibrax, base.gameObject.transform.position, Quaternion.identity).GetComponent<scrCalibrax>();
		double offset = GetOffset(angleRadians, conductor.bpm);
		listOffsets.Add(offset);
		txtLastOffset.text = Math.Round(offset * 1000.0).ToString() + RDString.Get("editor.unit.ms");
		float num = 0f;
		int num2 = 0;
		foreach (double listOffset in listOffsets)
		{
			float num3 = (float)listOffset;
			num += num3;
			num2++;
		}
		averageTimeOffset = num / (float)num2;
		averageAngleOffset = averageTimeOffset / (double)(60f / conductor.bpm) * 3.1415927410125732;
	}

	private double GetOffset(double anglerad, double bpm)
	{
		Vector3 zero = Vector3.zero;
		anglerad %= 6.2831854820251465;
		double num = 0.0;
		double num2 = 0.0;
		num2 = ((!ADOBase.isMobile) ? 0.78539818525314331 : 1.5707963705062866);
		if (anglerad < 3.1415927410125732 + num2 && anglerad > num2)
		{
			num = 1.5707963705062866;
		}
		if (anglerad > 3.1415927410125732 + num2)
		{
			num = 4.71238899230957;
		}
		if (anglerad < num2)
		{
			num = -1.5707963705062866;
		}
		return (anglerad - num) * (60.0 / bpm) / 3.1415927410125732;
	}

	private Feedback CheckConsistency()
	{
		int num = 0;
		double num2 = 0.20000000298023224;
		foreach (double listOffset in listOffsets)
		{
			if (Math.Abs(listOffset - averageTimeOffset) > num2)
			{
				num++;
			}
		}
		if (num > (ADOBase.isMobile ? 3 : 5))
		{
			return Feedback.Sparse;
		}
		foreach (double item in listOffsets.ToList())
		{
			if (Math.Abs(item - averageTimeOffset) > num2)
			{
				listOffsets.Remove(item);
			}
		}
		int num3 = 0;
		double num4 = 0.0;
		foreach (double listOffset2 in listOffsets)
		{
			num4 += listOffset2;
			num3++;
		}
		averageTimeOffset = num4 / (double)num3;
		int num5 = 0;
		double num6 = 0.0;
		foreach (double listOffset3 in listOffsets)
		{
			num6 += Math.Pow(listOffset3 - averageTimeOffset, 2.0);
			num5++;
		}
		standardDeviation = Math.Sqrt(num6 / (double)num5);
		if (num5 < (ADOBase.isMobile ? 7 : 10))
		{
			return Feedback.Sparse;
		}
		if (standardDeviation > 0.1)
		{
			return Feedback.Inconsistent;
		}
		return Feedback.OK;
	}

	private void Quit()
	{
		DOTween.KillAll();
		string scene = GCS.webVersion ? "scnIntro" : GCNS.sceneLevelSelect;
		if (GCS.lastVisitedScene != "")
		{
			scene = GCS.lastVisitedScene;
		}
		ADOBase.LoadScene(scene);
	}

	public void ShowDetailsTab(bool show)
	{
		if (show)
		{
			detailsPanel.gameObject.SetActive(value: true);
		}
		float endValue = show ? 0f : (0f - detailsPanel.sizeDelta.x);
		detailsPanel.DOKill();
		detailsPanel.DOAnchorPosX(endValue, 0.3f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
		detailsTabButton.enabled = show;
	}

	public void DetailsTabClick()
	{
		showingInformationPanel = !showingInformationPanel;
		float endValue = showingInformationPanel ? (detailsPanel.sizeDelta.x - 10f) : 0f;
		detailsPanel.DOKill();
		detailsPanel.DOAnchorPosX(endValue, 0.3f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
	}

	public void UploadData()
	{
		StartCoroutine(UploadDataCo());
	}

	private IEnumerator UploadDataCo()
	{
		sendButton.enabled = false;
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		list.Add(new MultipartFormDataSection("platform", _003CUploadDataCo_003Eg__ValidateString_007C56_0(ADOBase.platform.ToString())));
		list.Add(new MultipartFormDataSection("operating_system", _003CUploadDataCo_003Eg__ValidateString_007C56_0(SystemInfo.operatingSystem.ToString())));
		list.Add(new MultipartFormDataSection("device_model", _003CUploadDataCo_003Eg__ValidateString_007C56_0(SystemInfo.deviceModel.ToString())));
		list.Add(new MultipartFormDataSection("input_type", _003CUploadDataCo_003Eg__ValidateString_007C56_0(inputType.ToString())));
		list.Add(new MultipartFormDataSection("output_type", _003CUploadDataCo_003Eg__ValidateString_007C56_0(scrConductor.currentPreset.outputType.ToString())));
		list.Add(new MultipartFormDataSection("output_name", _003CUploadDataCo_003Eg__ValidateString_007C56_0(exportOutputName)));
		list.Add(new MultipartFormDataSection("input_offset", _003CUploadDataCo_003Eg__ValidateString_007C56_0(exportInputOffset)));
		list.Add(new MultipartFormDataSection("priority", "1"));
		list.Add(new MultipartFormDataSection("confident", "true"));
		list.Add(new MultipartFormDataSection("standard_deviation", _003CUploadDataCo_003Eg__ValidateString_007C56_0(exportStandardDeviation)));
		UnityWebRequest request = UnityWebRequest.Post("https://7thbe.at/api/submit-calibration", list);
		sendImage.color = loadingColor;
		sendButtonLabel.text = RDString.Get("calibration.uploadingProcess");
		sendButtonLabel.color = Color.black;
		yield return request.SendWebRequest();
		if (request.HasConnectionError())
		{
			sendImage.color = errorColor;
			sendButton.enabled = true;
			sendButtonLabel.text = RDString.Get("calibration.uploadingError", new Dictionary<string, object>
			{
				{
					"error",
					request.error
				}
			});
			sendButtonLabel.color = Color.white;
			UnityEngine.Debug.Log(request.error);
		}
		else
		{
			sendImage.color = successColor;
			sendButtonLabel.text = RDString.Get("calibration.uploadingSuccess");
			sendButtonLabel.color = Color.black;
			UnityEngine.Debug.Log("Form upload complete!");
		}
	}

	private InputType GetInputTypeForDown()
	{
		if (UnityEngine.Input.touchCount > 0)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.phase == TouchPhase.Began)
				{
					return InputType.Touch;
				}
			}
		}
		else if (Input.anyKeyDown)
		{
			for (int j = 1; j < 600; j++)
			{
				if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
				{
					return InputType.Keyboard;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
				{
					return InputType.Mouse;
				}
				if (UnityEngine.Input.GetKeyDown((KeyCode)j))
				{
					if (j < 323)
					{
						return InputType.Keyboard;
					}
					if (j < 350)
					{
						return InputType.Mouse;
					}
					return InputType.Joystick;
				}
			}
		}
		return InputType.None;
	}

	public void OpenPrivacyPolicy()
	{
		Application.OpenURL("https://7thbe.at/privacy");
	}

	public void QuitButton()
	{
		Quit();
	}

	[CompilerGenerated]
	private static string _003CUploadDataCo_003Eg__ValidateString_007C56_0(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return "[Not Found]";
		}
		return s;
	}
}
