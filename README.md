Save Timer
==========

Notify a user if they have not saved in a ‘watch directory’ for a certain interval.

## Basic Description

This is a very simple application, written in C#/.NET 4.5.2, which observes a specified ‘watch directory’ on a given interval.
The most recent file in the watch directory is examined to determine its last modified time. If this is older than the specified
interval time, the user is shown a message reminding them to save their work. The user can suppress the messages for an indefinite
period of time by right-clicking the icon in the “clock box”/system tray and choosing *“Stop reminding me”*.

This was written to support academic examination access arrangements, where users are intentionally only given access to a cut-down word
processor such as WordPad, without spellcheck support. Unfortunately, WordPad does not autosave, so this application provides a regular
reminder for the user to save. In this usage, the user is given a blank mapped drive to save in. In addition to the regular save reminders,
the application also **ensures that the user has saved in the correct directory** to avoid data loss and ensure compliance with controlled
conditions of where they must save.

## Foolish Assumptions and Known Limitations

This tool is currently fairly *opinionated*, in the sense that it works very well in the particular use case for which it was initially
conceived.

We assume:

 * there is a locked down environment only offering an ‘obvious’ place the user needs to save their work
 * the watch directory starts out empty and would be processed, backed up and cleared before the user interacted with Save Timer again (no continued access to documents)
 * the user has permissions to all subdirectories and files in the watch directory, if it is not empty
 * the application is intended to be launched at logon by a Group Policy object, Startup shortcut or similar, rather than interactively by the user
 * the application is not intended to be manually quit by the user during a logon session (Alt-right-clicking the “clock box” icon does offer *Quit*, however)

## Usage

The application expects three command line arguments:

    SaveTimer.exe [directory to watch] [required save interval - seconds] [initial grace period - seconds]

The 1st argument, the ‘watch directory’, is the folder which will be observed for the most recently changed file. If this file has not changed in the ‘save interval’ (2nd argument), a message reminding the user to save will be displayed.

The ‘initial grace period’ (3rd argument) is a period of time after the application starts during which the user is permitted to have not saved their work without the message appearing.

There are no persistent settings, or special install requirements — the application can run directly from the network, as long as the client machine has .NET 4.5.2. The application does log extensively to `%AppData%\SaveTimer\SaveTimer.log`, and automatically cleans logs older than 1 month that are greater than 100 KB in size. This is not currently configurable at runtime.
