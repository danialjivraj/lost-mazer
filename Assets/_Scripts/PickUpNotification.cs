using UnityEngine;

public class PickUpNotification : MonoBehaviour
{
    public GameObject notificationObject;

    void Start()
    {
        if (notificationObject != null)
        {
            notificationObject.SetActive(false);
        }
        NotificationManager.Instance.RegisterNotification(this);
    }

    public void ActivateNotification()
    {
        if (notificationObject != null)
        {
            notificationObject.SetActive(true);
        }
    }

    public void ShowNotification()
    {
        if (notificationObject != null)
        {
            NotificationManager.Instance.ShowNotification(this);
        }
    }

    public void ResetNotification()
    {
        if (notificationObject != null)
        {
            notificationObject.SetActive(false);
            ResetAnimator();
        }
    }

    private void ResetAnimator()
    {
        Animator warningAnimator = notificationObject.GetComponent<Animator>();
        if (warningAnimator != null)
        {
            warningAnimator.Rebind();
            warningAnimator.Update(0f);
        }
    }
}
