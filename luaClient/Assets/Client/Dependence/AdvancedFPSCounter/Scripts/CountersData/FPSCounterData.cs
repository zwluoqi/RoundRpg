using System;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	/// <summary>
	/// Shows frames per second counter.
	/// </summary>
	[AddComponentMenu("")]
	[Serializable]
	public class FPSCounterData: BaseCounterData
	{
		// ----------------------------------------------------------------------------
		// constants
		// ----------------------------------------------------------------------------

		private const string COROUTINE_NAME = "UpdateFPSCounter";
		private const string FPS_TEXT_START = "<color=#{0}><b>FPS: ";
		private const string FPS_TEXT_END = "</b></color>";
		private const string MS_TEXT_START = " <color=#{0}><b>[";
		private const string MS_TEXT_END = " MS]</b></color>";
		private const string MIN_TEXT_START = "<color=#{0}><b>MIN: ";
		private const string MIN_TEXT_END = "</b></color> ";
		private const string MAX_TEXT_START = "<color=#{0}><b>MAX: ";
		private const string MAX_TEXT_END = "</b></color>";
		private const string AVG_TEXT_START = " <color=#{0}><b>AVG: ";
		private const string AVG_TEXT_END = "</b></color>";

		// ----------------------------------------------------------------------------
		// public fields exposed to the inspector
		// ----------------------------------------------------------------------------

		/// <summary>
		/// If FPS will drop below this value, #ColorWarning will be used for counter text.
		/// </summary>
		public int warningLevelValue = 50;

		/// <summary>
		/// If FPS will be equal or less this value, #ColorCritical will be used for counter text.
		/// </summary>
		public int criticalLevelValue = 20;

		/// <summary>
		/// Average FPS counter accumulative data will be reset on new scene load if enabled.
		/// \sa averageSamples, lastAverageValue
		/// </summary>
		[Tooltip("Average FPS counter accumulative data will be reset on new scene load if enabled.")]
		public bool resetAverageOnNewScene = false;

		/// <summary>
		/// Minimum and maximum FPS readings will be reset on new scene load if enabled.
		/// \sa lastMinimumValue, lastMaximumValue
		/// </summary>
		[Tooltip("Minimum and maximum FPS readouts will be reset on new scene load if enabled")]
		public bool resetMinMaxOnNewScene = false;

		/// <summary>
		/// Amount of update intervals to skip before recording minimum and maximum FPS. Use it to skip initialization performance spikes and drops.
		/// \sa lastMinimumValue, lastMaximumValue
		/// </summary>
		[Tooltip("Amount of update intervals to skip before recording minimum and maximum FPS.\n" +
		         "Use it to skip initialization performance spikes and drops.")]
		[Range(0, 10)]
		public int minMaxIntervalsToSkip = 3;

		// ----------------------------------------------------------------------------
		// events
		// ----------------------------------------------------------------------------

		/// <summary>
		/// Event to let you react on FPS level change.
		/// \sa FPSLevel, CurrentFpsLevel
		/// </summary>
		public event Action<FPSLevel> OnFpsLevelChange;

		// ----------------------------------------------------------------------------
		// internal fields
		// ----------------------------------------------------------------------------

		internal float newValue;

		// ----------------------------------------------------------------------------
		// private fields
		// ----------------------------------------------------------------------------

		private string colorCachedMs;
		private string colorCachedMin;
		private string colorCachedMax;
		private string colorCachedAvg;

		private string colorWarningCached;
		private string colorWarningCachedMs;
		private string colorWarningCachedMin;
		private string colorWarningCachedMax;
		private string colorWarningCachedAvg;

		private string colorCriticalCached;
		private string colorCriticalCachedMs;
		private string colorCriticalCachedMin;
		private string colorCriticalCachedMax;
		private string colorCriticalCachedAvg;

		private bool inited;

		private int currentAverageSamples;
		private float currentAverageRaw;
		private float[] accumulatedAverageSamples;

		private int minMaxIntervalsSkipped;

		// ----------------------------------------------------------------------------
		// properties exposed to the inspector
		// ----------------------------------------------------------------------------

		#region UpdateInterval
		[Tooltip("Update interval in seconds.")]
		[Range(0.1f, 10f)]
		[SerializeField]
		private float updateInterval = 0.5f;

		/// <summary>
		/// Update interval in seconds.
		/// </summary>
		public float UpdateInterval
		{
			get { return updateInterval; }
			set
			{
				if (Math.Abs(updateInterval - value) < 0.001f || !Application.isPlaying) return;

				updateInterval = value;
				if (!enabled) return;

				RestartCoroutine();
			}
		}
		#endregion

		#region Milliseconds
		[Tooltip("Shows time in milliseconds spent to render 1 frame.")]
		[SerializeField]
		private bool milliseconds = true;

		/// <summary>
		/// Shows time in milliseconds spent to render 1 frame.
		/// \sa lastMillisecondsValue
		/// </summary>
		public bool Milliseconds
		{
			get { return milliseconds; }
			set
			{
				if (milliseconds == value || !Application.isPlaying) return;
				milliseconds = value;
				if (!milliseconds) LastMillisecondsValue = 0f;
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		#region Average
		[Tooltip("Shows Average FPS calculated from specified Samples amount or since game / scene start, " +
		         "depending on Samples value and 'Reset On Load' toggle.")]
		[SerializeField]
		private bool average = true;

		/// <summary>
		/// Shows Average FPS calculated from specified #AverageSamples amount or since game / scene start, depending on #AverageSamples value and #resetAverageOnNewScene toggle.
		/// \sa lastAverageValue
		/// </summary>
		public bool Average
		{
			get { return average; }
			set
			{
				if (average == value || !Application.isPlaying) return;
				average = value;
				if (!average) ResetAverage();
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		#region AverageSamples
		[Tooltip("Amount of last samples to get average from. Set 0 to get average from all samples since startup or level load.\n" +
		         "One Sample recorded per one Interval.")]
		[Range(0, 100)]
		[SerializeField]
		private int averageSamples = 50;

		/// <summary>
		/// Amount of last samples to get average from. Set 0 to get average from all samples since startup or level load. One Sample recorded per one #UpdateInterval.
		/// \sa resetAverageOnNewScene
		/// </summary>
		public int AverageSamples
		{
			get { return averageSamples; }
			set
			{
				if (averageSamples == value || !Application.isPlaying) return;
				averageSamples = value;
				if (!enabled) return;

				if (averageSamples > 0)
				{
					if (accumulatedAverageSamples == null)
					{
						accumulatedAverageSamples = new float[averageSamples];
					}
					else if (accumulatedAverageSamples.Length != averageSamples)
					{
						Array.Resize(ref accumulatedAverageSamples, averageSamples);
					}
				}
				else
				{
					accumulatedAverageSamples = null;
				}
				ResetAverage();
				Refresh();
			}
		}
		#endregion

		#region MinMax
		[Tooltip("Shows minimum and maximum FPS readouts since game / scene start, depending on 'Reset On Load' toggle.")]
		[SerializeField]
		private bool minMax;

		/// <summary>
		/// Shows minimum and maximum FPS readouts since game / scene start, depending on #resetMinMaxOnNewScene toggle.
		/// </summary>
		public bool MinMax
		{
			get { return minMax; }
			set
			{
				if (minMax == value || !Application.isPlaying) return;
				minMax = value;
				if (!minMax) ResetMinMax();
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		#region MinMaxNewLine
		[Tooltip("Controls placing Min Max on the new line.")]
		[SerializeField]
		private bool minMaxNewLine;

		/// <summary>
		/// Controls placing Min Max on the new line.
		/// \sa minMax
		/// </summary>
		public bool MinMaxNewLine
		{
			get { return minMaxNewLine; }
			set
			{
				if (minMaxNewLine == value || !Application.isPlaying) return;
				minMaxNewLine = value;
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		#region ColorWarning
		[Tooltip("Color of the FPS counter while FPS is between Critical and Warning levels.")]
		[SerializeField]
		private Color colorWarning = new Color32(236, 224, 88, 255);

		/// <summary>
		/// Color of the FPS counter while FPS is between #criticalLevelValue and #warningLevelValue levels.
		/// </summary>
		public Color ColorWarning
		{
			get { return colorWarning; }
			set
			{
				if (colorWarning == value || !Application.isPlaying) return;
				colorWarning = value;
				if (!enabled) return;

				CacheWarningColor();

				Refresh();
			}
		}
		#endregion

		#region ColorCritical
		[Tooltip("Color of the FPS counter while FPS is below Critical level.")]
		[SerializeField]
		private Color colorCritical = new Color32(249, 91, 91, 255);

		/// <summary>
		/// Color of the FPS counter while FPS is below #criticalLevelValue.
		/// </summary>
		public Color ColorCritical
		{
			get { return colorCritical; }
			set
			{
				if (colorCritical == value || !Application.isPlaying) return;
				colorCritical = value;
				if (!enabled) return;

				CacheCriticalColor();

				Refresh();
			}
		}
		#endregion

		// ----------------------------------------------------------------------------
		// properties only accessible from code
		// ----------------------------------------------------------------------------

		/// <summary>
		/// Last calculated FPS value.
		/// </summary>
		public int LastValue { get; private set; }

		/// <summary>
		/// Last calculated Milliseconds value.
		/// </summary>
		public float LastMillisecondsValue { get; private set; }

		/// <summary>
		/// Last calculated Average FPS value.
		/// \sa averageSamples, resetAverageOnNewScene
		/// </summary>
		public int LastAverageValue { get; private set; }

		/// <summary>
		/// Last minimum FPS value.
		/// \sa resetMinMaxOnNewScene
		/// </summary>
		public int LastMinimumValue { get; private set; }

		/// <summary>
		/// Last maximum FPS value.
		/// \sa resetMinMaxOnNewScene
		/// </summary>
		public int LastMaximumValue { get; private set; }

		/// <summary>
		/// Current FPS level.
		/// \sa FPSLevel, onFPSLevelChange
		/// </summary>
		public FPSLevel CurrentFpsLevel { get; private set; }

		// ----------------------------------------------------------------------------
		// constructor
		// ----------------------------------------------------------------------------

		internal FPSCounterData()
		{
			color = new Color32(85, 218, 102, 255);
		}

		// ----------------------------------------------------------------------------
		// public methods
		// ----------------------------------------------------------------------------

		/// <summary>
		/// Resets Average FPS counter accumulative data.
		/// </summary>
		public void ResetAverage()
		{
			if (!Application.isPlaying) return;

			LastAverageValue = 0;
			currentAverageSamples = 0;
			currentAverageRaw = 0;

			if (averageSamples > 0 && accumulatedAverageSamples != null)
			{
				Array.Clear(accumulatedAverageSamples, 0, accumulatedAverageSamples.Length);
			}
		}

		/// <summary>
		/// Resets minimum and maximum FPS readings.
		/// </summary>
		public void ResetMinMax()
		{
			if (!Application.isPlaying) return;
			LastMinimumValue = -1;
			LastMaximumValue = -1;
			minMaxIntervalsSkipped = 0;
			
			UpdateValue(true);
			dirty = true;
		}

		// ----------------------------------------------------------------------------
		// internal methods
		// ----------------------------------------------------------------------------

		internal override void Activate()
		{
			if (!enabled) return;
			base.Activate();

			LastValue = 0;
			LastMinimumValue = -1;

			if (main.OperationMode == OperationMode.Normal)
			{
				if (colorCached == null)
				{
					CacheCurrentColor();
				}

				if (colorWarningCached == null)
				{
					CacheWarningColor();
				}

				if (colorCriticalCached == null)
				{
					CacheCriticalColor();
				}

				text.Append(colorCriticalCached).Append("0").Append(FPS_TEXT_END);
				dirty = true;
			}

			if (!inited)
			{
				main.StartCoroutine(COROUTINE_NAME);
				inited = true;
			}
		}

		internal override void Deactivate()
		{
			if (!inited) return;
			base.Deactivate();

			main.StopCoroutine(COROUTINE_NAME);
			ResetMinMax();
			ResetAverage();
			LastValue = 0;
			CurrentFpsLevel = FPSLevel.Normal;

			inited = false;
		}

		internal override void UpdateValue(bool force)
		{
			if (!enabled) return;

			int roundedValue = (int)newValue;
			if (LastValue != roundedValue || force)
			{
				LastValue = roundedValue;
				dirty = true;
			}

			if (LastValue <= criticalLevelValue)
			{
				if (LastValue != 0 && CurrentFpsLevel != FPSLevel.Critical)
				{
					CurrentFpsLevel = FPSLevel.Critical;
					if (OnFpsLevelChange != null) OnFpsLevelChange(CurrentFpsLevel);
				}
			}
			else if (LastValue < warningLevelValue)
			{
				if (LastValue != 0 && CurrentFpsLevel != FPSLevel.Warning)
				{
					CurrentFpsLevel = FPSLevel.Warning;
					if (OnFpsLevelChange != null) OnFpsLevelChange(CurrentFpsLevel);
				}
			}
			else
			{
				if (LastValue != 0 && CurrentFpsLevel != FPSLevel.Normal)
				{
					CurrentFpsLevel = FPSLevel.Normal;
					if (OnFpsLevelChange != null) OnFpsLevelChange(CurrentFpsLevel);
				}
			}

			// since ms calculates from fps we can calculate it when fps changed
			if (dirty && milliseconds)
			{
				LastMillisecondsValue = 1000f / newValue;
			}

			int currentAverageRounded = 0;
			if (average)
			{
				if (averageSamples == 0)
				{
					currentAverageSamples++;
					currentAverageRaw += (LastValue - currentAverageRaw) / currentAverageSamples;
				}
				else
				{
					if (accumulatedAverageSamples == null)
					{
						accumulatedAverageSamples = new float[averageSamples];
						ResetAverage();
					}

					accumulatedAverageSamples[currentAverageSamples % averageSamples] = LastValue;
					currentAverageSamples++;

					currentAverageRaw = GetAverageFromAccumulatedSamples();
				}

				currentAverageRounded = Mathf.RoundToInt(currentAverageRaw);

				if (LastAverageValue != currentAverageRounded || force)
				{
					LastAverageValue = currentAverageRounded;
					dirty = true;
				}
			}

			if (minMax)
			{
				if (minMaxIntervalsSkipped < minMaxIntervalsToSkip)
				{
					if (!force) minMaxIntervalsSkipped ++;
				}
				else if (dirty)
				{
					if (LastMinimumValue == -1)
						LastMinimumValue = LastValue;
					else if (LastValue < LastMinimumValue)
					{
						LastMinimumValue = LastValue;
						dirty = true;
					}

					if (LastMaximumValue == -1)
						LastMaximumValue = LastValue;
					else if (LastValue > LastMaximumValue)
					{
						LastMaximumValue = LastValue;
						dirty = true;
					}
				}
			}

			if (dirty && main.OperationMode == OperationMode.Normal)
			{
				string coloredStartText;

				if (LastValue >= warningLevelValue)
					coloredStartText = colorCached;
				else if (LastValue <= criticalLevelValue)
					coloredStartText = colorCriticalCached;
				else
					coloredStartText = colorWarningCached;

				text.Length = 0;
				text.Append(coloredStartText).Append(LastValue).Append(FPS_TEXT_END);

				if (milliseconds)
				{
					if (LastValue >= warningLevelValue)
						coloredStartText = colorCachedMs;
					else if (LastValue <= criticalLevelValue)
						coloredStartText = colorCriticalCachedMs;
					else
						coloredStartText = colorWarningCachedMs;

					text.Append(coloredStartText).Append(LastMillisecondsValue.ToString("F")).Append(MS_TEXT_END);
				}

				if (average)
				{
					if (currentAverageRounded >= warningLevelValue)
						coloredStartText = colorCachedAvg;
					else if (currentAverageRounded <= criticalLevelValue)
						coloredStartText = colorCriticalCachedAvg;
					else
						coloredStartText = colorWarningCachedAvg;


					text.Append(coloredStartText).Append(currentAverageRounded).Append(AVG_TEXT_END);
				}

				if (minMax)
				{
					text.Append(minMaxNewLine ? AFPSCounter.NEW_LINE : AFPSCounter.SPACE);

					if (LastMinimumValue >= warningLevelValue)
						coloredStartText = colorCachedMin;
					else if (LastMinimumValue <= criticalLevelValue)
						coloredStartText = colorCriticalCachedMin;
					else
						coloredStartText = colorWarningCachedMin;

					text.Append(coloredStartText).Append(LastMinimumValue).Append(MIN_TEXT_END);
					if (LastMaximumValue >= warningLevelValue)
						coloredStartText = colorCachedMax;
					else if (LastMaximumValue <= criticalLevelValue)
						coloredStartText = colorCriticalCachedMax;
					else
						coloredStartText = colorWarningCachedMax;

					text.Append(coloredStartText).Append(LastMaximumValue).Append(MAX_TEXT_END);
				}
			}
		}

		// ----------------------------------------------------------------------------
		// protected methods
		// ----------------------------------------------------------------------------

		protected override void CacheCurrentColor()
		{
			string colorString = AFPSCounter.Color32ToHex(color);
			colorCached = string.Format(FPS_TEXT_START, colorString);
			colorCachedMs = string.Format(MS_TEXT_START, colorString);
			colorCachedMin = string.Format(MIN_TEXT_START, colorString);
			colorCachedMax = string.Format(MAX_TEXT_START, colorString);
			colorCachedAvg = string.Format(AVG_TEXT_START, colorString);
		}

		protected void CacheWarningColor()
		{
			string colorString = AFPSCounter.Color32ToHex(colorWarning);
			colorWarningCached = string.Format(FPS_TEXT_START, colorString);
			colorWarningCachedMs = string.Format(MS_TEXT_START, colorString);
			colorWarningCachedMin = string.Format(MIN_TEXT_START, colorString);
			colorWarningCachedMax = string.Format(MAX_TEXT_START, colorString);
			colorWarningCachedAvg = string.Format(AVG_TEXT_START, colorString);
		}

		protected void CacheCriticalColor()
		{
			string colorString = AFPSCounter.Color32ToHex(colorCritical);
			colorCriticalCached = string.Format(FPS_TEXT_START, colorString);
			colorCriticalCachedMs = string.Format(MS_TEXT_START, colorString);
			colorCriticalCachedMin = string.Format(MIN_TEXT_START, colorString);
			colorCriticalCachedMax = string.Format(MAX_TEXT_START, colorString);
			colorCriticalCachedAvg = string.Format(AVG_TEXT_START, colorString);
		}

		// ----------------------------------------------------------------------------
		// private methods
		// ----------------------------------------------------------------------------

		private void RestartCoroutine()
		{
			main.StopCoroutine(COROUTINE_NAME);
			main.StartCoroutine(COROUTINE_NAME);
		}

		private float GetAverageFromAccumulatedSamples()
		{
			float averageFps;
			float totalFps = 0;

			for (int i = 0; i < averageSamples; i++)
			{
				totalFps += accumulatedAverageSamples[i];
			}

			if (currentAverageSamples < averageSamples)
			{
				averageFps = totalFps / currentAverageSamples;
			}
			else
			{
				averageFps = totalFps / averageSamples;
			}

			return averageFps;
		}
	}
}