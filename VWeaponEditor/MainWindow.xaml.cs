using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using VWeaponEditor.Views;

namespace VWeaponEditor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow {
        private const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public MainWindow() {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();

            // Process process = Process.GetProcessesByName("notepad")[0];
            // IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            // int bytesRead = 0;
            // byte[] buffer = new byte[24]; //'Hello World!' takes 12*2 bytes because of Unicode
            // // 0x0046A3B8 is the address where I found the string, replace it with what you found
            // ReadProcessMemory((int) processHandle, 0x0046A3B8, buffer, buffer.Length, ref bytesRead);
            // var str = Encoding.Unicode.GetString(buffer);
            // MessageBox.Show(str +
            //                 " (" + bytesRead.ToString() + "bytes)", "msg");
        }
    }
}
