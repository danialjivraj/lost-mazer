using BackendlessAPI;
using UnityEngine;

public class BackendlessManager : MonoBehaviour
{
    private const string APP_ID = "70CF2EE3-D68E-4464-8689-9C5E7391BF1E";
    private const string API_KEY = "73B1057D-6087-4314-B34C-7692D1B406E0";

    void Awake()
    {
        Backendless.InitApp(APP_ID, API_KEY);
    }
}