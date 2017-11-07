using System;
using System.Collections;
using UnityEngine;
using CodeStage.AdvancedFPSCounter.CountersData;
using CodeStage.AdvancedFPSCounter.Labels;
using UnityEngine.UI;

namespace CodeStage.AdvancedFPSCounter
{
	/// <summary>
	/// Allows to see frames per second counter, memory usage counter and some simple hardware information right in running app on any device.<br/>
	/// Just call AFPSCounter.AddToScene() to use it.
	/// </summary>
	/// You also may add it to GameObject (without any child or parent objects, with zero rotation, zero position and 1,1,1 scale) as usual or through the<br/>
	/// "GameObject > Create Other > Code Stage > Advanced FPS Counter" menu.
	[AddComponentMenu(MENU_PATH)]
	[DisallowMultipleComponent]
	public class AFPSCounter: MonoBehaviour
	{
		// ----------------------------------------------------------------------------
		// constants
		// ----------------------------------------------------------------------------
		private const string MENU_PATH = "Code Stage/Advanced FPS Counter";

		private const string COMPONENT_NAME = "Advanced FPS Counter";
		private const string LOG_PREFIX = "<b>[AFPSCounter]:</b> ";
		internal const char NEW_LINE = '\n';
		internal const char SPACE = ' ';

		// ----------------------------------------------------------------------------
		// public fields
		// ----------------------------------------------------------------------------

		/// <summary>
		/// Frames Per Second counter.
		/// </summary>
		public FPSCounterData fpsCounter = new FPSCounterData();

		/// <summary>
		/// Mono or heap memory counter.
		/// </summary>
		public MemoryCounterData memoryCounter = new MemoryCounterData();

		/// <summary>
		/// Device hardware info.<br/>
		/// Shows CPU name, cores (threads) count, GPU name, total VRAM, total RAM, screen DPI and screen size.
		/// </summary>
		public DeviceInfoCounterData deviceInfoCounter = new DeviceInfoCounterData();

		/// <summary>
		/// Used to enable / disable plugin at runtime. Set to KeyCode.None to disable.
		/// </summary>
		[Tooltip("Used to enable / disable plugin.\nSet to None to disable.")]
		public KeyCode hotKey = KeyCode.BackQuote;

		/// <summary>
		/// Allows to keep Advanced FPS Counter game object on new level (scene) load.
		/// </summary>
		[Tooltip("Prevent current Game Object from destroying on level (scene) load.")]
		public bool keepAlive = true;

		// ----------------------------------------------------------------------------
		// internal fields
		// ----------------------------------------------------------------------------

		internal Canvas canvas;
		internal CanvasScaler canvasScaler;
		internal DrawableLabel[] labels;

		// ----------------------------------------------------------------------------
		// private fields
		// ----------------------------------------------------------------------------

		private int anchorsCount;
		private int cachedVSync = -1;
		private int cachedFrameRate = -1;
		private bool inited;

		// ----------------------------------------------------------------------------
		// properties
		// ----------------------------------------------------------------------------

		#region OperationMode
		[Tooltip("Disabled: removes labels and stops all internal processes except Hot Key listener.\n\n" +
		         "Background: removes labels keeping counters alive; use for hidden performance monitoring.\n\n" +
		         "Normal: shows labels and runs all internal processes as usual.")]
		[SerializeField]
		private OperationMode operationMode = OperationMode.Normal;

		/// <summary>
		/// Use it to change %AFPSCounter operation mode.
		/// </summary>
		/// Disabled: removes labels and stops all internal processes except Hot Key listener.<br/>
		/// Background: removes labels keeping counters alive. May be useful for hidden performance monitoring and benchmarking. Hot Key has no effect in this mode.<br/>
		/// Normal: shows labels and runs all internal processes as usual.
		public OperationMode OperationMode
		{
			get { return operationMode; }
			set
			{
				if (operationMode == value || !Application.isPlaying) return;
				operationMode = value;

				if (operationMode != OperationMode.Disabled)
				{
					if (operationMode == OperationMode.Background)
					{
						for (int i = 0; i < anchorsCount; i++)
						{
							labels[i].Clear();
						}
					}

					OnEnable();

					fpsCounter.UpdateValue();
					memoryCounter.UpdateValue();
					deviceInfoCounter.UpdateValue();

					UpdateTexts();
				}
				else
				{
					OnDisable();
				}
			}
		}
		#endregion

