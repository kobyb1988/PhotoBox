using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Monads;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace ImageMaker.RegisterServices
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult ChangeShortcutLocation(Session session)
        {
            session.Log("Begin ChangeShortcutLocation");

            string path = session.CustomActionData["Location"];

            try
            {
                const string cStartService = @"startService";
                const string cStartServiceShortcut = @"startService_admin";

                ChangeLinkTarget(Path.Combine(path, cStartServiceShortcut), Path.Combine(path, cStartService));
            }
            catch (Exception e)
            {
                //string path = @"C:\Users\phantomer\Desktop\Stuff\empty.txt";
                //File.WriteAllText(path, e.Message);

                //session.Log(e.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        private static void ChangeLinkTarget(string shortcutFullPath, string newTarget)
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

        private static Shell32.Folder GetShell32NameSpaceFolder(Object folder)
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");

            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace",
            System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { folder });
        }
    }
}
