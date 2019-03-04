using UnityEngine;

[CreateAssetMenu(fileName = "New NetConfig", menuName = "Create new NetConfig asset", order = 1)]
public class NetConfig : ScriptableObject
{
    public string ipAddress;
    public string ipPort;
    public bool IsExtranet;
}
