using UnityEngine;
using Yarn.Unity;
using Yarn.Unity.Attributes;
public class ManualNodeMovement : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    OptionsPresenter optionsPresenter;
    [SerializeField] KeyCode AdvanceKey;
    [SerializeField] internal YarnProject? yarnProject;
    [YarnNode(nameof(yarnProject))]
    [SerializeField] string NodeToAdvanceTo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(AdvanceKey))
        {
            if (dialogueRunner != null)
            {
               
            }
            else
            {
                Debug.LogError("No Dialog Runner");
            }

        }
    }
}
