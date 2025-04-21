using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RayCastUI : MonoBehaviour
{

    float begTime = 0.0f;

    bool inputActive = false;

    uiHit onHit;

    GraphicRaycaster overlayCanvas_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    GameObject RaycastPointObj;

    public void enableRayCast(bool state, uiHit _currentUIHit, GameObject RayCastObj)
    {

        RaycastPointObj = RayCastObj;
        onHit = _currentUIHit;
        inputActive = state;
        overlayCanvas_Raycaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();
    }

    public void disableRayCast()
    {
        inputActive = false;
    }



    private void Update()
    {
        if (inputActive)
        {
            raycastUI();
        }
    }


    void raycastUI()
    {

        if (Input.touchCount > 0)
        {


            m_PointerEventData = new PointerEventData(m_EventSystem);

            m_PointerEventData.position = RaycastPointObj.transform.position;

            List<RaycastResult> results = new List<RaycastResult>();


            overlayCanvas_Raycaster.Raycast(m_PointerEventData, results);
            onHit(results[0]);



        }


        if (Input.GetMouseButton(0) && RaycastPointObj != null)
        {



            begTime = Time.time;
            m_PointerEventData = new PointerEventData(m_EventSystem);

            m_PointerEventData.position = RaycastPointObj.transform.position;

            List<RaycastResult> results = new List<RaycastResult>();


            overlayCanvas_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
                onHit(results[0]);






        }


    }

    public void RaycastOneFrame(uiHit _currentUIHit, GameObject RayPointObj)
    {




        m_PointerEventData = new PointerEventData(m_EventSystem);

        m_PointerEventData.position = RayPointObj.transform.position;

        List<RaycastResult> results = new List<RaycastResult>();


        overlayCanvas_Raycaster.Raycast(m_PointerEventData, results);



        if (results.Count > 0)
            _currentUIHit(results[0]);






    }
}
