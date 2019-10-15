using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

internal struct LASTINPUTINFO
{
    public uint cbSize;

    public uint dwTime;
}

/// <summary>
/// Helps to find the idle time, (in milliseconds) spent since the last user input
/// </summary>
public class IdleTimeFinder
{
    [DllImport("User32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [DllImport("Kernel32.dll")]
    private static extern uint GetLastError();

    public static uint GetIdleTime()
    {
        LASTINPUTINFO lastInPut = new LASTINPUTINFO();
        lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
        GetLastInputInfo(ref lastInPut);

        return ((uint)Environment.TickCount - lastInPut.dwTime);
    }
    /// <summary>
    /// Get the Last input time in milliseconds
    /// </summary>
    /// <returns></returns>
    public static long GetLastInputTime()
    {
        LASTINPUTINFO lastInPut = new LASTINPUTINFO();
        lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
        if (!GetLastInputInfo(ref lastInPut))
        {
            throw new Exception(GetLastError().ToString());
        }
        return lastInPut.dwTime;
    }
}

namespace AutoSleep
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MessageBox.Show("AutoSleep starting..."); // Dialog to show that program is running
            while (true) // infinite loop to ensure progrma is running despite wakingup from sleep
            {
                if (IdleTimeFinder.GetIdleTime() > 600000) // Check if 15 minutes have passed
                {
                    Application.SetSuspendState(PowerState.Hibernate, true, true); // Sleep
                    MessageBox.Show("AutoSleep starting..."); // Dialog to show that program is running
                }
                Thread.Sleep(1000); // (Power optimization) Makes the program wait 1 second before refreshing, prevents overconsumption of CPU power
                //takes about 20% of CPU if constantly checking for idle time (while loop) 
            }
        }
    }
}

