using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public GameObject InstructionPanel;
    public GameObject CreditsPanel;
    public float PanelSwapDelay = 5f;

    private WaitForSeconds _panelSwapWait;

	// Use this for initialization
	void Start () {
        _panelSwapWait = new WaitForSeconds(PanelSwapDelay);

	    InitializePanels();

        StartCoroutine(SwitchPanels());
    }


    // Update is called once per frame
    void Update () {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }

    private void InitializePanels()
    {
        InstructionPanel.SetActive(true);
        CreditsPanel.SetActive(false);
    }

    private IEnumerator SwitchPanels()
    {
        while (true)
        {
            yield return _panelSwapWait;
            InstructionPanel.SetActive(!InstructionPanel.activeSelf);
            CreditsPanel.SetActive(!CreditsPanel.activeSelf);
        }
    }
}
