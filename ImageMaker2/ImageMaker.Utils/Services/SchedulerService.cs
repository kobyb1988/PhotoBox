using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32.TaskScheduler;

namespace ImageMaker.Utils.Services
{
    public class SchedulerService
    {
        private void StartNew(DateTime startTime, string action, string parameters, string taskName)
        {
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = ts.NewTask();
                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.RegistrationInfo.Description = "instaprinter";

                td.Triggers.Add(new TimeTrigger(startTime));
                td.Actions.Add(new ExecAction(action, parameters));

                // Retrieve the task, change the trigger and re-register it
                Task t = ts.GetTask(taskName);
                if (t != null)
                {
                    td = t.Definition;
                    td.Triggers[0].StartBoundary = startTime;
                    
                    ts.RootFolder.RegisterTaskDefinition(taskName, td); 
                }
                else
                {
                    ts.RootFolder.RegisterTaskDefinition(taskName, td);
                }
            }
        }

        public void StartInstagramMonitoring(DateTime startTime)
        {
            const string cTask = @"InstagramService";
            const string cTaskScreenSaver = @"ScreenSaver";

            const string cAction = @"%windir%\System32\cmd.exe";
            const string cStartService = @"startService";
            const string cStartServiceShortcut = @"startService_admin";

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            ChangeLinkTarget(Path.Combine(baseDir, cStartServiceShortcut), Path.Combine(baseDir, cStartService));

            //string path = @"C:\Users\phantomer\Documents\visual studio 2013\Projects\ImageMaker\Scripts";
            string cParams = string.Format("/c start /d \"{0}\" {1}", baseDir, "startService_admin");
            string cParamsForScreenSaver = string.Format("/c start /d \"{0}\" {1}", baseDir, "ImageMaker.ScreenSaver.exe");
            StartNew(startTime, 
                cAction,
                cParams,
                cTask
                );

            StartNew(startTime,
                cAction,
                cParamsForScreenSaver,
                cTaskScreenSaver
                );
        }

        public void ChangeLinkTarget(string shortcutFullPath, string newTarget)
        {
            // Load the shortcut.
            Shell32.Folder folder = GetShell32NameSpaceFolder(Path.GetDirectoryName(shortcutFullPath));
            Shell32.FolderItems items = folder.Items();
            IEnumerator en = items.GetEnumerator();

            Shell32.FolderItem item = null;
            while (en.MoveNext())
            {
                item = en.Current as Shell32.FolderItem;
                if (item == null)
                    continue;

                if (item.Name == Path.GetFileName(shortcutFullPath))
                    break;
            }

            if (item == null || !item.IsLink)
                return;

            Shell32.ShellLinkObject currentLink = (Shell32.ShellLinkObject)item.GetLink;

            // Assign the new path here. This value is not read-only.
            currentLink.Path = newTarget;

            // Save the link to commit the changes.
            currentLink.Save();
        }

        public Shell32.Folder GetShell32NameSpaceFolder(Object folder)
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");

            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace",
            System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { folder });
        }
    }
}
