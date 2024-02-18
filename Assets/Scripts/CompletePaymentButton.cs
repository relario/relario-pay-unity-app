using System;
using Relario.Network.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Android;
using Relario;

public class CompletePaymentButton : MonoBehaviour
{
    RelarioPay relarioPay;

    [Header("Callbacks")]
    public UnityEvent TransactionLaunch;
    public UnityEvent TransactionComplete, TransactionFailed, PartialPayments;



    void Start()
    {
        relarioPay = FindObjectOfType<RelarioPay>();
        if (relarioPay)
        {
            relarioPay.OnSuccessfulPay += OnTransactionPaid;
            relarioPay.OnFailedPay += OnTransactionFailed;
            relarioPay.OnPartialPay += OnTransactionPartial;
        }
        else
        {
            Debug.LogError("Ensure that you have at least one GameObect with RelarioPay attached to it");
        }
        GetComponent<Button>().onClick.AddListener(OnPaymentClick);
    }

    void OnPaymentClick()
    {
        var transaction = relarioPay.GetLastSubscriptionTransaction();
        relarioPay.RetryTransaction(transaction);
    }

    void OnTransactionPaid(Transaction transaction)
    {
        TransactionComplete.Invoke();
        // payOnTap = false;
    }

    void OnTransactionPartial(Transaction transaction)
    {
        PartialPayments.Invoke();
    }

    void OnTransactionFailed(Exception exception, Transaction transaction)
    {
        TransactionFailed.Invoke();
    }

}