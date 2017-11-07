using CodeStage.AdvancedFPSCounter.Editor.UI;
using CodeStage.AdvancedFPSCounter.Labels;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter
{
	[CustomEditor(typeof(AFPSCounter))]
	internal class AFPSCounterEditor: UnityEditor.Editor
	{
		private AFPSCounter me;

		private SerializedProperty operationMode;
		private SerializedProperty fpsGroupFoldout;
		private SerializedProperty fps;
		private SerializedProperty fpsEnabled;
		private SerializedProperty fpsAnchor;
		private SerializedProperty fpsInterval;
		private SerializedProperty fpsMilliseconds;
		private SerializedProperty fpsAverage;
		private SerializedProperty fpsMinMax;
		private SerializedProperty fpsMinMaxNewLine;
		private SerializedProperty fpsResetMinMaxOnNewScene;
		private SerializedProperty fpsMinMaxIntervalsToSkip;
		private SerializedProperty fpsAverageSamples;
		private SerializedProperty fpsResetAverageOnNewScene;
		private SerializedProperty fpsWarningLevelValue;
		private SerializedProperty fpsCriticalLevelValue;
		private SerializedProperty fpsColor;
		private SerializedProperty fpsColorWarning;
		private SerializedProperty fpsColorCritical;

		private SerializedProperty memoryGroupFoldout;
		private SerializedProperty memory;
		private SerializedProperty memoryEnabled;
		private SerializedProperty memoryAnchor;
		private SerializedProperty memoryInterval;
		private SerializedProperty memoryPrecise;
		private SerializedProperty memoryColor;
		private SerializedProperty memoryTotal;
		private SerializedProperty memoryAllocated;
		private SerializedProperty memoryMonoUsage;

		private SerializedProperty deviceGroupFoldout;
		private SerializedProperty device;
		private SerializedProperty deviceEnabled;
		private SerializedProperty deviceAnchor;
		private SerializedProperty deviceColor;
		private SerializedProperty deviceCpuModel;
		private SerializedProperty deviceGpuModel;
		private SerializedProperty deviceRamSize;
		private SerializedProperty deviceScreenData;

		private SerializedProperty lookAndFeelFoldout;
		private SerializedProperty scaleFactor;
		private SerializedProperty labelsFont;
		private SerializedProperty fontSize;
		private SerializedProperty lineSpacing;
		private SerializedProperty countersSpacing;
		private SerializedProperty paddingOffset;

		private SerializedProperty advancedFoldout;
		private SerializedProperty sortingOrder;

		private SerializedProperty hotKey;
		private SerializedProperty keepAlive;
		private SerializedProperty forceFrameRate;
		private SerializedProperty forcedFrameRate;

		private LabelAnchor groupAnchor = LabelAnchor.UpperLeft;

		public void OnEnable()
		{
			me = (target as AFPSCounter);

			operationMode = serializedObject.FindProperty("operationMode");

			hotKey = serializedObject.FindProperty("hotKey");
			keepAlive = serializedObject.FindProperty("keepAlive");
			forceFrameRate = serializedObject.FindProperty("forceFrameRate");
			forcedFrameRate = serializedObject.FindProperty("forcedFrameRate");

			lookAndFeelFoldout = serializedObject.FindProperty("lookAndFeelFoldout");
			scaleFactor = serializedObject.FindProperty("scaleFactor");
			labelsFont = serializedObject.FindProperty("labelsFont");
			fontSize = serializedObject.FindProperty("fontSize");
			lineSpacing = serializedObject.FindProperty("lineSpacing");
			countersSpacing = serializedObject.FindProperty("countersSpacing");
			paddingOffset = serializedObject.FindProperty("paddingOffset");

			advancedFoldout = serializedObject.FindProperty("advancedFoldout");
			sortingOrder = serializedObject.FindProperty("sortingOrder");

			fpsGroupFoldout = serializedObject.FindProperty("fpsGroupFoldout");
			fps = serializedObject.FindProperty("fpsCounter");
			fpsEnabled = fps.FindPropertyRelative("enabled");
			fpsInterval = fps.FindPropertyRelative("updateInterval");
			fpsAnchor = fps.FindPropertyRelative("anchor");
			fpsMilliseconds = fps.FindPropertyRelative("milliseconds");
			fpsAverage = fps.FindPropertyRelative("average");
			fpsMinMax = fps.FindPropertyRelative("minMax");
			fpsMinMaxNewLine = fps.FindPropertyRelative("minMaxNewLine");
			fpsResetMinMaxOnNewScene = fps.FindPropertyRelative("resetMinMaxOnNewScene");
			fpsMinMaxIntervalsToSkip = fps.FindPropertyRelative("minMaxIntervalsToSkip");
			fpsAverageSamples = fps.FindPropertyRelative("averageSamples");
			fpsResetAverageOnNewScene = fps.FindPropertyRelative("resetAverageOnNewScene");
			fpsWarningLevelValue = fps.FindPropertyRelative("warningLevelValue");
			fpsCriticalLevelValue = fps.FindPropertyRelative("criticalLevelValue");
			fpsColor = fps.FindPropertyRelative("color");
			fpsColorWarning = fps.FindPropertyRelative("colorWarning");
			fpsColorCritical = fps.FindPropertyRelative("colorCritical");

			memoryGroupFoldout = serializedObject.FindProperty("memoryGroupFoldout");
			memory = serializedObject.FindProperty("memoryCounter");
			memoryEnabled = memory.FindPropertyRelative("enabled");
			memoryInterval = memory.FindPropertyRelative("updateInterval");
			memoryAnchor = memory.FindPropertyRelative("anchor");
			memoryPrecise = memory.FindPropertyRelative("precise");
			memoryColor = memory.FindPropertyRelative("color");
			memoryTotal = memory.FindPropertyRelative("total");
			memoryAllocated = memory.FindPropertyRelative("allocated");
			memoryMonoUsage = memory.FindPropertyRelative("monoUsage");

			deviceGroupFoldout = serializedObject.FindProperty("deviceGroupFoldout");
			device = serializedObject.FindProperty("deviceInfoCounter");
			deviceEnabled = device.FindPropertyRelative("enabled");
			deviceAnchor = device.FindPropertyRelative("anchor");
			deviceColor = device.FindPropertyRelative("color");
			deviceCpuModel = device.FindPropertyRelative("cpuModel");
			deviceGpuModel = device.FindPropertyRelative("gpuModel");
			deviceRamSize = device.FindPropertyRelative("ramSize");
			deviceScreenData = device.FindPropertyRelative("screenData");
		}

		public override void OnInspectorGUI()
		{
			if (me == null) return;
			serializedObject.Update();

			UIUtils.SetupStyles();

			GUILayout.Space(5);

			UIUtils.DrawProperty(operationMode, () => me.OperationMode = (OperationMode)operationMode.enumValueIndex);

			EditorGUILayout.PropertyField(hotKey);
			EditorGUILayout.PropertyField(keepAlive);

			using (new UIUtils.HorizontalBlock(GUILayout.ExpandWidth(true)))
			{
				UIUtils.DrawProperty(forceFrameRate, "Force FPS", () => me.ForceFrameRate = forceFrameRate.boolValue);
				UIUtils.DrawProperty(forcedFrameRate, GUIContent.none, () => me.ForcedFrameRate = forcedFrameRate.intValue);
			}

            if (UIUtils.Foldout(lookAndFeelFoldout, "Look & Feel"))
			{
				UIUtils.Indent();
				UIUtils.DrawProperty(scaleFactor, () => me.ScaleFactor = scaleFactor.floatValue);
				UIUtils.DrawProperty(labelsFont, () => me.LabelsFont = (Font)labelsFont.objectReferenceValue);
				UIUtils.DrawProperty(fontSize, () => me.FontSize = fontSize.intValue);
				UIUtils.DrawProperty(lineSpacing, () => me.LineSpacing = lineSpacing.floatValue);
				UIUtils.DrawProperty(countersSpacing, () => me.CountersSpacing = countersSpacing.intValue);
				UIUtils.DrawProperty(paddingOffset, () => me.PaddingOffset = paddingOffset.vector2Value);

				UIUtils.Header("Service Commands");

				using (new UIUtils.HorizontalBlock())
				{
					groupAnchor = (LabelAnchor)EditorGUILayout.EnumPopup(
						new GUIContent("Move All To", "Use to explicitly move all counters to the specified anchor label.\n" +
					                                  "Select anchor and press Apply."), groupAnchor);

					if (GUILayout.Button(new GUIContent("Apply", "Press to move all counters to the selected anchor label."),
					                     GUILayout.Width(45)))
					{
						me.fpsCounter.Anchor = groupAnchor;
						fpsAnchor.enumValueIndex = (int)groupAnchor;

						me.memoryCounter.Anchor = groupAnchor;
						memoryAnchor.enumValueIndex = (int)groupAnchor;

						me.deviceInfoCounter.Anchor = groupAnchor;
						deviceAnchor.enumValueIndex = (int)groupAnchor;
					}
				}
				UIUtils.Unindent();
			}

			if (UIUtils.Foldout(advancedFoldout, "Advanced Settings"))
			{
				UIUtils.Indent();
				UIUtils.DrawProperty(sortingOrder, () => me.SortingOrder = sortingOrder.intValue);
				UIUtils.Unindent();
			}

			GUI.enabled = UIUtils.ToggleFoldout(fpsEnabled, fpsGroupFoldout, "FPS Counter");
			me.fpsCounter.Enabled = fpsEnabled.boolValue;

			if (fpsGroupFoldout.boolValue)
			{
				UIUtils.Indent();
				UIUtils.DrawProperty(fpsInterval, "Interval", () => me.fpsCounter.UpdateInterval = fpsInterval.floatValue);
				UIUtils.DrawProperty(fpsAnchor, () => me.fpsCounter.Anchor = (LabelAnchor)fpsAnchor.enumValueIndex);
                UIUtils.Separator(5);

				float minVal = fpsCriticalLevelValue.intValue;
				float maxVal = fpsWarningLevelValue.intValue;

				EditorGUILayout.MinMaxSlider(new GUIContent("Colors Range", 
					"This range will be used to apply colors below on specific FPS:\n" +
					"Critical: 0 - min\n" +
					"Warning: min+1 - max-1\n" +
					"Normal: max+"), 
					ref minVal, ref maxVal, 1, 60);

				fpsCriticalLevelValue.intValue = (int)minVal;
				fpsWarningLevelValue.intValue = (int)maxVal;

				using (new UIUtils.HorizontalBlock())
				{
					UIUtils.DrawProperty(fpsColor, "Normal", () => me.fpsCounter.Color = fpsColor.colorValue);
					GUILayout.Label(maxVal + "+ FPS", GUILayout.Width(75));
				}

				using (new UIUtils.HorizontalBlock())
				{
					UIUtils.DrawProperty(fpsColorWarning, "Warning", () => me.fpsCounter.ColorWarning = fpsColorWarning.colorValue);
					GUILayout.Label((minVal + 1) + " - " + (maxVal - 1) + " FPS", GUILayout.Width(75));
				}

				using (new UIUtils.HorizontalBlock())
				{
					UIUtils.DrawProperty(fpsColorCritical, "Critical", () => me.fpsCounter.ColorCritical = fpsColorCritical.colorValue);
					GUILayout.Label("0 - " + minVal + " FPS", GUILayout.Width(75));
				}

				UIUtils.Separator(5);
				UIUtils.DrawProperty(fpsMilliseconds, () => me.fpsCounter.Milliseconds = fpsMilliseconds.boolValue);
				UIUtils.DrawProperty(fpsAverage, "Average FPS", () => me.fpsCounter.Average = fpsAverage.boolValue);

				if (fpsAverage.boolValue)
				{
					UIUtils.Indent();

					UIUtils.DrawProperty(fpsAverageSamples, "Samples", () => me.fpsCounter.AverageSamples = fpsAverageSamples.intValue);
					using (new UIUtils.HorizontalBlock())
					{
						EditorGUILayout.PropertyField(fpsResetAverageOnNewScene, new GUIContent("Auto Reset"));
						if (GUILayout.Button("Reset Now"))
						{
							me.fpsCounter.ResetAverage();
						}
					}

					UIUtils.Unindent();
				}

				UIUtils.DrawProperty(fpsMinMax, "MinMax FPS", () => me.fpsCounter.MinMax = fpsMinMax.boolValue);

				if (fpsMinMax.boolValue)
				{
					UIUtils.Indent();
					EditorGUILayout.PropertyField(fpsMinMaxIntervalsToSkip, new GUIContent("Delay"));
					UIUtils.DrawProperty(fpsMinMaxNewLine, "New Line", () => me.fpsCounter.MinMaxNewLine = fpsMinMaxNewLine.boolValue);
					using (new UIUtils.HorizontalBlock())
					{
						EditorGUILayout.PropertyField(fpsResetMinMaxOnNewScene, new GUIContent("Auto Reset"));
						if (GUILayout.Button("Reset Now"))
						{
							me.fpsCounter.ResetMinMax();
						}
					}
					UIUtils.Unindent();
				}
				UIUtils.Unindent();
			}
			GUI.enabled = true;

			GUI.enabled = UIUtils.ToggleFoldout(memoryEnabled, memoryGroupFoldout, "Memory Counter");
			me.memoryCounter.Enabled = memoryEnabled.boolValue;
			if (memoryGroupFoldout.boolValue)
			{
				UIUtils.Indent();
				UIUtils.DrawProperty(memoryInterval, "Interval", () => me.memoryCounter.UpdateInterval = memoryInterval.floatValue);
				UIUtils.DrawProperty(memoryAnchor, () => me.memoryCounter.Anchor = (LabelAnchor)memoryAnchor.enumValueIndex);
				UIUtils.DrawProperty(memoryColor, () => me.memoryCounter.Color = memoryColor.colorValue);
				EditorGUILayout.Space();
				UIUtils.DrawProperty(memoryPrecise, () => me.memoryCounter.Precise = memoryPrecise.boolValue);
				UIUtils.DrawProperty(memoryTotal, () => me.memoryCounter.Total = memoryTotal.boolValue);
				UIUtils.DrawProperty(memoryAllocated, () => me.memoryCounter.Allocated = memoryAllocated.boolValue);
				UIUtils.DrawProperty(memoryMonoUsage, "Mono", () => me.memoryCounter.MonoUsage = memoryMonoUsage.boolValue);
				UIUtils.Unindent();
            }
			GUI.enabled = true;

			GUI.enabled = UIUtils.ToggleFoldout(deviceEnabled, deviceGroupFoldout, "Device Information");
			me.deviceInfoCounter.Enabled = deviceEnabled.boolValue;
			if (deviceGroupFoldout.boolValue)
			{
				UIUtils.Indent();
				UIUtils.DrawProperty(deviceAnchor, () => me.deviceInfoCounter.Anchor = (LabelAnchor)deviceAnchor.intValue);
				UIUtils.DrawProperty(deviceColor, () => me.deviceInfoCounter.Color = deviceColor.colorValue);
				EditorGUILayout.Space();
				UIUtils.DrawProperty(deviceCpuModel, "CPU", () => me.deviceInfoCounter.CpuModel = deviceCpuModel.boolValue);
				UIUtils.DrawProperty(deviceGpuModel, "GPU", () => me.deviceInfoCounter.GpuModel = deviceGpuModel.boolValue);
				UIUtils.DrawProperty(deviceRamSize, "RAM", () => me.deviceInfoCounter.RamSize = deviceRamSize.boolValue);
				UIUtils.DrawProperty(deviceScreenData, "Screen", () => me.deviceInfoCounter.ScreenData = deviceScreenData.boolValue);
				UIUtils.Unindent();
			}
			GUI.enabled = true;
			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}
	}
}