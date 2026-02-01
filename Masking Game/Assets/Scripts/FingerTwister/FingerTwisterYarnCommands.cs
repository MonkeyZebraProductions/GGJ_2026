using UnityEngine;
using Yarn.Unity;

public class FingerTwisterYarnCommands : MonoBehaviour
{
    [SerializeField] private FingerTwisterController twister;

    [YarnCommand("twister_start")]
    public void StartTwister()
    {
        twister.GameStarted = true;
        twister.active = true;
        twister.StartTwister(twister.startCount, twister.maxCount);
    }

    [YarnCommand("twister_stop")]
    public void StopTwister()
    {
        twister.StopTwister();
    }

    [YarnCommand("twister_end")]
    public void EndTwister()
    {
        twister.HideUI();
        twister.gameObject.SetActive(false);
    }

    [YarnCommand("twister_max")]
    public void SetMax(int max)
    {
        twister.SetMaxCount(max);
    }
}
