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


namespace SaveTimer
{
    partial class SaveTimer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveTimer));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.whenDidILastSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.stopRemindingMeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCheckTimer = new System.Windows.Forms.Timer(this.components);
            this.gracePeriodTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.notifyContextMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "icon";
            this.notifyIcon1.Visible = true;
            // 
            // notifyContextMenu
            // 
            this.notifyContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whenDidILastSaveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.stopRemindingMeToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.notifyContextMenu.Name = "notifyContextMenu";
            this.notifyContextMenu.Size = new System.Drawing.Size(184, 76);
            this.notifyContextMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.notifyContextMenu_PreviewKeyDown);
            // 
            // whenDidILastSaveToolStripMenuItem
            // 
            this.whenDidILastSaveToolStripMenuItem.Name = "whenDidILastSaveToolStripMenuItem";
            this.whenDidILastSaveToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.whenDidILastSaveToolStripMenuItem.Text = "&When did I last save?";
            this.whenDidILastSaveToolStripMenuItem.Click += new System.EventHandler(this.whenDidILastSaveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 6);
            // 
            // stopRemindingMeToolStripMenuItem
            // 
            this.stopRemindingMeToolStripMenuItem.Name = "stopRemindingMeToolStripMenuItem";
            this.stopRemindingMeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.stopRemindingMeToolStripMenuItem.Text = "&Stop reminding me";
            this.stopRemindingMeToolStripMenuItem.Click += new System.EventHandler(this.stopRemindingMeToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Visible = false;
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // saveCheckTimer
            // 
            this.saveCheckTimer.Enabled = true;
            this.saveCheckTimer.Interval = 10000;
            this.saveCheckTimer.Tick += new System.EventHandler(this.saveCheckTimer_Tick);
            // 
            // gracePeriodTimer
            // 
            this.gracePeriodTimer.Enabled = true;
            this.gracePeriodTimer.Interval = 50000;
            this.gracePeriodTimer.Tick += new System.EventHandler(this.gracePeriodTimer_Tick);
            // 
            // SaveTimer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 176);
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.Name = "SaveTimer";
            this.ShowInTaskbar = false;
            this.Text = "SaveTimer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.SaveTimer_Load);
            this.notifyContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip notifyContextMenu;
        private System.Windows.Forms.ToolStripMenuItem stopRemindingMeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.Timer saveCheckTimer;
        private System.Windows.Forms.Timer gracePeriodTimer;
        private System.Windows.Forms.ToolStripMenuItem whenDidILastSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}

