using UnityEngine;

namespace WTTClientCommonLib.Models;

public class CustomZoneContainer
{
    public string FlareZoneType;
    public GameObject GameObject;
    public string ZoneType;

    public CustomZoneContainer(GameObject gameObject, string zoneType, string flareZoneType)
    {
        GameObject = gameObject;
        ZoneType = zoneType;
        FlareZoneType = flareZoneType;
    }
}
