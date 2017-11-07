using CodeStage.AdvancedFPSCounter.CountersData;
using CodeStage.AdvancedFPSCounter.Labels;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter
{
	public class APITester : MonoBehaviour
	{
		private int selectedTab;
		private readonly string[] tabs = {"Common", "FPS Counter", "Memory Counter", "Device info"};

		private FPSLevel currentFPSLevel = FPSLevel.Normal;

		private void Start()
		{
			// will add AFPSCounter to the scene if it not exists
			// you don't need to call it if you already have AFPSCounter in the scene
			AFPSCounter.AddToScene();
			AFPSCounter.Instance.fpsCounter.OnFpsLevelChange += OnFPSLevelChanged;
		}

		private void OnFPSLevelChanged(FPSLevel newLevel)
		{
			currentFPSLevel = newLevel;
		}

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(40, 110, Screen.width-80, Screen.height - 140));

			GUIStyle richStyle = new GUIStyle(GUI.skin.label);
			richStyle.richText = true;

			GUIStyle centeredStyle = new GUIStyle(richStyle);
			centeredStyle.alignment = TextAnchor.UpperCenter;

			GUILayout.Label("<b>Public API usage examples</b>", centeredStyle);

			selectedTab = GUILayout.Toolbar(selectedTab, tabs);

			if (selectedTab == 0)
			{
				GUILayout.Space(10);

				GUILayout.BeginHorizontal();
				GUILayout.Label("Operation Mode:", GUILayout.MaxWidth(100));
				int operationMode = (int)AFPSCounter.Instance.OperationMode;
				operationMode = GUILayout.Toolbar(operationMode, new[]
				{
					OperationMode.Disabled.ToString(),
					OperationMode.Background.ToString(),
					OperationMode.Normal.ToString()
				});

				if (GUI.changed)
					AFPSCounter.Instance.OperationMode = (OperationMode)operationMode;
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Hot Key:", GUILayout.MaxWidth(100));
				int hotKeyIndex;

				if (AFPSCounter.Instance.hotKey == KeyCode.BackQuote)
				{
					hotKeyIndex = 1;
				}
				else
				{
					hotKeyIndex = (int)AFPSCounter.Instance.hotKey;
				}

				hotKeyIndex = GUILayout.Toolbar(hotKeyIndex, new[] { "None (disabled)", "BackQoute (`)"});
				AFPSCounter.Instance.hotKey = hotKeyIndex == 1 ? KeyCode.BackQuote : KeyCode.None;
				GUILayout.EndHorizontal();

				AFPSCounter.Instance.keepAlive = GUILayout.Toggle(AFPSCounter.Instance.keepAlive, "Keep Alive");

				GUILayout.BeginHorizontal();
				AFPSCounter.Instance.ForceFrameRate = GUILayout.Toggle(AFPSCounter.Instance.ForceFrameRate, "Force FPS", GUILayout.Width(100));
				AFPSCounter.Instance.ForcedFrameRate = (int)SliderLabel(AFPSCounter.Instance.ForcedFrameRate, -1, 100);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Scale", GUILayout.Width(100));
				AFPSCounter.Instance.ScaleFactor = SliderLabel(AFPSCounter.Instance.ScaleFactor, 0f, 30f);
				GUILayout.EndHorizontal();

				float offsetX = AFPSCounter.Instance.PaddingOffset.x;
				float offsetY = AFPSCounter.Instance.PaddingOffset.y;

				GUILayout.BeginHorizontal();
				GUILayout.Label("Padding", GUILayout.Width(100));
				GUILayout.Label("X: ", GUILayout.Width(20));
				offsetX = (int)SliderLabel(offsetX, 0, 100);
				GUILayout.Space(30);
                GUILayout.Label("Y:", GUILayout.Width(20));
				offsetY = (int)SliderLabel(offsetY, 0, 100);
				GUILayout.EndHorizontal();

				AFPSCounter.Instance.PaddingOffset = new Vector2(offsetX, offsetY);

				GUILayout.BeginHorizontal();
				GUILayout.Label("Font Size", GUILayout.Width(100));
				AFPSCounter.Instance.FontSize = (int)SliderLabel(AFPSCounter.Instance.FontSize, 0, 100);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Line spacing", GUILayout.Width(100));
				AFPSCounter.Instance.LineSpacing = SliderLabel(AFPSCounter.Instance.LineSpacing, 0f, 10f);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Counters spacing", GUILayout.Width(120));
				AFPSCounter.Instance.CountersSpacing = (int)SliderLabel(AFPSCounter.Instance.CountersSpacing, 0, 10);
				GUILayout.EndHorizontal();
			}
			else if (selectedTab == 1)
			{
				GUILayout.Space(10);
				AFPSCounter.Instance.fpsCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Enabled, "Enabled");
				GUILayout.Space(10);
				AFPSCounter.Instance.fpsCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.fpsCounter.Anchor, new[] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
				GUILayout.Space(10);

				GUILayout.BeginHorizontal();
				GUILayout.Label("Update Interval", GUILayout.Width(100));
				AFPSCounter.Instance.fpsCounter.UpdateInterval = SliderLabel(AFPSCounter.Instance.fpsCounter.UpdateInterval, 0.1f, 10f);
				GUILayout.EndHorizontal();

				AFPSCounter.Instance.fpsCounter.Milliseconds = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Milliseconds, "Milliseconds");
				GUILayout.BeginHorizontal();
				AFPSCounter.Instance.fpsCounter.Average = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Average, "Average FPS", GUILayout.Width(100));

				if (AFPSCounter.Instance.fpsCounter.Average)
				{
					GUILayout.Label("Samples", GUILayout.ExpandWidth(false));
					AFPSCounter.Instance.fpsCounter.AverageSamples = (int)SliderLabel(AFPSCounter.Instance.fpsCounter.AverageSamples, 0, 100);
					GUILayout.Space(10);
					AFPSCounter.Instance.fpsCounter.resetAverageOnNewScene = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.resetAverageOnNewScene, "Reset Average On Load", GUILayout.ExpandWidth(false));
					if (GUILayout.Button("Reset now!", GUILayout.ExpandWidth(false)))
					{
						AFPSCounter.Instance.fpsCounter.ResetAverage();
					}
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				AFPSCounter.Instance.fpsCounter.MinMax = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.MinMax, "MinMax FPS", GUILayout.Width(100));

				if (AFPSCounter.Instance.fpsCounter.MinMax)
				{
					GUILayout.Label("Delay", GUILayout.ExpandWidth(false));
					AFPSCounter.Instance.fpsCounter.minMaxIntervalsToSkip = (int)SliderLabel(AFPSCounter.Instance.fpsCounter.minMaxIntervalsToSkip, 0, 10);
					GUILayout.Space(10);
					AFPSCounter.Instance.fpsCounter.MinMaxNewLine = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.MinMaxNewLine, "On new line");
					
					AFPSCounter.Instance.fpsCounter.resetMinMaxOnNewScene = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.resetMinMaxOnNewScene, "Reset MinMax On Load", GUILayout.ExpandWidth(false));
					if (GUILayout.Button("Reset now!", GUILayout.ExpandWidth(false)))
					{
						AFPSCounter.Instance.fpsCounter.ResetMinMax();
					}
				}
				GUILayout.EndHorizontal();
			}
			else if (selectedTab == 2)
			{
				GUILayout.Space(10);
				AFPSCounter.Instance.memoryCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Enabled, "Enabled");
				GUILayout.Space(10);
				AFPSCounter.Instance.memoryCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.memoryCounter.Anchor, new[] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Update Interval", GUILayout.Width(100));
				AFPSCounter.Instance.memoryCounter.UpdateInterval = SliderLabel(AFPSCounter.Instance.memoryCounter.UpdateInterval, 0.1f, 10f);
				GUILayout.EndHorizontal();
				
				AFPSCounter.Instance.memoryCounter.Precise = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Precise, "Precise (uses more system resources)");
				AFPSCounter.Instance.memoryCounter.Total = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Total, "Show total reserved memory size");
				AFPSCounter.Instance.memoryCounter.Allocated = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Allocated, "Show allocated memory size");
				AFPSCounter.Instance.memoryCounter.MonoUsage = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.MonoUsage, "Show mono memory usage");
			}
			else if (selectedTab == 3)
			{
				GUILayout.Space(10);
				AFPSCounter.Instance.deviceInfoCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.Enabled, "Enabled");
				GUILayout.Space(10);
				AFPSCounter.Instance.deviceInfoCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.deviceInfoCounter.Anchor, new[] { "UpperLeft", "UpperRight", "LowerLeft", "LowerRight", "UpperCenter", "LowerCenter" });
				GUILayout.Space(10);
				AFPSCounter.Instance.deviceInfoCounter.CpuModel = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.CpuModel, "Show CPU model and maximum threads count");
				AFPSCounter.Instance.deviceInfoCounter.GpuModel = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuModel, "Show GPU model and total VRAM count");
				AFPSCounter.Instance.deviceInfoCounter.RamSize = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.RamSize, "Show total RAM size");
				AFPSCounter.Instance.deviceInfoCounter.ScreenData = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.ScreenData, "Show resolution, window size and DPI (if possible)");
			}

			GUILayout.Label("<b>Raw counters values</b> (read using API)", richStyle);

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			GUILayout.Label("  FPS: " + AFPSCounter.Instance.fpsCounter.LastValue +
							"  [" + AFPSCounter.Instance.fpsCounter.LastMillisecondsValue + " MS]" +
							"  AVG: " + AFPSCounter.Instance.fpsCounter.LastAverageValue + 
							"\n  MIN: " + AFPSCounter.Instance.fpsCounter.LastMinimumValue +
							"  MAX: " + AFPSCounter.Instance.fpsCounter.LastMaximumValue +
							"\n  Level (direct / callback): " + AFPSCounter.Instance.fpsCounter.CurrentFpsLevel + " / " + currentFPSLevel);

			if (AFPSCounter.Instance.memoryCounter.Precise)
			{
				GUILayout.Label("  Memory (Total, Allocated, Mono):\n  " +
								AFPSCounter.Instance.memoryCounter.LastTotalValue / (float)MemoryCounterData.MEMORY_DIVIDER + ", " +
								AFPSCounter.Instance.memoryCounter.LastAllocatedValue / (float)MemoryCounterData.MEMORY_DIVIDER + ", " +
								AFPSCounter.Instance.memoryCounter.LastMonoValue / (float)MemoryCounterData.MEMORY_DIVIDER);
			}
			else
			{
				GUILayout.Label("  Memory (Total, Allocated, Mono):\n  " +
								AFPSCounter.Instance.memoryCounter.LastTotalValue + ", " +
								AFPSCounter.Instance.memoryCounter.LastAllocatedValue + ", " +
								AFPSCounter.Instance.memoryCounter.LastMonoValue);
			}
			GUILayout.EndVertical();
			if (AFPSCounter.Instance.deviceInfoCounter.Enabled) GUILayout.Label(AFPSCounter.Instance.deviceInfoCounter.LastValue);
			GUILayout.EndHorizontal();

			GUILayout.EndArea();
		}

		private static float SliderLabel(float sliderValue, float sliderMinValue, float sliderMaxValue)
		{
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Space(8);
			sliderValue = GUILayout.HorizontalSlider(sliderValue, sliderMinValue, sliderMaxValue);
			GUILayout.EndVertical();
            GUILayout.Space(10);
			GUILayout.Label(string.Format("{0:F2}", sliderValue), GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();

			return sliderValue;
		}
	}
}
