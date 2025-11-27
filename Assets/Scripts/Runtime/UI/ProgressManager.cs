using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressManager : MonoBehaviour
{
    [Header("Config")]
    public int totalStudents = 6;
    public int star1Threshold = -1; // = ceil(total/3) nếu -1
    public int star2Threshold = -1; // = ceil(total*2/3) nếu -1
    public int star3Threshold = -1; // = total nếu -1

    [Header("UI")]
    public Image barFill;
    public Image star1, star2, star3;
    public TextMeshProUGUI counter;

    int points;

    void Awake()
    {
        if (star1Threshold < 0) star1Threshold = Mathf.CeilToInt(totalStudents / 3f);
        if (star2Threshold < 0) star2Threshold = Mathf.CeilToInt(totalStudents * 2f / 3f);
        if (star3Threshold < 0) star3Threshold = totalStudents;
        Refresh();
    }

    public void AddPoint(int v)
    {
        points = Mathf.Clamp(points + v, 0, totalStudents);
        Refresh();
    }

    void Refresh()
    {
        if (barFill) barFill.fillAmount = (float)points / totalStudents;
        if (star1) star1.enabled = points >= star1Threshold;
        if (star2) star2.enabled = points >= star2Threshold;
        if (star3) star3.enabled = points >= star3Threshold;
        if (counter) counter.text = $"{points}/{totalStudents}";
    }
}
