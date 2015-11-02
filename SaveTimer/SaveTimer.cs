/*
    Save Timer -- notify a user if they have not saved in a 'watch directory' for a certain interval
    Copyright (C) 2015 Peter Upfold.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

    The clock.ico and associated clock-* files are from WordPress Dashicons
    <https://developer.wordpress.org/resource/dashicons> and are hereby licensed
    under GPLv3 (or any later version) with font exception.
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace SaveTimer
{


    public partial class SaveTimer : Form
    {

        // internal state
        protected DateTime lastSaveTime = DateTime.Parse("1985-10-01 02:00:00");
        protected bool isEnabled = true;
        protected bool hasThrownPermissionsWarning = false;
        protected bool hasWarnedEmpty = false; // for now, we are always warning about empty
        protected bool isInGracePeriod = true; // a timer sets this to false upon expiry of the grace period

        // command line options can override these
        protected string watchDirectory = "X:\\";
        protected int secondsInterval = 60*5; // 5min
        protected int gracePeriodSeconds = 60 * 11; // 11 min


        public SaveTimer()
        {
            InitializeComponent();       

        }

        private bool setupArguments()
        {

            string logFilePath = Environment.ExpandEnvironmentVariables("%AppData%") + @"\SaveTimer\SaveTimer.log";

            // if log file is really big and old, we will clear it
            try { 
                if (File.Exists(logFilePath))
                {
                    FileInfo logFileInfo = new FileInfo(logFilePath);
                    if (DateTime.Compare(logFileInfo.LastWriteTimeUtc, DateTime.Now.AddMonths(-1)) < 0 &&
                        logFileInfo.Length > 100000) {
                        File.Delete(logFilePath);
                    }
                }
            }
            catch (Exception e)
            {}

            // create log directory if needed
            if (!Directory.Exists(Environment.ExpandEnvironmentVariables("%AppData%") + @"\SaveTimer"))
            {
                try
                {
                    Directory.CreateDirectory(Environment.ExpandEnvironmentVariables("%AppData%") + @"\SaveTimer");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to create the log directory for " + logFilePath, "Save Timer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }

            FileStream hlogFile = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
            TextWriterTraceListener stListener = new TextWriterTraceListener(hlogFile);
            Trace.AutoFlush = true;
            Trace.Listeners.Add(stListener);
            Trace.WriteLine("Started Save Timer at " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 0)
            {
                int argCount = 0;
                foreach (string arg in args)
                {
                    switch (argCount)
                    {
                        case 1:
                            watchDirectory = arg;

                            if (!(Directory.Exists(watchDirectory)))
                            {
                                MessageBox.Show("The watch directory specified does not exist.", "Unable to start", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Trace.WriteLine("The watch directory specified - " + watchDirectory.ToString() + " - does not exist.");
                                showHelp();
                                return false;
                            }

                            try
                            {
                                FileInfo[] files = null;
                                DirectoryInfo dir = new DirectoryInfo(watchDirectory);
                                files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                            }
                            catch (UnauthorizedAccessException e)
                            {
                                Trace.WriteLine("UnauthorizedAccessException on " + watchDirectory.ToString() + "\\*.*. Message returned is below.");
                                Trace.WriteLine(e.Message);
                                if (!hasThrownPermissionsWarning)
                                {
                                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                                    Trace.WriteLine(e.Message);
#if DEBUG
                                    notifyIcon1.BalloonTipText = "Save Timer couldn't read the permissions on a subfile or subfolder of " + watchDirectory.ToString() + ". " + e.Message;
#else
                                    notifyIcon1.BalloonTipText = "Save Timer couldn't read the permissions on a subfile or subfolder of " + watchDirectory.ToString() + ".";
#endif

                                    notifyIcon1.BalloonTipTitle = "Save Timer";
                                    notifyIcon1.ShowBalloonTip(15000);
                                    hasThrownPermissionsWarning = true;
                                }

                            }

                            break;

                        case 2:
                            try
                            {
                                secondsInterval = int.Parse(arg);
                            }
                            catch (FormatException fe)

                            {
                                Trace.WriteLine("The seconds interval was specified incorrectly on the command line. '" + arg + "' was provided. This caused a FormatException: " + fe.Message);
                                MessageBox.Show("The seconds interval of how frequently to check was not in the correct format. Please provide the number of seconds as the 2nd argument to the program.", "Unable to start", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                showHelp();
                                return false;
                            }

                            saveCheckTimer.Interval = secondsInterval * 1000;

                            break;
                        case 3:
                            try
                            {
                                gracePeriodSeconds = int.Parse(arg);
                            }
                            catch (FormatException fe)

                            {
                                Trace.WriteLine("The grace period seconds interval was specified incorrectly on the command line. '" + arg + "' was provided. This caused a FormatException: " + fe.Message);
                                MessageBox.Show("The grace period seconds interval of how frequently to check was not in the correct format. Please provide the number of seconds for the grace period as the 3nd argument to the program.", "Unable to start", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                showHelp();
                                return false;
                            }

                            gracePeriodTimer.Interval = gracePeriodSeconds * 1000;

                            break;
                    }
                    argCount++;
                }
            }

            Trace.WriteLine("Configured successfully.");
            Trace.WriteLine("Watch directory: " + watchDirectory);
            Trace.WriteLine("Save check interval: " + secondsInterval + "s");
            Trace.WriteLine("Grace period: " + gracePeriodSeconds + "s");

            return true;
        }

        private void showHelp()
        {
            // write help to console
            Console.WriteLine("Usage: SaveTimer.exe [directory to watch] [required save interval - seconds] [initial grace period - seconds]");
            MessageBox.Show("Usage: SaveTimer.exe [directory to watch] [required save interval - seconds] [initial grace period - seconds]\n\nThe 'watch directory' is the folder which will be observed for the most recently changed file.\nIf this file has not changed in the 'save interval', a message reminding the user to save will be displayed.\nThe 'initial grace period' is a period of time after the application starts during which the user is permitted to have not saved their work without the message appearing.", "Save Timer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "If you choose to quit, you will not be reminded again about saving.\n\nBy clicking Yes, I agree that I must now take all responsibility for saving my work.",
                "Quitting Save Timer",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.ServiceNotification
                ) == DialogResult.Yes )
            {
                Trace.WriteLine("Quitting Save Timer at " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ", as the context menu was used to quit.");
                Application.Exit();
            }

        }

        private void stopRemindingMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isEnabled) { 
                if (MessageBox.Show(
                    "By clicking OK, I agree that I must now take all responsibility for saving my work regularly.",
                    "Save Timer",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.ServiceNotification
                    ) == DialogResult.OK)
                {
                    isEnabled = false;
                    stopRemindingMeToolStripMenuItem.Text = "Start reminding me again";
                    Trace.WriteLine("User " + Environment.GetEnvironmentVariable("USERNAME") + " requested 'stop reminding me' at " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                }
            }
            else
            {
                isEnabled = true;
                notifyIcon1.BalloonTipTitle = "Save reminders enabled";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "You will now be reminded to save if you have not saved recently.";
                notifyIcon1.ShowBalloonTip(3000);
                Trace.WriteLine("User " + Environment.GetEnvironmentVariable("USERNAME") + " requested 'start reminding me again' at " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                stopRemindingMeToolStripMenuItem.Text = "Stop reminding me";

            }

        }

        private void saveCheckTimer_Tick(object sender, EventArgs e)
        {
            if (shouldNotifyToSave())
            {
                notifyToSave();
            }

        }


        protected bool shouldNotifyToSave()
        {
            // determine if the user needs to save, because a save has not been performed recently enough.

            // if disabled by the user, then we shouldn't
            if (!isEnabled)
            {
                Trace.WriteLine("User disabled notifications, so not evaluating file save time. " + DateTime.Now.ToString());
                return false;
            }
            if (isInGracePeriod)
            {
                Trace.WriteLine("In grace period, so not evaluating file save time. " + DateTime.Now.ToString());
                return false;
            }

            DirectoryInfo dirInfo;
            // examine last save time of most recent file in watch directory
            try { 
                dirInfo = new DirectoryInfo(watchDirectory);
            }
            catch (Exception)
            {
                // if something went wrong, play it safe and notify
                return true;
            }

            List<FileInfo> recentFiles = determineMostRecentFileTime(dirInfo);
            if ( recentFiles == null)
            {
                return true; // as before, play it safe
            }

            DateTime tooLongAgo;
            try { 
                tooLongAgo = DateTime.UtcNow.AddSeconds(float.Parse("-" + secondsInterval.ToString()));
            }
            catch (Exception e)
            {
                Trace.WriteLine("Unable to compare times -- couldn't get a negative seconds interval.");
                Trace.WriteLine("secondsInterval: " + secondsInterval.ToString());
                Trace.WriteLine(e.Message);
                return true;
            }

            if (DateTime.Compare(recentFiles[0].LastWriteTimeUtc, tooLongAgo) < 0)
            {
                Trace.WriteLine("User " + Environment.GetEnvironmentVariable("USERNAME") + " has not saved '" + recentFiles[0].FullName + "' since " + recentFiles[0].LastWriteTimeUtc.ToString() + ". The oldest save time acceptable would be " + tooLongAgo + ". Displaying message.");
                return true;
            }

            Trace.WriteLine("User " + Environment.GetEnvironmentVariable("USERNAME") + " has saved '" + recentFiles[0].FullName + " at " + recentFiles[0].LastWriteTimeUtc.ToString() + ". The oldest save time acceptable would be " + tooLongAgo + ". Not displaying message.");
            return false;
        }

        protected List<FileInfo> determineMostRecentFileTime(DirectoryInfo dir)
        {
            FileInfo[] files = null;
            List<FileInfo> fileList = new List<FileInfo>();

            try
            {
                files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException e)
            {
                Trace.WriteLine("UnauthorizedAccessException on " + dir.ToString() + "\\*.*. Message returned is below.");
                Trace.WriteLine(e.Message);
                if (!hasThrownPermissionsWarning)
                {
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                    notifyIcon1.BalloonTipText = "Save Timer couldn't read the permissions on a subfile or subfolder of " + dir.ToString() + ". Save Timer will be unable to determine if you have saved recently or not.";
                    notifyIcon1.BalloonTipTitle = "Save Timer";
                    notifyIcon1.ShowBalloonTip(15000);
                    hasThrownPermissionsWarning = true;
                }
                return null;

            }
            catch (DirectoryNotFoundException e)
            {
                Trace.WriteLine("DirectoryNotFoundException on " + dir.ToString());
                Trace.WriteLine(e.Message);
                return null;
            }

            // add to list
            foreach(FileInfo fi in files)
            {
                Debug.WriteLine(fi.FullName + " " + fi.LastWriteTimeUtc);
                fileList.Add(fi);
            }

            // sort list
            List<FileInfo> sortedFiles = fileList.OrderByDescending(x => x.LastWriteTimeUtc).ToList();

            if ( sortedFiles.Count > 0) { 
                // set instance variable for displaying in message
                lastSaveTime = sortedFiles[0].LastWriteTime;    

                return sortedFiles;
            }
            else
            {
                if ( !hasWarnedEmpty )
                {
                    notifyIcon1.BalloonTipTitle = "Save Timer";
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                    notifyIcon1.BalloonTipText = "You have not saved into the correct folder yet. Please save your work in " + watchDirectory.ToString();
                    notifyIcon1.ShowBalloonTip(25000);
                    //hasWarnedEmpty = true; ALWAYS warn about empty for now
                    Trace.WriteLine("Displayed the empty folder warning to the user.");
                }
                Trace.WriteLine("Would have displayed the empty folder warning to the user, but they already saw it.");
                return null;
            }

        }

        protected void notifyToSave()
        {
            if (lastSaveTime.Year != 1985) { 
                notifyIcon1.BalloonTipTitle = "You have not saved recently";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                notifyIcon1.BalloonTipText = "You have not saved since " + lastSaveTime.ToShortTimeString() + ". You must save your work regularly.";
                notifyIcon1.ShowBalloonTip(6000);
            }
            else
            {
                Trace.WriteLine("Last save time is empty.");
            }
        }

        private void SaveTimer_Load(object sender, EventArgs e)
        {
            if ( !setupArguments() )
            {
                Trace.WriteLine("Exiting Save Timer, as arguments could not be set up correctly.");
                Application.Exit();
            }
        }

        private void notifyContextMenu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Alt)
            {
                quitToolStripMenuItem.Visible = true;
            }
            else
            {
                quitToolStripMenuItem.Visible = false;
            }
        }

        private void exitHandler(object sender, EventArgs e)
        {
            Trace.WriteLine("Save Timer did quit at " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
        }

        private void gracePeriodTimer_Tick(object sender, EventArgs e)
        {
            isInGracePeriod = false;
            gracePeriodTimer.Enabled = false;
        }

        private void whenDidILastSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir;
            try {
                dir = new DirectoryInfo(watchDirectory);
                List<FileInfo> recentFiles = determineMostRecentFileTime(dir);

                if (recentFiles != null)
                {

                    lastSaveTime = recentFiles[0].LastWriteTime;

                    notifyIcon1.BalloonTipTitle = "Save Timer";
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon1.BalloonTipText = "You last saved at " + lastSaveTime.ToShortTimeString() + ".";
                    notifyIcon1.ShowBalloonTip(10000);
                }
                else
                {
                    Trace.WriteLine("Was going to show last save time by manual request, but directory is empty");
                }

            } catch (Exception ex)
            {
                Trace.WriteLine("Trying to show last save time by manual request.");
                Trace.WriteLine(ex.Message);
                notifyIcon1.BalloonTipTitle = "Save Timer";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "Sorry, couldn't show the last save time.";
                notifyIcon1.ShowBalloonTip(10000);
            }
    }
    }
}
