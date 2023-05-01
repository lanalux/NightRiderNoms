using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUDControls : MonoBehaviour
{
    public static HUDControls Instance {get;set;}

    public CanvasGroup overlay, pickUpOverlay, dropOffOverlay;

    float defaultTimeScale = 1.0f;
    public bool gameIsPaused = false;

    float money = 0; // :(
    [SerializeField] Text moneyDisplay;

    [SerializeField] GameObject pauseMenu;

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused){
            PauseGame();
        }
    }
    public void FadeOut(){
        overlay.DOFade(1.0f, 2.0f);
    }

    public void FadeIn(){
        overlay.DOFade(0.0f, 2.0f);
    }

    public void PauseGame(){
        pauseMenu.SetActive(true);
        PauseTime();
    }

    public void ResumeGame(){
        Debug.Log("resume");
        pauseMenu.SetActive(false);
        ResumeTime();
    }

    public void PauseTime(){
        gameIsPaused = true;
        Time.timeScale = 0;
        MouseLook.Instance.mouselookIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeTime(){
        gameIsPaused = false;
        Time.timeScale = defaultTimeScale;
        MouseLook.Instance.mouselookIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void AddPayment(float amountToChange){
        money+=amountToChange;
        UpdateMoneyDisplay();
    }

    void UpdateMoneyDisplay(){
        moneyDisplay.text = "$"+money.ToString("F0");
    }
}
