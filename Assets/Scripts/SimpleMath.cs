using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

public class SimpleMath : MonoBehaviour
{
    [SerializeField]
    TEXInput m_Input;
    [SerializeField]
    TEXDraw m_Output;
    [SerializeField]
    GameObject m_Up, m_Down;
    [SerializeField]
    List<GameObject> KeyGrids;

    EventSystem eventSystem;

    int Mode { get; set; }
    int current = 0;
    bool Linear { get; set; }

    List<Calculation> answers;
    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        answers = new List<Calculation>();
        Focus();
        Mode = 0;
        OnOffUpDown();
    }

    private void Focus()
    {
        eventSystem.SetSelectedGameObject(m_Input.gameObject);
    }

    public void AddButton(string input)
    {
        try
        {
            m_Input.SetSelection(input);
            Move(0);
            Move(1);
            Focus();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AddButton {input} doesn't work");
            Debug.LogError(ex);
        }
    }

    public void DelButton()
    {
        m_Input.Backspace();
        Focus();
    }

    public void AC()
    {
        m_Input.text = "";
        m_Output.text = "";
        OnOffUpDown();
        Focus();
    }

    public void Reset()
    {
        m_Input.text = "";
        m_Output.text = "";
        answers = new List<Calculation>();
        current = 0;
        OnOffUpDown();
        Focus();
    }

    public void Left(int amount)
    {
        if (m_Input.selectionStart != 0)
            m_Input.selectionStart -= amount;
    }

    public void Move(int key)
    {
        switch (key)
        {
            case 0:
                m_Input.Move(false, m_Input.MoveLeft);
                Focus();
                return;
            case 1:
                m_Input.Move(false, m_Input.MoveRight);
                Focus();
                return;
            case 2:
                m_Input.Move(false, m_Input.MoveUp);
                Focus();
                return;
            case 3:
                m_Input.Move(false, m_Input.MoveDown);
                Focus();
                return;
        }
        Debug.LogError($"Move {key} doesn't work");
    }

    public void MoveMouse(int position)
    {
        m_Input.selectionStart += position;
        Focus();
    }

    public void ChangeKeys(int mode)
    {
        if (Mode == mode)
        {
            SwitchKeys(0);
            Focus();
            return;
        }
        else
        {
            SwitchKeys(mode);
            Focus();
            return;
        }
        //Debug.LogError($"ChangeKeys {mode} doesn't work");
    }

    public void SwitchKeys(int mode)
    {
        if (mode >= 0 && mode < KeyGrids.Count)
        {
            foreach (var item in KeyGrids)
                item.SetActive(false);
            KeyGrids[mode].SetActive(true);
            Focus();
            return;
        }
        Debug.LogError($"SwitchKey {mode} doesn't work");
    }

    public void Calculation()
    {
        if (PlayerPrefs.HasKey("blocked"))
        {
            m_Output.text = "Internet need for ads!";
            return;
        }
        Calculation output = new(m_Input.text);
        if (output.Success)
        {
            answers.Add(output);
            current = answers.Count - 1;
            m_Output.text = answers[current].Output;
            OnOffUpDown();
        }
        else
        {
            m_Output.text = output.Error;
            OnOffUpDown();
        }
        Focus();
    }

    public void OnOffUpDown()
    {
        Linear = true;
        if (current == 0 && answers.Count < 2)
        {
            m_Up.SetActive(false);
            m_Down.SetActive(false);
        }
        else if (current == 0 && answers.Count > 1)
        {
            m_Down.SetActive(true);
            m_Up.SetActive(false);
        }
        else if (current == answers.Count - 1 && answers.Count > 1)
        {
            m_Up.SetActive(true);
            m_Down.SetActive(false);
        }
        else
        {
            m_Down.SetActive(true);
            m_Up.SetActive(true);
        }
    }

    public void Up()
    {
        current--;
        m_Output.text = answers[current].Output;
        m_Input.text = answers[current].Input;
        OnOffUpDown();
        Focus();
    }

    public void Down()
    {
        current++;
        m_Output.text = answers[current].Output;
        m_Input.text = answers[current].Input;
        OnOffUpDown();
        Focus();
    }

    public void SwitchLinearAndMath()
    {
        if (Linear)
        {
            Linear = false;
            m_Output.text = answers[current].OutputMath();
        }
        else
        {
            Linear = true;
            m_Output.text = answers[current].Output;
        }
    }

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = answers[current].Output;
    }
}