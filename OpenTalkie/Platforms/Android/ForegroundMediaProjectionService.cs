﻿using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Microsoft.Maui.Platform;

namespace OpenTalkie;

[Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeMediaProjection)]
internal partial class ForegroundMediaProjectionService : Service
{
    private readonly string NOTIFICATION_CHANNEL_ID = "1001";
    private readonly int NOTIFICATION_ID = 2;
    private readonly string NOTIFICATION_CHANNEL_NAME = "system_audio_notification";

    private Intent _intent = new(Android.App.Application.Context, typeof(ForegroundMediaProjectionService));

    public void Start() =>
        Android.App.Application.Context.StartForegroundService(_intent);

    public void Stop() =>
        Android.App.Application.Context.StopService(_intent);


    private void StartForegroundService()
    {
        var notifcationManager = GetSystemService(NotificationService) as NotificationManager;

        if (OperatingSystem.IsAndroidVersionAtLeast(26))
            CreateNotificationChannel(notifcationManager);

        var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
        notification.SetAutoCancel(false);
        notification.SetOngoing(true);
        notification.SetSmallIcon(Resource.Mipmap.appicon);
        notification.SetContentTitle("Open Talkie");
        notification.SetContentText("System audio capture service is running");

        if (OperatingSystem.IsAndroidVersionAtLeast(29))
            StartForeground(NOTIFICATION_ID, notification.Build(), Android.Content.PM.ForegroundService.TypeMediaProjection);
        else
            StartForeground(NOTIFICATION_ID, notification.Build());
    }

    private void CreateNotificationChannel(NotificationManager notificationMnaManager)
    {
        var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME,
        NotificationImportance.Low);
        notificationMnaManager.CreateNotificationChannel(channel);
    }

    public override IBinder OnBind(Intent intent) => null;

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        StartForegroundService();
        return StartCommandResult.NotSticky;
    }
}
