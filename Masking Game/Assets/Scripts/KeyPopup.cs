using UnityEngine;
using TMPro;

public class KeyPopup : MonoBehaviour
{
    
    TextMeshProUGUI keyPopupText;
    KeyCode keyCode;
    private string[] chars;
    private string randomChar;
    
// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Set Up Random Key Code
        chars = new string[36] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", 
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        randomChar = chars[Random.Range(0, 35)];

        keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), randomChar);
        
        keyPopupText = GetComponent<TextMeshProUGUI>();
        if (keyPopupText != null)
        {
            keyPopupText.text = randomChar;
        }
        else { Debug.LogError("ADD TMPRO"); }
        Debug.Log(keyCode.ToString());
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(keyCode)) 
        {
            //Perform Key Delete Function On Press
            Debug.Log("KeyPressed");
            Destroy(this.gameObject);
        }
    }

}