		#region ForceFrameRate
		[Tooltip("Allows to see how your game performs on specified frame rate.\n" +
		         "Does not guarantee selected frame rate. Set -1 to render as fast as possible in current conditions.\n" +
		         "IMPORTANT: this option disables VSync while enabled!")]
		[SerializeField]
		private bool forceFrameRate;

		/// <summary>
		/// Allows to see how your game performs on specified frame rate.<br/>
		/// <strong>\htmlonly<font color="7030A0">IMPORTANT:</font>\endhtmlonly this option disables VSync while enabled!</strong>
		/// </summary>
		/// Useful to check how physics performs on slow devices for example.
		public bool ForceFrameRate
		{
			get { return forceFrameRate; }
			set
			{
				if (forceFrameRate == value || !Application.isPlaying) return;
				forceFrameRate = value;
				if (operationMode == OperationMode.Disabled) return;

				RefreshForcedFrameRate();
			}
		}
		#endregion

		#region ForcedFrameRate
		[Range(-1, 200)]
		[SerializeField]
		private int forcedFrameRate = -1;

		/// <summary>
		/// Desired frame rate for ForceFrameRate option, does not guarantee selected frame rate.
		/// Set to -1 to render as fast as possible in current conditions.
		/// </summary>
		public int ForcedFrameRate
		{
			get { return forcedFrameRate; }
			set
			{
				if (forcedFrameRate == value || !Application.isPlaying) return;
				forcedFrameRate = value;
				if (operationMode == OperationMode.Disabled) return;

				RefreshForcedFrameRate();
			}
		}
		#endregion

		/* look and feel settings */

		#region ScaleFactor
		[Tooltip("Controls global scale of all texts.")]
		[Range(0f, 30f)]
		[SerializeField]
		private float scaleFactor = 1;

		/// <summary>
		/// Controls global scale of all texts.
		/// </summary>
		public float ScaleFactor
		{
			get { return scaleFactor; }
			set
			{
				if (Math.Abs(scaleFactor - value) < 0.001f || !Application.isPlaying) return;
				scaleFactor = value;
				if (operationMode == OperationMode.Disabled || canvasScaler == null) return;

				canvasScaler.scaleFactor = scaleFactor;
			}
		}
		#endregion

		#region LabelsFont
		[Tooltip("Leave blank to use default font.")]
		[SerializeField]
		private Font labelsFont;

		/// <summary>
		/// Font to render labels with.
		/// </summary>
		public Font LabelsFont
		{
			get { return labelsFont; }
			set
			{
				if (labelsFont == value || !Application.isPlaying) return;
				labelsFont = value;
				if (operationMode == OperationMode.Disabled || labels == null) return;

				for (int i = 0; i < anchorsCount; i++)
				{
					labels[i].ChangeFont(labelsFont);
				}
			}
		}
		#endregion

		#region FontSize
		[Tooltip("Set to 0 to use font size specified in the font importer.")]
		[Range(0, 100)]
		[SerializeField]
		private int fontSize;

		/// <summary>
		/// The font size to use (for dynamic fonts).
		/// </summary>
		/// If this is set to a non-zero value, the font size specified in the font importer is overridden with a custom size. This is only supported for fonts set to use dynamic font rendering. Other fonts will always use the default font size.
		public int FontSize
		{
			get { return fontSize; }
			set
			{
				if (fontSize == value || !Application.isPlaying) return;
				fontSize = value;
				if (operationMode == OperationMode.Disabled || labels == null) return;

				for (int i = 0; i < anchorsCount; i++)
				{
					labels[i].ChangeFontSize(fontSize);
				}
			}
		}
		#endregion

		#region LineSpacing
		[Tooltip("Space between lines in labels.")]
		[Range(0f, 10f)]
		[SerializeField]
		private float lineSpacing = 1;

		/// <summary>
		/// Space between lines.
		/// </summary>
		public float LineSpacing
		{
			get { return lineSpacing; }
			set
			{
				if (Math.Abs(lineSpacing - value) < 0.001f || !Application.isPlaying) return;
				lineSpacing = value;
				if (operationMode == OperationMode.Disabled || labels == null) return;

				for (int i = 0; i < anchorsCount; i++)
				{
					labels[i].ChangeLineSpacing(lineSpacing);
				}
			}
		}
		#endregion

