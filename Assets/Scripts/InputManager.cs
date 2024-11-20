using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public Dictionary<string, Dictionary<string, KeyCode>> playersKeyBindings;

    public Dictionary<string, KeyCode> p1KeyBindings;
    public List<KeyCode> p1Keys;
    public Dictionary<string, KeyCode> p2KeyBindings;
    public List<KeyCode> p2Keys;

    public List<string> KeyBindingFunctionName;

    public static InputManager _inputManager;

    GameObject player1KeybindButtons;
    GameObject player2KeybindButtons;

    [HideInInspector]
    public bool isTimed;

    bool canStopAddFunctionOnP1KeyBinds;
    bool canStopAddFunctionOnP2KeyBinds;

    GameObject currentKey;
    string currentPlayerKeyBind;

    void Awake()
    {
        if(_inputManager != null && _inputManager != this)
            Destroy(gameObject);


        else
            _inputManager = this;

        p1KeyBindings = new Dictionary<string, KeyCode>();
        p2KeyBindings = new Dictionary<string, KeyCode>();
        playersKeyBindings = new Dictionary<string, Dictionary<string, KeyCode>>();
        for (int i = 0; i < p1Keys.Count; i++)
        {           
            p1Keys[i] = (KeyCode)System.Enum.Parse((typeof(KeyCode)), PlayerPrefs.GetString(KeyBindingFunctionName[i],p1Keys[i].ToString()));
            p2Keys[i] = (KeyCode)System.Enum.Parse((typeof(KeyCode)), PlayerPrefs.GetString(KeyBindingFunctionName[i], p2Keys[i].ToString()));
            p1KeyBindings.Add(KeyBindingFunctionName[i], p1Keys[i]);
            p2KeyBindings.Add(KeyBindingFunctionName[i], p2Keys[i]);
        }

        if(p1KeyBindings[KeyBindingFunctionName[0]] == KeyCode.LeftArrow)
            p1KeyBindings[KeyBindingFunctionName[0]] = KeyCode.A;

        playersKeyBindings.Add("Player1", p1KeyBindings);
        playersKeyBindings.Add("Player2", p2KeyBindings);
    }

    private void Update()
    {
        DontDestroyOnLoad(gameObject);

        if(player1KeybindButtons == null && SceneManager.GetActiveScene().buildIndex == 0)
            player1KeybindButtons = GameObject.FindGameObjectWithTag("P1KeyBind");


        if (player2KeybindButtons == null && SceneManager.GetActiveScene().buildIndex == 0)
            player2KeybindButtons = GameObject.FindGameObjectWithTag("P2KeyBind");



        if (player1KeybindButtons != null && SceneManager.GetActiveScene().buildIndex == 0 && !canStopAddFunctionOnP1KeyBinds)
        {
            for (int i = 0; i < player1KeybindButtons.transform.childCount; i++)
            {
                TextMeshProUGUI text = player1KeybindButtons.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                text.text = playersKeyBindings["Player1"][KeyBindingFunctionName[i]].ToString();
                Button bb = text.gameObject.transform.parent.GetComponent<Button>();
                bb.onClick.AddListener(() => { GetButtonGameObject("Player1"); });
                canStopAddFunctionOnP1KeyBinds = true;

            }
        }


        if (player2KeybindButtons != null && SceneManager.GetActiveScene().buildIndex == 0 && !canStopAddFunctionOnP2KeyBinds)
        {
            for (int i = 0; i < player2KeybindButtons.transform.childCount; i++)
            {
                TextMeshProUGUI text = player2KeybindButtons.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                text.text = playersKeyBindings["Player2"][KeyBindingFunctionName[i]].ToString();
                Button bb = text.gameObject.transform.parent.GetComponent<Button>();
                bb.onClick.AddListener(() => { GetButtonGameObject("Player2"); });
                canStopAddFunctionOnP2KeyBinds = true;
            }
        }

        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            canStopAddFunctionOnP1KeyBinds = false;
            canStopAddFunctionOnP2KeyBinds = false;
        }
    }

    public void GetButtonGameObject(string pKeyBind)
    {
        GameObject buttonObj = EventSystem.current.currentSelectedGameObject;
        currentKey = buttonObj;
        currentPlayerKeyBind = pKeyBind;
        Debug.Log(buttonObj.name);
    }

    void OnGUI()
    {
        if(currentKey != null)
        {
            Event e = Event.current;
            if(e.isKey)
            {
                playersKeyBindings[currentPlayerKeyBind][KeyBindingFunctionName[currentKey.transform.GetSiblingIndex()]] = e.keyCode;
                TextMeshProUGUI text = currentKey.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                text.text = e.keyCode.ToString();

                PlayerPrefs.SetString(KeyBindingFunctionName[currentKey.transform.GetSiblingIndex()], e.keyCode.ToString());

                if (currentPlayerKeyBind == "Player 1")
                    p1KeyBindings[KeyBindingFunctionName[currentKey.transform.GetSiblingIndex()]] = e.keyCode;
                else if (currentPlayerKeyBind == "Player 2")
                    p2KeyBindings[KeyBindingFunctionName[currentKey.transform.GetSiblingIndex()]] = e.keyCode;

                currentKey = null;
                currentPlayerKeyBind = null;
            }
        }
    }
}
