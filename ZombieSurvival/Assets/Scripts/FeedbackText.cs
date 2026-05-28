using TMPro;
using UnityEngine;

public class FeedbackText : MonoBehaviour
{
    [SerializeField]
    private float lifespan = 2f;

    [SerializeField]
    private float moveSpeed = 0.5f;

    [SerializeField]
    private float fadeSpeed = 0.5f;

    private TextMeshPro myText;

    private void Awake()
    {
        myText = transform.Find("Text").GetComponent<TextMeshPro>();

        Destroy(gameObject, lifespan);
    }

    private void Update()
    {
        transform.localPosition +=
            Vector3.forward *
            Time.deltaTime *
            moveSpeed *
            lifespan;

        SetFadingText();
    }

    private void SetFadingText()
    {
        Color c = myText.color;

        c.a -=
            Time.deltaTime *
            fadeSpeed *
            lifespan;

        myText.color = c;
    }

    public void ChangeText(float value)
    {
        value = Mathf.Round(value);

        string strVal =
            value.ToString("N0");

        if (value > 0)
        {
            strVal = "+" + strVal;
        }

        myText.text = strVal;

        myText.color =
            (value > 0)
            ? Color.green
            : Color.red;
    }

}