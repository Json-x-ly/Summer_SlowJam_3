using UnityEngine;
using System.Collections;

public class LogicVisualSpace:MonoBehaviour{
    public static LogicVisualSpace main;
    public Material worldMat;
    public float offset;
    private static float camZ
    {
        get
        {
            return Camera.main.transform.position.z;
        }
    }
    public static Vector3 LogicToVisual(Vector3 pos)
    {
        float o = pos.y - (Mathf.Pow((pos.z + main.offset-camZ) * 0.1f, 2));
        o += +Mathf.Sin(pos.z * 0.1f + pos.x * 0.1f + Time.time) * 1;
        o += Mathf.Sin(pos.z * 0.3f + Time.time) * 0.2f;
        return new Vector3(pos.x,o,pos.z);
    }
    void Awake()
    {
        main = this;
        if (worldMat == null)
        {
            Debug.LogError("No World material attached to " + this.name + " on " + gameObject.name);
        }
    }
    void Update()
    {
        worldMat.SetFloat("_Offset", offset-camZ);
        worldMat.SetFloat("_MyTime", Time.time);
    }

}
