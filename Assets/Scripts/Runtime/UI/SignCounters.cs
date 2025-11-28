using TMPro;
using UnityEngine;

public class SignCounter : MonoBehaviour
{
    public TextMeshProUGUI label;

    public void SetTotal(int v)
    {
        if (label) label.text = v.ToString();
    }

    public void SetRemaining(int v)
    {
        if (label) label.text = v.ToString();
    }
}
