﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;

namespace TgReminderBot.Wpf.Services
{
    static class TrayIconHelper
    {       
        static TaskbarIcon taskbarIcon = null;

        public static void InitTrayIcon(ContextMenu contextMenu)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    taskbarIcon = new TaskbarIcon();
                    string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
                    taskbarIcon.Icon = new System.Drawing.Icon(iconPath);
                    taskbarIcon.ContextMenu = contextMenu;
                }
                catch { }
            });
        }

        public static void ShowNotification(string title, string text)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {                  
                    taskbarIcon.ShowBalloonTip(
                        title ?? "",
                        text ?? "",
                        BalloonIcon.Info);
                }
                catch { }
            });
        }

        public static ContextMenu MenuFromDict(Dictionary<string, Action> menuDict)
        {
            var contextMenu = new ContextMenu();
            foreach (var item in menuDict)
            {
                var menuItem = new MenuItem();
                menuItem.Header = item.Key;
                var handler = item.Value;
                menuItem.Click += (s, a) =>
                {
                    handler();
                };
                contextMenu.Items.Add(menuItem);
            }
            return contextMenu;
        }
    }


}
