using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _tapToPlayText;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject touchCanvas;

    void Start()
    {
        menuPanel.SetActive(true);
        touchCanvas.SetActive(false); 

        _tapToPlayText.transform.DOScale(1.1f, 0.5f).SetLoops(10000, LoopType.Yoyo).SetEase(Ease.InOutFlash);

        EventTrigger trigger = menuPanel.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = menuPanel.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { OnPlayButtonPressed(); });
        trigger.triggers.Add(entry);
    }

    public void OnPlayButtonPressed()
    {
        menuPanel.SetActive(false);
        touchCanvas.SetActive(true);
    }
}
