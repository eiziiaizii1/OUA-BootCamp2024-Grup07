using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeButtonColor(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeButtonColor(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeButtonColor(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeButtonColor(3);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeButtonColor(4);
        }
    }

    private void ChangeButtonColor(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colors = buttons[i].colors;
            if (i == index)
            {
                colors.normalColor = Color.green;
                colors.selectedColor = Color.green;
                colors.pressedColor = Color.green;
            }
            else
            {
                colors.normalColor = Color.white;
                colors.selectedColor = Color.white;
                colors.pressedColor = Color.gray;
            }
            buttons[i].colors = colors;
        }
    }
}
