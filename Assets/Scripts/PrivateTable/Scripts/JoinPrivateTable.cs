using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinPrivateTable : BaseMonoBehaviour
{
    [SerializeField] private TMP_InputField tableIdField;
    [SerializeField] private Button joinPrivateTableBtn;
    [SerializeField] private Button closePanelBtn;

    void Start()
    {
        joinPrivateTableBtn.onClick.AddListener(() =>
        {
            JoinPrivateTableResponseHandler(tableIdField.text);
            tableIdField.text = "";
            gameObject.SetActive(false);
        });

        closePanelBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