		#region CountersSpacing
		[Tooltip("Lines count between different counters in a single label.")]
		[Range(0, 10)]
		[SerializeField]
		private int countersSpacing;

		/// <summary>
		/// Lines count between different counters in a single label.
		/// </summary>
		public int CountersSpacing
		{
			get { return countersSpacing; }
			set
			{
				if (Math.Abs(countersSpacing - value) < 0.001f || !Application.isPlaying) return;
				countersSpacing = value;
				if (operationMode == OperationMode.Disabled || labels == null) return;

				UpdateTexts();
				for (int i = 0; i < anchorsCount; i++)
				{
					labels[i].dirty = true;
				}
			}
		}
		#endregion

		#region PaddingOffset
		[Tooltip("Pixel offset for anchored labels. Automatically applied to all 4 corners.")]
		[SerializeField]
		private Vector2 paddingOffset = new Vector2(5, 5);

		/// <summary>
		/// Pixel offset for anchored labels. Automatically applied to all 4 corners.
		/// </summary>
		public Vector2 PaddingOffset
		{
			get { return paddingOffset; }
			set
			{
				if (paddingOffset == value || !Application.isPlaying) return;
				paddingOffset = value;
				if (operationMode == OperationMode.Disabled || labels == null) return;

				for (int i = 0; i < anchorsCount; i++)
				{
					labels[i].ChangeOffset(paddingOffset);
				}
			}
		}
		#endregion

		/* advanced settings */

		#region SortingOrder
		[Tooltip("Sorting order to use for the canvas.\nSet higher value to get closer to the user.")]
		[SerializeField]
		private int sortingOrder = 10000;

		/// <summary>
		/// Sorting order to use for the canvas.
		/// </summary>
		/// Set higher value to get closer to the user.
		public int SortingOrder
		{
			get { return sortingOrder; }
			set
			{
				if (sortingOrder == value || !Application.isPlaying) return;
				sortingOrder = value;

				if (operationMode == OperationMode.Disabled || canvas == null) return;

				canvas.sortingOrder = sortingOrder;
			}
		}
		#endregion

		// preventing direct instantiation
		private AFPSCounter() { }

		// ----------------------------------------------------------------------------
		// instance
		// ----------------------------------------------------------------------------

		/// <summary>
		/// Allows reaching public properties from code. Can be null.
		/// \sa AddToScene()
		/// </summary>
		public static AFPSCounter Instance { get; private set; }

		private static AFPSCounter GetOrCreateInstance
		{
			get
			{
				if (Instance == null)
				{
					AFPSCounter counter = FindObjectOfType<AFPSCounter>();
					if (counter != null)
					{
						Instance = counter;
					}
					else
					{
						CreateInScene(false);
					}
				}
				return Instance;
			}
		}

		// ----------------------------------------------------------------------------
		// public static methods
		// ----------------------------------------------------------------------------

		/// <summary>
		/// Creates and adds new %AFPSCounter instance to the scene if it doesn't exists.
		/// Use it to instantiate %AFPSCounter from code before using AFPSCounter.Instance.
		/// </summary>
		/// <returns>Existing or new %AFPSCounter instance.</returns>
		public static AFPSCounter AddToScene()
		{
			return GetOrCreateInstance;
		}

		/// <summary>
		/// Use it to completely dispose current %AFPSCounter instance.
		/// </summary>
		public static void Dispose()
		{
			if (Instance != null) Instance.DisposeInternal();
		}

		// ----------------------------------------------------------------------------
		// internal static methods
		// ----------------------------------------------------------------------------

		internal static string Color32ToHex(Color32 color)
		{
			return color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2") + color.a.ToString("x2");
		}

		// ----------------------------------------------------------------------------
		// private static methods
		// ----------------------------------------------------------------------------

		private static AFPSCounter CreateInScene()
		{
			return CreateInScene(true);
		}

