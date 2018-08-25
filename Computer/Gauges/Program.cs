using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;

namespace Gauges {
	class Program {
		public static String ComPort { get; set; }
		public static List<Counter> counters = new List<Counter>();
		static StringBuilder SB = new StringBuilder();
		static int iTotalRam;

		static void Main(string[] args) {
			ComPort = "";
			foreach (string a in args) {
				if (a.Substring(0, 1) != "/" && a.Substring(0, 1) != "-") {
					ComPort = a.ToString();
				}
			}

			if (String.IsNullOrWhiteSpace(ComPort)) {
				Console.WriteLine("COM port was not supplied. (Pass it as a command line arg, no slash or dash.)");
				return;
			} else {
				Console.WriteLine("Port: " + ComPort);
			}

			counters.Add(new Counter(Counter.CounterTypes.CPU, new PerformanceCounter("Processor", "% Processor Time", "0")));
			counters.Add(new Counter(Counter.CounterTypes.CPU, new PerformanceCounter("Processor", "% Processor Time", "1")));
			counters.Add(new Counter(Counter.CounterTypes.CPU, new PerformanceCounter("Processor", "% Processor Time", "2")));
			counters.Add(new Counter(Counter.CounterTypes.CPU, new PerformanceCounter("Processor", "% Processor Time", "3")));
			counters.Add(new Counter(Counter.CounterTypes.RAM, new PerformanceCounter("Memory", "Available MBytes")));

			iTotalRam = (int)(new ComputerInfo().TotalPhysicalMemory / 1024 / 1024);

			Console.WriteLine("Running.");

			while (true) {
				using (var sp = new System.IO.Ports.SerialPort(ComPort, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One)) {
					try {
						sp.Open();
					} catch (Exception ex) {
						Console.WriteLine("Couldn't open COM port.");
						System.Threading.Thread.Sleep(5000);
					}
					while (true) {
						for (int i = 0; i < counters.Count; i++) {
							counters[i].GetNextPlot();
							SB.Append(i.ToString());
							SB.Append(":");
							SB.Append(counters[i].ShortAverage.ToString());
							SB.Append(",");
							SB.Append(counters[i].LongAverage.ToString());
							SB.Append(";");
						}
						SB.Append("\n");

#if DEBUG
						// Write to console.
						Console.Write(SB.ToString());
#endif

						try {
							// Write to serial.
							sp.Write(SB.ToString());
						} catch (Exception ex) {
							Console.WriteLine("Couldn't write to COM port.");
							System.Threading.Thread.Sleep(5000);
						}


						SB.Clear();
						System.Threading.Thread.Sleep(200);
					}
				}
			}
		}
		public class Counter {
			public enum CounterTypes { CPU, RAM }
			public PerformanceCounter PerfCounter { get; set; }
			private int[] _ShortBuffer = new int[2];
			private int[] _LongBuffer = new int[50];
			public int ShortAverage { get; set; }
			public int LongAverage { get; set; }
			private CounterTypes CounterType;

			public Counter(CounterTypes CT, PerformanceCounter PC) {
				ShortAverage = 0;
				LongAverage = 0;
				CounterType = CT;
				PerfCounter = PC;
			}

			public void GetNextPlot() {
				int iNewVal = 0;
				switch (CounterType) {
					case CounterTypes.CPU:
						iNewVal = (int)PerfCounter.NextValue();
						break;
					case CounterTypes.RAM:
						int iUsed = iTotalRam - (int)PerfCounter.NextValue();
						iNewVal = (int)((float)iUsed / (float)iTotalRam * (float)100); // Convert to percentage.
						break;
				}

				// Add new value to the short buffer.
				for (int i = 0; i < _ShortBuffer.Length - 1; i++) {
					_ShortBuffer[i] = _ShortBuffer[i + 1];
				}
				_ShortBuffer[_ShortBuffer.Length - 1] = iNewVal;

				// Add new value to the long buffer.
				for (int i = 0; i < _LongBuffer.Length - 1; i++) {
					_LongBuffer[i] = _LongBuffer[i + 1];
				}
				_LongBuffer[_LongBuffer.Length - 1] = iNewVal;

				// Calculate the mean over the short buffer.
				ShortAverage = (int)_ShortBuffer.Average();

				// Calculate the mean over the long buffer.
				LongAverage = (int)_LongBuffer.Average();

			}
		}
	}
}
