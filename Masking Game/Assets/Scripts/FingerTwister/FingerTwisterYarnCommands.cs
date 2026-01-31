using UnityEngine;
using Yarn.Unity;

public class FingerTwisterYarnCommands : MonoBehaviour
{
    [SerializeField] private FingerTwisterController twister;

    [YarnCommand("twister_start")]
    public void StartTwister()
    {
        twister.StartTwister(twister.startCount, twister.maxCount);
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
