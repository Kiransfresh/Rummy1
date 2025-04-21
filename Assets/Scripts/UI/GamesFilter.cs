using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class GamesFilter : MonoBehaviour
{
    public Transform GamesListHolder;
    [Header("EntryFee Toggles")]
    public Toggle High;
    public Toggle Medium;
    public Toggle Low;
    [Header("Players Toggles")]
    public Toggle  Two_Players;
    public Toggle FOUR_Players;
    public Toggle SIX_Players;
    public Button ApplyBtn,FilterBtn;
    public GameObject FilterPanel,GridViewPanel;
    public Button SlideViewBtn;
    
    private Decimal HighestEntryFee,LowestEntryFee;
    private List<string> MediumValues= new List<string>();
    private List<GameListRecord> TotalGameslist;
    private List<GameListRecord> CurrentGamesList = new List<GameListRecord>();

    private bool refreshFilter = false;
    private bool refreshFilter2 = false;

    private List<Toggle> ValueChangedToggles=new List<Toggle>();
    bool FilterMode=true;

    private void Awake()
    {
        TotalGameslist = new List<GameListRecord>();

        High.onValueChanged.AddListener(delegate {
            ToggleValueChanged(High);
        });
        Medium.onValueChanged.AddListener(delegate {
            ToggleValueChanged(Medium);
        });
        Low.onValueChanged.AddListener(delegate {
            ToggleValueChanged(Low);
        });
        Two_Players.onValueChanged.AddListener(delegate {
            ToggleValueChanged(Two_Players);
        });

        SIX_Players.onValueChanged.AddListener(delegate {
            ToggleValueChanged(SIX_Players);
        });
        FOUR_Players.onValueChanged.AddListener(delegate {
            ToggleValueChanged(FOUR_Players);
        });
        
        SlideViewBtn.onClick.AddListener(() => {
                GridViewPanel.SetActive(false);
              
        });
        FilterBtn.onClick.AddListener(() => {
            FilterPanel.SetActive(true);
        });

        ApplyBtn.onClick.AddListener(OnApplyChanges);
        Invoke("GetHighandLowFees",1);
        
    }

    
    private void Update()
    {
      /*  if (UIManager.instance.toggleController.isOn && !refreshFilter)
        {
            refreshFilter = true;
            refreshFilter2 = false;
            Invoke("GetHighandLowFees", 1);
            FilterMode = true;
        }
        else if (!UIManager.instance.toggleController.isOn && !refreshFilter2)
        {
            refreshFilter = false;
            refreshFilter2 = true;
            Invoke("GetHighandLowFees", 1);
            FilterMode = true;
        }*/
    }



    private bool CheckForFilterMode()
    {
        if (High.isOn || Low.isOn || Medium.isOn || Two_Players.isOn /* || FOUR_Players.isOn */ || SIX_Players.isOn) {

            FilterMode = true;
            return FilterMode;

        }
        else
        {
            FilterMode = false;
            return FilterMode;
        }
         
    }
    private bool CheckForPlayerFilterMode()
    {
        if (Two_Players.isOn /* || FOUR_Players.isOn */ || SIX_Players.isOn)
        {
             return true;

        }
        else
        {   
            return false;
        }

    }
    private bool CheckForEntryfeeFilterMode()
    {
        if (High.isOn || Low.isOn || Medium.isOn)
        {
            return true;

        }
        else
        {
            return false;
        }

    }



    void ToggleValueChanged(Toggle toggle) {
        
            if (ValueChangedToggles.Contains(toggle))
            {
                ValueChangedToggles.Remove(toggle);
            }
            else
            {
                ValueChangedToggles.Add(toggle);
            }
        
    }

    public void DisableFilterPanel ()
    {
        FilterMode = false;
        var toggles = ValueChangedToggles;
        if (toggles.Count > 0) {
            foreach (var toggle in toggles)
            {
                toggle.isOn = !toggle.isOn;
            }
        }
        
        ValueChangedToggles.Clear();
        FilterMode = true;
        FilterPanel.SetActive(false);
    }

    // Method called in Invoke on Awake and Update
    public void GetHighandLowFees()
    {
        TotalGameslist = GamesListHolder.GetComponentsInChildren<GameListRecord>().ToList();

        foreach (var gamerec in TotalGameslist)
        {
            Decimal fee  = Decimal.Parse(gamerec.EntryFee.text);
            int Players = gamerec.MaxPlayers;
            if (fee > HighestEntryFee) {
                HighestEntryFee = fee;
            }

            if (fee < LowestEntryFee || LowestEntryFee == 0) {

                LowestEntryFee = fee;
            }
        }
        foreach (var gamerec in TotalGameslist)
        {
            Decimal fee = Decimal.Parse(gamerec.EntryFee.text);
            int Players = gamerec.MaxPlayers;
            if (fee > HighestEntryFee &&fee>LowestEntryFee)
            {
                MediumValues.Add(gamerec.EntryFee.text);
            }
        }
    }

    void OnApplyChanges()
    {


        if (ValueChangedToggles.Count > 0)
        {

            if (!CheckForFilterMode())
            {
                foreach (var game in TotalGameslist)
                {
                    game.gameObject.SetActive(true);
                }
                ValueChangedToggles.Clear();
                FilterPanel.SetActive(false);
                return;


            }



            if (CheckForPlayerFilterMode())
            {

                FilterWithPlayersNo(2,Two_Players.isOn,TotalGameslist);
                FilterWithPlayersNo(4,FOUR_Players.isOn, TotalGameslist);
                FilterWithPlayersNo(6,SIX_Players.isOn, TotalGameslist);

            }
            else
            {
                FilterWithPlayersNo(2, true,TotalGameslist);
                FilterWithPlayersNo(4, true, TotalGameslist);
                FilterWithPlayersNo(6, true, TotalGameslist);
            }
            UpdateCurrentGameList();
            if (CheckForEntryfeeFilterMode())
            {
                if (CheckForPlayerFilterMode())
                {

                    FilterMediuimEntryFee(Medium.isOn, CurrentGamesList);
                    FilterWithEntryFee(HighestEntryFee, High.isOn, CurrentGamesList);
                    FilterWithEntryFee(LowestEntryFee, Low.isOn, CurrentGamesList);



                }
                else
                {
                    FilterMediuimEntryFee(Medium.isOn, TotalGameslist);
                    FilterWithEntryFee(HighestEntryFee, High.isOn, TotalGameslist);
                    FilterWithEntryFee(LowestEntryFee, Low.isOn, TotalGameslist);
                }


            }
            ValueChangedToggles.Clear();
        }
        FilterPanel.SetActive(false);
    }

    void FilterWithEntryFee(Decimal fee,bool RecordStatus,List<GameListRecord> Gameslist)
        {
            

            foreach (var gamerecord in Gameslist)
            {
               
                if (gamerecord.EntryFee.text.Equals(fee.ToString()))
                {
                    
                    if (RecordStatus)
                    {
                        gamerecord.gameObject.SetActive(true);

                    }
                    else
                    {
                        gamerecord.gameObject.SetActive(false);
                    }
                }

            }
        
            ValueChangedToggles.Clear();
        }
    

    void FilterWithPlayersNo(int players,bool RecordStatus, List<GameListRecord> Gameslist) {

        foreach (var gamerecord in Gameslist)
        {

            if (gamerecord.MaxPlayers==players)
            {

                if (RecordStatus)
                {

                    gamerecord.gameObject.SetActive(true);
                }
                else {

                    gamerecord.gameObject.SetActive(false);
                }
            }
           
        }
       
    }

    void FilterMediuimEntryFee(bool RecordStatus, List<GameListRecord> Gameslist) {



      foreach (var gamerecord in Gameslist)
      {

          if (MediumValues.Contains(gamerecord.EntryFee.text))
          {

              if (RecordStatus)
              {
                  gamerecord.gameObject.SetActive(true);

              }
              else
              {
                  gamerecord.gameObject.SetActive(false);
              }
          }

      }
    }



    void UpdateCurrentGameList() {

        CurrentGamesList.Clear();
        foreach (var gamerecord in TotalGameslist)
        {
            if (gamerecord.gameObject.activeInHierarchy) {

                CurrentGamesList.Add(gamerecord);
            
            }
        }
    }
}
