using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameSettingMenu : MonoBehaviour
{
    [SerializeField] private Toggle soundSettingToggle;
    [SerializeField] private Toggle vibrationSettingToggle;

    public GameObject ChangeThemePanel, ReportProblemPanel;
    public Button ChangeThemeBtn, ReportBtn;

    
    private void Start()
    {
        soundSettingToggle.isOn = true;
        vibrationSettingToggle.isOn = true;
   
        soundSettingToggle.onValueChanged.AddListener((value) =>
        {
            if (value)
                AudioController.instance.SoundOn();
            else
                AudioController.instance.SoundOff();
        });

        vibrationSettingToggle.onValueChanged.AddListener((value) =>
        {
            AudioController.instance.isVibrate = value;
        });


        ReportBtn.onClick.AddListener(OnReportClick);
        ChangeThemeBtn.onClick.AddListener(OnChangeTheme);
    
    }


    private void OnDisable()
    {
        if (ChangeThemePanel.activeInHierarchy)
            ChangeThemePanel.GetComponent<ThemeChanger>().DisableThemeChnagePanelPanelView();
        if (ReportProblemPanel.activeInHierarchy)
            ReportProblemPanel.GetComponent<ReportAProblempanel>().DisableReportPanelPanelView();
    }
    void OnChangeTheme() {
        if (!ReportProblemPanel.activeInHierarchy)
        {

            ChangeThemePanel.SetActive(true);
        }
        else
        {
            ReportProblemPanel.GetComponent<ReportAProblempanel>().DisableReportPanelPanelView();
            ChangeThemePanel.SetActive(true);
        }
    }
    void OnReportClick() {
        if (!ChangeThemePanel.activeInHierarchy)
        {
            ReportProblemPanel.SetActive(true);
        }
        else
        {
            ChangeThemePanel.GetComponent<ThemeChanger>().DisableThemeChnagePanelPanelView();
            ReportProblemPanel.SetActive(true);
        }
    }

}
[System.Serializable]
public class Theme {

    public Sprite Themeicon,ThemeTableBg;
    

}