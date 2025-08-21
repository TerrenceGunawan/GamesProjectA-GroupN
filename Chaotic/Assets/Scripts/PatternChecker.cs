using UnityEngine;
using UnityEngine.UI;

public class PatternChecker : MonoBehaviour
{

    private int correct;
    [SerializeField] private int correctCount = 4;
    [SerializeField] private Image[] toggleImages;


    void Start()
    {

    }

    void Update()
    {

    }

    public void Correct(bool toggle)
    {
        if (toggle)
        {
            correct++;
            Complete();
        }

        else
        {
            correct--;
            Complete();
        }

    }

    private void Complete()
    {
        if (correct == correctCount)
        {
            for (int i = 0; i < toggleImages.Length; i++)
            {
                toggleImages[i].color = new Color(0.3f, 1f, 0f);
            }
        }
        if (correct < 0)
        {
            correct = 0;
        }
    }
}
