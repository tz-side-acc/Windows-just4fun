using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Media;

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

public class AutoClosingMessageBox
{
    System.Threading.Timer _timeoutTimer;
    string _caption;
    AutoClosingMessageBox(string text, string caption, int timeout)
    {
        _caption = caption;
        _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
            null, timeout, System.Threading.Timeout.Infinite);
        using (_timeoutTimer)
            MessageBox.Show(text, caption);
    }
    public static void Show(string text, string caption, int timeout)
    {
        new AutoClosingMessageBox(text, caption, timeout);
    }
    void OnTimerElapsed(object state)
    {
        IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
        if (mbWnd != IntPtr.Zero)
            SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        _timeoutTimer.Dispose();
    }
    const int WM_CLOSE = 0x0010;
    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
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
            // Initialize variabales
            int sleepTime = 600000; // 10 minutes
            bool displayed = false; // for warning/alert message
            long LastInputTime = IdleTimeFinder.GetLastInputTime();
            DateTime StartupTime = DateTime.Now; // Else would be undefined

            SoundPlayer alertSound = new SoundPlayer(@"C:\Users\Tze Zhao\Desktop\C#\AutoSleep\Google_event_notification_tone.wav"); //Initiate Sound player
            AutoClosingMessageBox.Show("AutoSleep starting", "AutoSleep.exe", 5000); // Dialog to show that program is running

            while (true) // infinite loop to ensure program is running despite waking up from sleep
            {

                // To display warning message
                if (IdleTimeFinder.GetIdleTime() > sleepTime-60000) // Check if there is one minute left before shutdown
                {
                    if (displayed)
                    {
                        if (IdleTimeFinder.GetLastInputTime() != LastInputTime) // means that user has interacted with device
                        {
                            LastInputTime = IdleTimeFinder.GetLastInputTime(); // update last interacted time
                            displayed = false; // computer to display again the next time device is going to sleep
                        }
                    }
                    else
                    {
                        alertSound.Play(); // play alert sound
                        AutoClosingMessageBox.Show("Going to sleep in 1 min", "AutoSleep.exe", 30000); // alert user that computer is going to sleep
                        displayed = true; // computer has displayed
                    }
                }

                // To put PC to sleep
                if (IdleTimeFinder.GetIdleTime() > sleepTime && (StartupTime.AddMinutes(sleepTime / 60000) > DateTime.Now)) // Check if 10 minutes have passed since last user interaction and waking up from sleep
                {
                    Application.SetSuspendState(PowerState.Hibernate, true, true); // Sleep
                    AutoClosingMessageBox.Show("AutoSleep starting", "AutoSleep.exe", 5000); // Dialog to show that program is running after waking up
                    StartupTime = DateTime.Now;
                }
                Thread.Sleep(1000); // (Power optimization) Makes the program wait 1 second before refreshing, prevents overconsumption of CPU power
                //takes about 20% of CPU if constantly checking for idle time (while loop) 
            }
        }
    }
}