		private static AFPSCounter CreateInScene(bool lookForExistingConatiner)
		{
			GameObject container = lookForExistingConatiner ? GameObject.Find(COMPONENT_NAME) : null;
			if (container == null)
			{
				container = new GameObject(COMPONENT_NAME);
				container.layer = LayerMask.NameToLayer("UI");
#if UNITY_EDITOR
				UnityEditor.Undo.RegisterCreatedObjectUndo(container, "Create " + COMPONENT_NAME);
				UnityEditor.Selection.activeObject = container;
#endif
			}
			return container.AddComponent<AFPSCounter>();
		}

		// ----------------------------------------------------------------------------
		// unity callbacks
		// ----------------------------------------------------------------------------

		#region unity callbacks
		private void Awake()
		{
			/* checks for duplication */

			if (Instance != null && Instance.keepAlive)
			{
				Destroy(this);
				return;
			}

			/* editor-only checks */

#if UNITY_EDITOR
			if (!IsPlacedCorrectly())
			{
				Debug.LogWarning(LOG_PREFIX + "incorrect placement detected! Please, use \"" + GAME_OBJECT_MENU_GROUP + MENU_PATH + "\" menu to fix it!", this);
			}
#endif

			/* initialization */

			Instance = this;
			DontDestroyOnLoad(gameObject);

			fpsCounter.Init(this);
			memoryCounter.Init(this);
			deviceInfoCounter.Init(this);

			ConfigureCanvas();
			ConfigureLabels();

			inited = true;
		}

		private void Update()
		{
			if (!inited) return;

			if (hotKey != KeyCode.None)
			{
				if (Input.GetKeyDown(hotKey))
				{
					SwitchCounter();
				}
			}
		}

		private void OnLevelWasLoaded(int index)
		{
			if (!inited) return;

			if (!keepAlive)
			{
				DisposeInternal();
			}
			else
			{
				if (fpsCounter.Enabled)
				{
					if (fpsCounter.MinMax && fpsCounter.resetMinMaxOnNewScene) fpsCounter.ResetMinMax();
					if (fpsCounter.Average && fpsCounter.resetAverageOnNewScene) fpsCounter.ResetAverage();
				}
			}
		}

		private void OnEnable()
		{
			if (!inited) return;

			if (operationMode == OperationMode.Disabled) return;
			ActivateCounters();
			Invoke("RefreshForcedFrameRate", 0.5f);
		}

		private void OnDisable()
		{
			if (!inited) return;

			DeactivateCounters();
			if (IsInvoking("RefreshForcedFrameRate")) CancelInvoke("RefreshForcedFrameRate");
			RefreshForcedFrameRate(true);

			for (int i = 0; i < anchorsCount; i++)
			{
				labels[i].Clear();
			}
		}

		private void OnDestroy()
		{
			if (inited)
			{
				fpsCounter.Dispose();
				memoryCounter.Dispose();
				deviceInfoCounter.Dispose();

				if (labels != null)
				{
					for (int i = 0; i < anchorsCount; i++)
					{
						labels[i].Dispose();
					}

					Array.Clear(labels, 0, anchorsCount);
					labels = null;
				}
				inited = false;
			}

			if (transform.childCount == 0 && GetComponentsInChildren<Component>().Length <= 2)
			{
				Destroy(gameObject);
			}
		}
		#endregion

		// ----------------------------------------------------------------------------
		// internal methods
		// ----------------------------------------------------------------------------

		internal void MakeDrawableLabelDirty(LabelAnchor anchor)
		{
			if (operationMode == OperationMode.Normal)
			{
				labels[(int)anchor].dirty = true;
			}
		}

