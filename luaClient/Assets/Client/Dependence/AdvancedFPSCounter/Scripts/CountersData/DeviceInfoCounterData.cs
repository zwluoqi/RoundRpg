using System;
using System.Text;
using CodeStage.AdvancedFPSCounter.Labels;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	/// <summary>
	/// Shows additional device information.
	/// </summary>
	[AddComponentMenu("")]
	[Serializable]
	public class DeviceInfoCounterData: BaseCounterData
	{
		// ----------------------------------------------------------------------------
		// private fields
		// ----------------------------------------------------------------------------

		private bool inited;

		// ----------------------------------------------------------------------------
		// properties exposed to the inspector
		// ----------------------------------------------------------------------------

		#region CPUModel
		[Tooltip("CPU model and cores (including virtual cores from Intel's Hyper Threading) count.")]
		[SerializeField]
		private bool cpuModel = true;

		/// <summary>
		/// Shows CPU model and cores (including virtual cores from Intel's Hyper Threading) count.
		/// </summary>
		public bool CpuModel
		{
			get { return cpuModel; }
			set
			{
				if (cpuModel == value || !Application.isPlaying) return;
				cpuModel = value;
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		#region GPUModel
		[Tooltip("Shows GPU model, graphics API version, supported shader model (if possible), approximate pixel fill-rate in megapixels per second (if possible) and total Video RAM size (if possible).")]
		[SerializeField]
		private bool gpuModel = true;

		/// <summary>
		/// Shows GPU model, graphics API version, supported shader model (if possible), approximate pixel fill-rate in megapixels per second (if possible) and total Video RAM size (if possible).
		/// </summary>
		public bool GpuModel
		{
			get { return gpuModel; }
			set
			{
				if (gpuModel == value || !Application.isPlaying) return;
				gpuModel = value;
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		#region RAMSize
		[Tooltip("Shows total RAM size.")]
		[SerializeField]
		private bool ramSize = true;

		/// <summary>
		/// Shows total RAM size.
		/// </summary>
		public bool RamSize
		{
			get { return ramSize; }
			set
			{
				if (ramSize == value || !Application.isPlaying) return;
				ramSize = value;
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		#region ScreenData
		[Tooltip("Shows screen resolution, size and DPI (if possible).")]
		[SerializeField]
		private bool screenData = true;

		/// <summary>
		/// Shows screen resolution, size and DPI (if possible).
		/// </summary>
		public bool ScreenData
		{
			get { return screenData; }
			set
			{
				if (screenData == value || !Application.isPlaying) return;
				screenData = value;
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		// ----------------------------------------------------------------------------
		// properties only accessible from code
		// ----------------------------------------------------------------------------

		public string LastValue { get; private set; }

		// ----------------------------------------------------------------------------
		// constructor
		// ----------------------------------------------------------------------------

		internal DeviceInfoCounterData()
		{
			color = new Color32(172, 172, 172, 255);
			anchor = LabelAnchor.LowerLeft;
		}

		// ----------------------------------------------------------------------------
		// internal methods
		// ----------------------------------------------------------------------------

		internal override void Activate()
		{
			if (!enabled || !HasData()) return;
			base.Activate();

			inited = true;

			if (main.OperationMode == OperationMode.Normal)
			{
				if (colorCached == null)
				{
					CacheCurrentColor();
				}
			}

			UpdateValue();
		}

		internal override void Deactivate()
		{
			if (!inited) return;
			base.Deactivate();

			if (text != null) text.Length = 0;
			main.MakeDrawableLabelDirty(anchor);

			inited = false;
		}

		internal override void UpdateValue(bool force)
		{
			if (!inited && (HasData()))
			{
				Activate();
				return;
			}

			if (inited && (!HasData()))
			{
				Deactivate();
				return;
			}

			if (!enabled) return;

			bool needNewLine = false;

			if (text == null)
			{
				text = new StringBuilder(500);
			}
			else
			{
				text.Length = 0;
			}

			if (cpuModel)
			{
				text.Append("CPU: ").Append(SystemInfo.processorType).Append(" [").Append(SystemInfo.processorCount).Append(" cores]");
				needNewLine = true;
			}

			if (gpuModel)
			{
				if (needNewLine) text.Append(AFPSCounter.NEW_LINE);

				text.Append("GPU: ").Append(SystemInfo.graphicsDeviceName)
					.Append(", API: ").Append(SystemInfo.graphicsDeviceVersion);

				bool previousExists = true;

				int sm = SystemInfo.graphicsShaderLevel;
				if (sm == 20)
				{
					text.Append(AFPSCounter.NEW_LINE).Append("GPU: SM: 2.0");
				}
				else if (sm == 30)
				{
					text.Append(AFPSCounter.NEW_LINE).Append("GPU: SM: 3.0");
				}
				else if (sm == 40)
				{
					text.Append(AFPSCounter.NEW_LINE).Append("GPU: SM: 4.0");
				}
				else if (sm == 41)
				{
					text.Append(AFPSCounter.NEW_LINE).Append("GPU: SM: 4.1");
				}
				else if (sm == 50)
				{
					text.Append(AFPSCounter.NEW_LINE).Append("GPU: SM: 5.0");
				}
				else
				{
					previousExists = false;
				}

#if !UNITY_5
				int fillRate = SystemInfo.graphicsPixelFillrate;
				if (fillRate > 0)
				{
					if (previousExists)
					{
						text.Append(", FR: ");
					}
					else
					{
						text.Append(AFPSCounter.NEW_LINE).Append("GPU: FR: ");
					}
					text.Append(fillRate).Append(" MP/S");
					previousExists = true;
				}
#endif

				int vram = SystemInfo.graphicsMemorySize;
				if (vram > 0)
				{
					if (previousExists)
					{
						text.Append(", VRAM: ");
					}
					else
					{
						text.Append(AFPSCounter.NEW_LINE).Append("GPU: VRAM: ");
					}
					text.Append(vram).Append(" MB");
				}
				needNewLine = true;
			}

			if (ramSize)
			{
				if (needNewLine) text.Append(AFPSCounter.NEW_LINE);

				int ram = SystemInfo.systemMemorySize;

				if (ram > 0)
				{
					text.Append("RAM: ").Append(ram).Append(" MB");
					needNewLine = true;
				}
				else
				{
					needNewLine = false;
				}
			}

			if (screenData)
			{
				if (needNewLine) text.Append(AFPSCounter.NEW_LINE);
				Resolution res = Screen.currentResolution;

				text.Append("SCR: ").Append(res.width).Append("x").Append(res.height).Append("@").Append(res.refreshRate).Append("Hz [window size: ").Append(Screen.width).Append("x").Append(Screen.height);
				float dpi = Screen.dpi;
				if (dpi > 0)
				{
					text.Append(", DPI: ").Append(dpi).Append("]");
				}
				else
				{
					text.Append("]");
				}
			}

			LastValue = text.ToString();

			if (main.OperationMode == OperationMode.Normal)
			{
				text.Insert(0, colorCached);
				text.Append("</color>");
			}
			else
			{
				text.Length = 0;
			}

			dirty = true;
		}

		// ----------------------------------------------------------------------------
		// protected methods
		// ----------------------------------------------------------------------------

		protected override void CacheCurrentColor()
		{
			colorCached = "<color=#" + AFPSCounter.Color32ToHex(color) + ">";
		}

		// ----------------------------------------------------------------------------
		// private methods
		// ----------------------------------------------------------------------------

		private bool HasData()
		{
			return cpuModel || gpuModel || ramSize || screenData;
		}
	}
}
