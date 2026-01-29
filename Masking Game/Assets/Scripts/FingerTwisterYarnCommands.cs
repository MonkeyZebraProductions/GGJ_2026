using UnityEngine;
using Yarn.Unity;

public class FingerTwisterYarnCommands : MonoBehaviour
{
    [SerializeField] private FingerTwisterController twister;

    [YarnCommand("twister_start")]
    public void StartTwister(int startCount = 1, int maxCount = 4)
    {
        twister.StartTwister(startCount, maxCount);
    }

    [YarnCommand("twister_stop")]
    public void StopTwister()
    {
        twister.StopTwister();
    }

    [YarnCommand("twister_max")]
    public void SetMax(int max)
    {
        twister.SetMaxCount(max);
    }
}
