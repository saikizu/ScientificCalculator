using UnityEngine;
using UnityEngine.UI;

public class InternetCheck : MonoBehaviour
{
    public Text text;
    public int times;
    public int timeset;

    void Start()
    {
        if (!PlayerPrefs.HasKey("times"))
            PlayerPrefs.SetInt("times", timeset);
        else
            times = PlayerPrefs.GetInt("times");
        Show();
    }

    void Show()
    {
        if (IsConnected())
        {
            times = timeset;
            PlayerPrefs.SetInt("times", timeset);
            text.gameObject.SetActive(false);
            if (PlayerPrefs.HasKey("blocked"))
                PlayerPrefs.DeleteKey("blocked");
        }
        else
        {
            if (times == 0)
                PlayerPrefs.SetInt("blocked", 1);
            text.gameObject.SetActive(true);
            text.text = times.ToString();
        }
    }

    public void Counting()
    {
        if (times > 0)
        {
            times -= 1;
            PlayerPrefs.SetInt("times", times);
            text.text = times.ToString();
        }
        Show();
    }

    public bool IsConnected()
    {
        return !(Application.internetReachability == NetworkReachability.NotReachable);
    }
}