		internal void UpdateTexts()
		{
			if (operationMode != OperationMode.Normal) return;

			bool anyContentPresent = false;

			if (fpsCounter.Enabled)
			{
				DrawableLabel label = labels[(int)fpsCounter.Anchor];
				if (label.newText.Length > 0)
				{
					label.newText.Append(new string(NEW_LINE, countersSpacing + 1));
				}
				label.newText.Append(fpsCounter.text);
				label.dirty |= fpsCounter.dirty;
				fpsCounter.dirty = false;

				anyContentPresent = true;
			}

			if (memoryCounter.Enabled)
			{
				DrawableLabel label = labels[(int)memoryCounter.Anchor];
				if (label.newText.Length > 0)
				{
					label.newText.Append(new string(NEW_LINE, countersSpacing + 1));
				}
				label.newText.Append(memoryCounter.text);
				label.dirty |= memoryCounter.dirty;
				memoryCounter.dirty = false;

				anyContentPresent = true;
			}

			if (deviceInfoCounter.Enabled)
			{
				DrawableLabel label = labels[(int)deviceInfoCounter.Anchor];
				if (label.newText.Length > 0)
				{
					label.newText.Append(new string(NEW_LINE, countersSpacing + 1));
				}
				label.newText.Append(deviceInfoCounter.text);
				label.dirty |= deviceInfoCounter.dirty;
				deviceInfoCounter.dirty = false;

				anyContentPresent = true;
			}

			if (anyContentPresent)
			{
				for (int i = 0; i < anchorsCount; i++)
				{
					labels[i].CheckAndUpdate();
				}
			}
			else
			{
				for (int i = 0; i < anchorsCount; i++)
				{
					labels[i].Clear();
				}
			}
		}

		// ----------------------------------------------------------------------------
		// private methods
		// ----------------------------------------------------------------------------
		private void ConfigureCanvas()
		{
			GameObject canvasObject = new GameObject("CountersCanvas");
			canvasObject.tag = tag;
			canvasObject.layer = gameObject.layer;
			canvasObject.transform.parent = transform;

			canvas = canvasObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.pixelPerfect = true;
			canvas.sortingOrder = 10000;

			canvasScaler = canvasObject.AddComponent<CanvasScaler>();
			canvasScaler.scaleFactor = 1;
		}

		private void ConfigureLabels()
		{
			anchorsCount = Enum.GetNames(typeof(LabelAnchor)).Length;
			labels = new DrawableLabel[anchorsCount];

			for (int i = 0; i < anchorsCount; i++)
			{
				labels[i] = new DrawableLabel(canvas, (LabelAnchor)i, paddingOffset, labelsFont, fontSize, lineSpacing);
			}
		}

		private void DisposeInternal()
		{
			Destroy(this);
			if (Instance == this) Instance = null;
		}

		private void SwitchCounter()
		{
			if (operationMode == OperationMode.Disabled)
			{
				OperationMode = OperationMode.Normal;
			}
			else if (operationMode == OperationMode.Normal)
			{
				OperationMode = OperationMode.Disabled;
			}
		}

		private void ActivateCounters()
		{
			fpsCounter.Activate();
			memoryCounter.Activate();
			deviceInfoCounter.Activate();

			if (fpsCounter.Enabled || memoryCounter.Enabled || deviceInfoCounter.Enabled)
			{
				UpdateTexts();
			}
		}

		private void DeactivateCounters()
		{
			if (Instance == null) return;

			fpsCounter.Deactivate();
			memoryCounter.Deactivate();
			deviceInfoCounter.Deactivate();
		}

		private void RefreshForcedFrameRate()
		{
			RefreshForcedFrameRate(false);
		}

		private void RefreshForcedFrameRate(bool disabling)
		{
			if (forceFrameRate && !disabling)
			{
				if (cachedVSync == -1)
				{
					cachedVSync = QualitySettings.vSyncCount;
					cachedFrameRate = Application.targetFrameRate;
					QualitySettings.vSyncCount = 0;
				}
				
				Application.targetFrameRate = forcedFrameRate;
			}
			else
			{
				if (cachedVSync != -1)
				{
					QualitySettings.vSyncCount = cachedVSync;
					Application.targetFrameRate = cachedFrameRate;
					cachedVSync = -1;
				}
			}
		}

		// ----------------------------------------------------------------------------
		// coroutines
		// ----------------------------------------------------------------------------

		private IEnumerator UpdateFPSCounter()
		{
			while (true)
			{
				float previousUpdateTime = Time.unscaledTime;
				int previousUpdateFrames = Time.frameCount;
				yield return new WaitForSeconds(fpsCounter.UpdateInterval);
				float timeElapsed = Time.unscaledTime - previousUpdateTime;
				int framesChanged = Time.frameCount - previousUpdateFrames;

				fpsCounter.newValue = framesChanged / timeElapsed;
				fpsCounter.UpdateValue(false);
				UpdateTexts();
			}
		}

