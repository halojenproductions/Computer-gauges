using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace GaugesTest {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
		}

		PerformanceCounter cpuCounter;
		PerformanceCounter ramCounter;
		int[] cpu = new int[5];

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
			ramCounter = new PerformanceCounter("Memory", "Available MBytes");

			while (true) {
				using (var sp = new System.IO.Ports.SerialPort("COM6", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One)) {

					sp.Open();

					// Add new value to buffer.
					for(int i = 0; i < cpu.Length-1; i++){
						cpu[i] = cpu[i + 1];
					}
					cpu[cpu.Length-1]=getCurrentCpuUsage();

					// Calculate mean over the buffer.
					int average = (int)cpu.Average();

					//string cpu = getCurrentCpuUsage();
					sp.Write(average + "\n");
					btnWhatever.Content = average;

				}
				System.Threading.Thread.Sleep(100);
			}
		}
		public int getCurrentCpuUsage() {
			return (int)cpuCounter.NextValue();
		}
		public string getAvailableRAM() {
			return Math.Round(ramCounter.NextValue()).ToString();
		}
	}
}
