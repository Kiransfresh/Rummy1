using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HintScript : MonoBehaviour
{
    private Button PanelClsBtn;
    private void Start()
    {
        PanelClsBtn = GetComponent<Button>();

        if (PanelClsBtn != null)
        {
            PanelClsBtn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);

            });
        }
    }
}