		private IEnumerator UpdateMemoryCounter()
		{
			while (true)
			{
				memoryCounter.UpdateValue();
				UpdateTexts();
				yield return new WaitForSeconds(memoryCounter.UpdateInterval);
			}
		}

		// ----------------------------------------------------------------------------
		// editor-only code
		// ----------------------------------------------------------------------------

		#region editor-only code
#if UNITY_EDITOR
		private const string GAME_OBJECT_MENU_GROUP = "GameObject/Create Other/";
		private const string GAME_OBJECT_MENU_PATH = GAME_OBJECT_MENU_GROUP + MENU_PATH + " %&F";
		
		[HideInInspector]
		[SerializeField]
		private bool fpsGroupFoldout;

		[HideInInspector]
		[SerializeField]
		private bool memoryGroupFoldout;

		[HideInInspector]
		[SerializeField]
		private bool deviceGroupFoldout;

		[HideInInspector]
		[SerializeField]
		private bool lookAndFeelFoldout;

		[HideInInspector]
		[SerializeField]
		private bool advancedFoldout;

		[UnityEditor.MenuItem(GAME_OBJECT_MENU_PATH, false)]
		private static void AddToSceneInEditor()
		{
			AFPSCounter counter = FindObjectOfType<AFPSCounter>();
			if (counter != null)
			{
				if (counter.IsPlacedCorrectly())
				{
					if (UnityEditor.EditorUtility.DisplayDialog("Remove " + COMPONENT_NAME + "?",
						COMPONENT_NAME + " already exists in scene and placed correctly. Do you wish to remove it?", "Yes", "No"))
					{
						DestroyInEditorImmediate(counter);
					}
				}
				else
				{
					if (counter.MayBePlacedHere())
					{
						int dialogResult = UnityEditor.EditorUtility.DisplayDialogComplex("Fix existing Game Object to work with " +
							COMPONENT_NAME + "?",
							COMPONENT_NAME + " already exists in scene and placed on Game Object \"" +
							counter.name + "\" with minor errors.\nDo you wish to let plugin fix and use this Game Object further? " +
							"Press Delete to remove plugin from scene at all.", "Fix", "Delete", "Cancel");

						switch (dialogResult)
						{
							case 0:
								counter.FixCurrentGameObject();
								break;
							case 1:
								DestroyInEditorImmediate(counter);
								break;
						}
					}
					else
					{
						int dialogResult = UnityEditor.EditorUtility.DisplayDialogComplex("Move existing " + COMPONENT_NAME +
							" to own Game Object?",
							"Looks like " + COMPONENT_NAME + " already exists in scene and placed incorrectly on Game Object \"" +
							counter.name + "\".\nDo you wish to let plugin move itself onto separate correct Game Object \"" +
							COMPONENT_NAME + "\"? Press Delete to remove plugin from scene at all.", "Move", "Delete", "Cancel");

						switch (dialogResult)
						{
							case 0:
								AFPSCounter newCounter = CreateInScene();
								UnityEditor.EditorUtility.CopySerialized(counter, newCounter);
								break;
						}
						DestroyInEditorImmediate(counter);
					}
				}
			}
			else
			{
				CreateInScene();
				UnityEditor.EditorUtility.DisplayDialog("Advanced FPS Counter added!", "Advanced FPS Counter successfully added to the object \"" + COMPONENT_NAME + "\"", "OK");
			}

		}

		private bool MayBePlacedHere()
		{
			return (transform.childCount == 0 &&
					transform.parent == null);
		}

		private bool IsPlacedCorrectly()
		{
			return (transform.position == Vector3.zero &&
					transform.rotation == Quaternion.identity &&
					transform.localScale == Vector3.one &&
					gameObject.isStatic == false &&
					MayBePlacedHere());
		}

		private void FixCurrentGameObject()
		{
			gameObject.name = COMPONENT_NAME;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			tag = "Untagged";
			gameObject.layer = LayerMask.NameToLayer("UI");
			gameObject.isStatic = false;
		}

		private static void DestroyInEditorImmediate(AFPSCounter component)
		{
			if (component.transform.childCount == 0 && component.GetComponentsInChildren<Component>(true).Length <= 2)
			{
				DestroyImmediate(component.gameObject);
			}
			else
			{
				DestroyImmediate(component);
			}
		}
#endif
		#endregion
	}
}