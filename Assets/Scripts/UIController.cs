using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Relario.Network.Models;
using System;
using Relario;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text partialPaymentsText;
    public GameObject loadingScreen;
    public List<GameObject> subscribeButtons;

    [Header("Notices")]
    public GameObject notifyObj;
    public Image notifyPanel;
    public TMP_Text notifyText;
    public Color normalPanelColor, warnPanelColor;

    [Header("Extras")]
    public GameObject completePaymentBtn;
    public GameObject cancelSubBtn;

    [Header("Subscription Manager")]
    public RelarioPay relarioPay;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        relarioPay.OnSuccessfulPay += HandleTransactionPaid;
        relarioPay.OnFailedPay += HandleTransactionFailed;
        relarioPay.OnPartialPay += HandlePartialPaymentsRecieved;

        if (PlayerPrefs.GetInt("Subscribed") == 1)
        {
            ToggleSubscribeButtons(false);
            ToggleCancelPaymentBtn(true);
        }else{
            ToggleSubscribeButtons(true);
            ToggleCancelPaymentBtn(false);
        }
        ToggleCompletePaymentBtn(false);
        UpdatePartialPaymentsText();
    }

    public void HandleTransactionPaid(Transaction transaction)
    {
        cancelSubBtn.SetActive(true);
        // Handle successful transaction
        NotifyPlayer($"Rewards Granted");
        Debug.Log($"Rewards Granted, {transaction.payments.Count} Payments recieved");
    }

    public void HandleTransactionFailed(Exception exception, Transaction transaction)
    {
        // Handle failed transaction
        NotifyPlayerWarn($"You don't have enough sms units");
        ResetSubscription();
    }

    public void HandlePartialPaymentsRecieved(Transaction transaction)
    {
        int curPartial_Payments = transaction.payments.Count;
        int extPartial_Payments = PlayerPrefs.GetInt("partial_Payments");

        curPartial_Payments += extPartial_Payments;
        PlayerPrefs.SetInt("partial_Payments", curPartial_Payments);

        // Handle partial payments recieved
        //to display content------------- 
        completePaymentBtn.SetActive(true);
        UpdatePartialPaymentsText();
        NotifyPlayerWarn($"There were partial payments made. Reload your airtime and complete the transaction");
        //--------------------------------------------------
    }

    public void ResetPartialPayments()
    {
        PlayerPrefs.SetInt("partial_Payments", 0);
        UpdatePartialPaymentsText();
    }

    public void ResetSubscription()
    {
        ToggleSubscribeButtons(true);
        ToggleCancelPaymentBtn(false);

        PlayerPrefs.SetInt("Subscribed", 0);
    }

    public void ToggleSubscribeButtons(bool state)
    {
        foreach (var btn in subscribeButtons)
        {
            btn.SetActive(state);
        }
    }

    public void ToggleLoadStatus(bool state)
    {
        loadingScreen.SetActive(state);
    }

    public void ToggleCompletePaymentBtn(bool state)
    {
        completePaymentBtn.SetActive(state);
    }

    public void ToggleCancelPaymentBtn(bool state)
    {
        cancelSubBtn.SetActive(state);
    }


    public void UpdatePartialPaymentsText()
    {
        partialPaymentsText.text = "Partial Payments: " + PlayerPrefs.GetInt("partial_Payments").ToString();
    }

    public void NotifyPlayer(string _noticeText)
    {
        notifyPanel.color = normalPanelColor;
        notifyObj.SetActive(true);
        notifyText.text = _noticeText;
    }

    public void NotifyPlayerWarn(string _noticeText)
    {
        notifyPanel.color = warnPanelColor;
        notifyObj.SetActive(true);
        notifyText.text = _noticeText;
    }
}
