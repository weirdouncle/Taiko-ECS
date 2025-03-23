using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ShowFpsScript : MonoBehaviour
{
    public Text Text;

    private float updateInterval = 1;
    private float lastInterval;
    private int frames = 0;
    private bool counting = true;

    private float lowest = 900;
    private int counter = 0;
    private float amount = 0;

    public float FPS = 0;

    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
    }

    private void EndPlay()
    {
        counting = false;
        Debug.Log(string.Format("lowest {0} average {1}", lowest, amount / counter));
    }

    void Update()
    {
        if (counting)
        {
            frames++;
            var timeNow = Time.realtimeSinceStartup;
            if (timeNow > lastInterval + updateInterval)
            {
                FPS = frames / (timeNow - lastInterval);
                Text.text = FPS.ToString("#0.0");
                frames = 0;
                lastInterval = timeNow;
#if UNITY_EDITOR
                if (FPS < lowest)
                    lowest = FPS;

                counter++;
                amount += FPS;
#endif
            }
        }
    }
}
