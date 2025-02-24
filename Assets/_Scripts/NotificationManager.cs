using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }
    private List<PickUpNotification> notifications = new List<PickUpNotification>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterNotification(PickUpNotification notification)
    {
        if (!notifications.Contains(notification))
        {
            notifications.Add(notification);
        }
    }

    public void UnregisterNotification(PickUpNotification notification)
    {
        if (notifications.Contains(notification))
        {
            notifications.Remove(notification);
        }
    }

    public void ShowNotification(PickUpNotification notificationToShow)
    {
        foreach (var notification in notifications)
        {
            notification.ResetNotification();
        }

        notificationToShow.ActivateNotification();
    }
}
