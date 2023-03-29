using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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
using System.Runtime.InteropServices;

namespace TestApplication;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    List<CpuInfo> CPUs = new List<CpuInfo>();
    List<GpuInfo> Gpus = new List<GpuInfo>();
    DriveInfo[] allDrives = null;
    private long totalMemory;

    private const string name ="";
    private readonly string nameReadonly;

    public MainWindow()
    {
        nameReadonly = "";
        InitializeComponent();
        //Storage devices
        allDrives = DriveInfo.GetDrives();
        foreach (var driveInfo in allDrives)
        {
            DrivesList.Items.Add(driveInfo.Name + " - Size: " + driveInfo.TotalSize/(1024*1024*1024) + " GB");
        }
        //RAM
        GetPhysicallyInstalledSystemMemory(out totalMemory);
        RamLabel.Content = totalMemory / (1024 * 1024) + " GB";
        //Processor
        var managementObjects =
            new ManagementObjectSearcher("select * from Win32_Processor")
                .Get()
                .Cast<ManagementObject>();
        foreach (var cpu in managementObjects)
        {
            var CPU = new CpuInfo();
            CPU.ID = (string)cpu["ProcessorId"];
            CPU.Socket = (string)cpu["SocketDesignation"];
            CPU.Name = (string)cpu["Name"];
            CPU.Description = (string)cpu["Caption"];
            CPU.AddressWidth = (ushort)cpu["AddressWidth"];
            CPU.DataWidth = (ushort)cpu["DataWidth"];
            CPU.Architecture = (ushort)cpu["Architecture"];
            CPU.SpeedMHz = (uint)cpu["MaxClockSpeed"];
            CPU.BusSpeedMHz = (uint)cpu["ExtClock"];
            CPU.L2Cache = (uint)cpu["L2CacheSize"] * (ulong)1024;
            CPU.L3Cache = (uint)cpu["L3CacheSize"] * (ulong)1024;
            CPU.Cores = (uint)cpu["NumberOfCores"];
            CPU.Threads = (uint)cpu["NumberOfLogicalProcessors"];

            CPU.Name =
                CPU.Name
                    .Replace("(TM)", "™")
                    .Replace("(tm)", "™")
                    .Replace("(R)", "®")
                    .Replace("(r)", "®")
                    .Replace("(C)", "©")
                    .Replace("(c)", "©")
                    .Replace("    ", " ")
                    .Replace("  ", " ");
            CPUs.Add(CPU);
            CpuList.Items.Add(CPU);
        }
       
        //GPU
        using var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
        foreach (ManagementObject obj in searcher.Get())
        {
            var gpu = new GpuInfo();
            gpu.Name =obj["Name"]?.ToString();
            gpu.DeviceID =obj["DeviceID"]?.ToString();
            gpu.AdapterRAM =obj["AdapterRAM"]?.ToString();
            gpu.AdapterDACType =obj["AdapterDACType"]?.ToString();
            gpu.Monochrome =obj["Monochrome"]?.ToString();
            gpu.InstalledDisplayDrivers =obj["InstalledDisplayDrivers"]?.ToString();
            gpu.DriverVersion =obj["DriverVersion"]?.ToString();
            gpu.VideoProcessor =obj["VideoProcessor"]?.ToString();
            gpu.VideoArchitecture =obj["VideoArchitecture"]?.ToString();
            gpu.VideoMemoryType =obj["VideoMemoryType"]?.ToString();
            Gpus.Add(gpu);
            GpuList.Items.Add(gpu);
        }

    }

    public class GpuInfo
    {
        public string? Name { get; set; }  
        public string? DeviceID { get; set; }  
        public string? AdapterRAM { get; set; }  
        public string? AdapterDACType { get; set; }  
        public string? Monochrome { get; set; }  
        public string? InstalledDisplayDrivers { get; set; }  
        public string? DriverVersion { get; set; }  
        public string? VideoProcessor { get; set; }  
        public string? VideoArchitecture { get; set; }  
        public string? VideoMemoryType { get; set; }  
    }
    public class CpuInfo {
        public string ID { get; set; }
        public string Socket { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ushort AddressWidth { get; set; }
        public ushort DataWidth { get; set; }
        public ushort Architecture { get; set; }
        public uint SpeedMHz { get; set; }
        public uint BusSpeedMHz { get; set; }
        public ulong L2Cache { get; set; }
        public ulong L3Cache { get; set; }
        public uint Cores { get; set; }
        public uint Threads { get; set; }
    }
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

}
