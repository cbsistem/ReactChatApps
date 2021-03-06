﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using ServiceStack;
using ServiceStack.Text;
using Squirrel;

namespace ReactChat.AppWinForms
{
    static class Program
    {
        public static string HostUrl = "http://localhost:2337/";
        public static AppHost AppHost;
        public static FormMain Form;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SquirrelAwareApp.HandleEvents(
                OnInitialInstall,
                OnAppUpdate,
                onAppUninstall: OnAppUninstall,
                onFirstRun: OnFirstRun);

            AppHost = new AppHost();

            Cef.EnableHighDPISupport();
            Cef.Initialize(new CefSettings ());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                AppHost.Init().Start("http://*:2337/");
                "ServiceStack SelfHost listening at {0} ".Fmt(HostUrl).Print();
                Form = new FormMain();
            }
            catch (Exception e)
            {
                "Listening to existing service at {0}".Print(HostUrl);
                Form = new FormMain(startRight: true);
            }

            Form.Disposed += (sender, args) => AppUpdater.Dispose();
            Application.Run(Form);
        }

        public static void OnInitialInstall(Version version)
        {
            // Hook for first install
            AppUpdater.CreateShortcutForThisExe();
        }

        public static void OnAppUpdate(Version version)
        {
            // Hook for application update, CheckForUpdates() initiates this.
            AppUpdater.CreateShortcutForThisExe();
        }

        public static void OnAppUninstall(Version version)
        {
            // Hook for application uninstall
            AppUpdater.RemoveShortcutForThisExe();
        }

        public static void OnFirstRun()
        {
            // Hook for first run
        }
    }
}
