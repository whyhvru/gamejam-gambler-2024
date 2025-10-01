using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [SerializeField] private Button _exitButton;

    private void Start() 
    {
        _exitButton.onClick.AddListener(Quit);
    }

    private void Quit()
    {
        Application.Quit();
    }
}