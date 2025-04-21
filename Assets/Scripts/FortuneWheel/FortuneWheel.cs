using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class FortuneWheel : MonoBehaviour
{
    public ScalingEffect[] scalingEffect;
    public GameObject PrizePanel;
    public GameObject Wheelparent;
    public TextMeshProUGUI Prizetext,TimerText;
    public Button OKBtn,CloseBtn;
    public Transform Wheel, Handel;
    public float WheelSpeed;
    [Range(3, 7)]
    public int SpinRounds;
    public int CostOfSpin;
    public float EachFreeSpinTime;
    public GameObject SliceHolder, SlicePrefab, ItemsHolder, PrizeitemPrefab;
    private List<GameObject> ExtraObjects = new List<GameObject>();
    public SliceProperties[] NoOfItems;
    public float Spacing;
    public String PrizeAmount;
    public bool RandomPrize;
    public Button SpinBtn;
    public float WheelRotationTime;
    private float ZAngle, FillAmount, MaxAngle, SlowSpeed;
    private int SpinCount;
    private bool IsSpinning, StopSpin,IsFreeSpinAvaible;
    SliceProperties PrizeSlice;
    private ulong LastSpinTime;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private float RotationTime;

    private void Start()
    {
        RotationTime = WheelRotationTime;
        StartCoroutine(FortuneWheelPanelViewEntryEffect());
        if (SpinBtn != null)
            SpinBtn.onClick.AddListener(SpinWheel);

        OKBtn.onClick.AddListener(OkBtnClick);
        CloseBtn.onClick.AddListener(DisableFortuneWheelPanelPanelView);


        SpinCount = 0;

        SlowSpeed = 1;
    }
    



    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

  
    private IEnumerator FortuneWheelPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator FortuneWheelPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }




    public void DisableFortuneWheelPanelPanelView()
    {
        StartCoroutine(FortuneWheelPanelViewExitEffect());
    }


    private void PlayStartEffects()
    {
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].ExitEffect());
        }
    }
    private void OnEnable()
    {
        StartCoroutine(FortuneWheelPanelViewEntryEffect());
        //if (!IsFreeSpinAvaible)
      //  SpinBtn.interactable = false;
        if (PlayerPrefs.HasKey("LastFortuneWheelSpin"))
        {
            
            LastSpinTime = ulong.Parse(PlayerPrefs.GetString("LastFortuneWheelSpin"));
        }
        else
        {
            
            PlayerPrefs.SetString("LastFortuneWheelSpin", DateTime.Now.Ticks.ToString());
            LastSpinTime = ulong.Parse(PlayerPrefs.GetString("LastFortuneWheelSpin"));
            OnFreeSpinAvailable();
        }
    }
    private void OnDisable()
    {
        AudioController.instance.StopSounds();
        if(PrizePanel.activeInHierarchy)
            PrizePanel.SetActive(false);
        if (StopSpin || IsSpinning) {
            StopSpin = false;
            IsSpinning = false;
            RefreshTimer();
            
        }
    }

    private void Update()
    {

        if (!IsFreeSpinAvaible)
        {
            ulong TimeDiff = ((ulong)DateTime.Now.Ticks - LastSpinTime);

            ulong Secs = TimeDiff / TimeSpan.TicksPerSecond;

            float secondsleft = (int)((EachFreeSpinTime * 3600) - Secs);
            int Hrsleft = (int)secondsleft / 3600;
            int MinsLeft = ((int)secondsleft / 60) % 60;
            int SecsLeft = (int)(secondsleft % 60);
            TimerText.text = String.Format("Next Free Turn :{0:00}:{1:00}:{2:00}", Hrsleft, MinsLeft, SecsLeft);
            if (secondsleft < 0)
            {

                OnFreeSpinAvailable();

            }


        }
        if (Wheel != null && IsSpinning)
        {
            RotationTime -= Time.deltaTime;
            Wheel.Rotate(Vector3.forward * WheelSpeed * Time.deltaTime);
            // Handel.Rotate(Vector3.forward*Time.deltaTime);
            if (MaxAngle < Wheel.eulerAngles.z)
            {

                MaxAngle = Wheel.eulerAngles.z;

            }
            else
            {
                MaxAngle = 0;
                SpinCount++;
            }
            if (RotationTime < WheelRotationTime / 1.5f)
            {
                IsSpinning = false;
                StopWheel();

            }
        }
        if (StopSpin)
        {

            if (RotationTime > 0.5f)
            {
                RotationTime -= Time.deltaTime;
                Wheel.Rotate(Vector3.forward * WheelSpeed / SlowSpeed * Time.deltaTime);
                SlowSpeed += Time.deltaTime / 2;
                SlowSpeed = Mathf.Clamp(SlowSpeed, 1, 4);
                if (MaxAngle < Wheel.eulerAngles.z)
                {

                    MaxAngle = Wheel.eulerAngles.z;

                }
                else
                {
                    MaxAngle = 0;
                    SpinCount++;
                }

            }
            else if (Vector3.Distance(Wheel.eulerAngles, new Vector3(0, 0, 360 - (PrizeSlice.ZAngle + (360 / NoOfItems.Length) / 2))) > 1)
            {
                Wheel.Rotate(Vector3.forward * WheelSpeed / SlowSpeed * Time.deltaTime);

            }
            else
            {
                RotationTime = WheelRotationTime;
                StopSpin = true;
                // AudioController.instance.StopSounds();
                Invoke("DisplayPrize", 1);
                SlowSpeed = 0;
                SpinCount = 1;
                CloseBtn.gameObject.SetActive(true);
                RefreshTimer();
            }

        }


    }
    void SpinWheel()
    {
        SpinBtn.interactable = true;
        AudioController.instance.OnSpinWheel();
        CloseBtn.gameObject.SetActive(false);
        if (!IsSpinning)
        {
           
            IsSpinning = true;

        }
        

    }
   
    void StopWheel()
    {

        PrizeSlice = Array.Find(NoOfItems, SliceProperties => SliceProperties.Prizetext == PrizeAmount);


        if (PrizeSlice != null && !RandomPrize)
        {
            StopSpin = true;
        }
        else
        {
            PrizeSlice = NoOfItems[Random.Range(0, NoOfItems.Length)];
            StopSpin = true;
        }

       
    }

    public void GenerateSlices()
    {

        double fillAmount = 1 / (double)NoOfItems.Length;

        if (SliceHolder.transform.childCount >= 2)
        {
            int CurrentSliceCount = SliceHolder.transform.childCount;

                for (int i = 0; i < CurrentSliceCount; i++)
                {
                    
                if (i<NoOfItems.Length)
                {
                    SliceProperties temp = NoOfItems[i];
                    var CurrentSlice = SliceHolder.transform.GetChild(i);
                    WheelSlice Slice = CurrentSlice.GetComponent<WheelSlice>();
                    Slice.UpdateSlice(temp.SliceColor, GetFillAmount(), GetZangle(i), Spacing);
                    temp.ZAngle = GetZangle(i);
                    var CurrentPrizeitem = ItemsHolder.transform.GetChild(i);
                    Prizeitem item = CurrentPrizeitem.GetComponent<Prizeitem>();
                    item.UpdatePrizeItem(temp.ItemImage, temp.Prizetext, temp.ZAngle + (360 / NoOfItems.Length) / 2);
                }
                else
                {
                    ExtraObjects.Add(SliceHolder.transform.GetChild(i).gameObject);
                    ExtraObjects.Add(ItemsHolder.transform.GetChild(i).gameObject);
                }
                }

            if (ExtraObjects.Count > 1) {
                DestroyExtraObjects();
            }
            

        }

        if (SliceHolder.transform.childCount != NoOfItems.Length)
        {
            for (int i = SliceHolder.transform.childCount; i < NoOfItems.Length; i++)
            {
                SliceProperties temp = NoOfItems[i];
                var CurrentSlice = Instantiate(SlicePrefab, SliceHolder.transform);
                WheelSlice Slice = CurrentSlice.GetComponent<WheelSlice>();
                Slice.UpdateSlice(temp.SliceColor, GetFillAmount(), GetZangle(i), Spacing);
                temp.ZAngle = GetZangle(i);

                var CurrentPrizeitem = Instantiate(PrizeitemPrefab, ItemsHolder.transform);
                Prizeitem item = CurrentPrizeitem.GetComponent<Prizeitem>();
                item.UpdatePrizeItem(temp.ItemImage, temp.Prizetext, temp.ZAngle + (360 / NoOfItems.Length) / 2);


            }
        }
    }

    //Invoked From Update After SpinStop
    void DisplayPrize()
    {
        PrizePanel.SetActive(true);
     //   Wheelparent.SetActive(false);
        Prizetext.text = PrizeSlice.Prizetext;
        AudioController.instance.OnFortuneWin();
        UIManager.instance.lobbyView.RefreshCoins();
        PrizeSlice = null;
    }

    void OkBtnClick()
    {

        PrizePanel.SetActive(false);
        DisableFortuneWheelPanelPanelView();
       // Wheelparent.SetActive(true);
      //  gameObject.SetActive(false);
        AudioController.instance.StopSounds();

    }

    float GetFillAmount()
    {
        return (float)1 / NoOfItems.Length;
    }
    float GetZangle(int ItemNo)
    {
        return 360 / NoOfItems.Length * ItemNo;
    }

    void DestroyExtraObjects() {

        foreach (var Obj in ExtraObjects)
        {
           
            DestroyImmediate(Obj);
            
        }
        ExtraObjects.Clear();
    }
    void OnFreeSpinAvailable() {

        IsFreeSpinAvaible = true;
        TimerText.text = "Spin Wheel For Free Coins";
        SpinBtn.interactable = true;
        
    }

   public void RefreshTimer() {

        IsFreeSpinAvaible = false;
        SpinBtn.interactable = false;
        SpinCount = 0;
        SlowSpeed = 1;
        PlayerPrefs.SetString("LastFortuneWheelSpin", DateTime.Now.Ticks.ToString());
        LastSpinTime = ulong.Parse(PlayerPrefs.GetString("LastFortuneWheelSpin"));

    }
}
[System.Serializable]
public class SliceProperties
{
    [HideInInspector]
    public float ZAngle;
    public string Prizetext;
    public Sprite ItemImage;
    public Color SliceColor;
}
