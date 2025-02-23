using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class UIScreen : MonoBehaviour
{
    public Button button;
    public GameObject canvas;
    public Text text;
    public TimeManager timeManager;
    public PathFollower pathFollower;
    public XRInteractorLineVisual XRInteractorLeft;
    public XRInteractorLineVisual XRInteractorRight;

    void Start()
    {
        // positioning the UI screen based on the path
        // canvas.transform.position = new Vector3(pathFollower.pathCreator.path.GetPointAtDistance(-1f)[0] - 10f, pathFollower.pathCreator.path.GetPointAtDistance(-1f)[1] + 3f, pathFollower.pathCreator.path.GetPointAtDistance(-1f)[2] + 1f);
        button.onClick.AddListener(HideUIScreen);
    }

    void HideUIScreen()
    {
        // if user clicks start
        if (text.text == "START")
        {
            // hides UI screen
            canvas.SetActive(false);
            timeManager.gameStarted = true;
            HideRayCaster();
        }
        else if (text.text == "EXIT")
        {
            // UnityEditor.EditorApplication.isPlaying = false; // <- exits out of play mode in Unity
            Application.Quit(); // <- quits the actual game, which can be tested after we build the game and run it
        }
    }

    void HideRayCaster()
    {
        XRInteractorLeft.gameObject.SetActive(false);
        XRInteractorRight.gameObject.SetActive(false);
    }

    public void ShowRayCaster()
    {
        XRInteractorLeft.gameObject.SetActive(true);
        XRInteractorRight.gameObject.SetActive(true);
    }
}
