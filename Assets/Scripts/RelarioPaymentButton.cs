using System;
using Relario.Network.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Android;
using Relario;
using System.Collections.Generic;
using System.Collections;

public enum PurchaseType
{
    consumable,
    subscription
}

public class RelarioPaymentButton : MonoBehaviour
{
    RelarioPay relarioPay;

    [Header("Purchase Type")]
    public PurchaseType purchaseType;

    [Header("Payment Options")]
    public string productId;
    public string productName;
    public int smsCount;

    [Header("Subscription Options")]
    public TimeUnit timeUnit;
    public int intervalRate;

    [Header("(Optional)")]
    public string customerId;
    public bool canRetryFailedPurchase;

    [Header("Callbacks")]
    public UnityEvent TransactionLaunch;
    public UnityEvent TransactionComplete, TransactionFailed, PartialPayments;

    Coroutine checkingRoutine;

    void Start()
    {
        relarioPay = FindObjectOfType<RelarioPay>();
        if (!relarioPay)
        {
            Debug.LogError("Ensure that you have at least one GameObect with RelarioPay attached to it");
        }
        GetComponent<Button>().onClick.AddListener(OnPaymentClick);
    }

    private void OnDisable()
    {
        SubscribeEvents(false);
    }

    void SubscribeEvents(bool subscribe)
    {
        if (subscribe)
        {
            relarioPay.OnSuccessfulPay += OnTransactionPaid;
            relarioPay.OnFailedPay += OnTransactionFailed;
            relarioPay.OnPartialPay += OnTransactionPartial;
        }
        else
        {
            relarioPay.OnSuccessfulPay -= OnTransactionPaid;
            relarioPay.OnFailedPay -= OnTransactionFailed;
            relarioPay.OnPartialPay -= OnTransactionPartial;
        }
    }

    void OnPaymentClick()
    {
        SubscribeEvents(true);

        if (purchaseType == PurchaseType.consumable)
        {
            relarioPay.Pay(smsCount, productId, productName, customerId, (exception, transaction) =>
                {
                    Debug.Log("Consumable Transaction started");
                }
            );
        }
        else if (purchaseType == PurchaseType.subscription)
        {
            SubscriptionOptions options = new SubscriptionOptions(
                productId: productId,
                smsCount: smsCount,
                productName: productName,
                customerId: Guid.NewGuid().ToString(),
                intervalRate: intervalRate,
                timeUnit: timeUnit
            );
            relarioPay.Subscribe(options);

            if (checkingRoutine != null)
            { StopCoroutine(checkingRoutine); }

            checkingRoutine = StartCoroutine(CheckSubscriptionTransactionStatus());
            Debug.Log("Subscription Transaction started");
        }
        TransactionLaunch.Invoke();
    }

    void OnTransactionPaid(Transaction transaction)
    {
        TransactionComplete.Invoke();
        SubscribeEvents(false);
    }

    void OnTransactionPartial(Transaction transaction)
    {
        PartialPayments.Invoke();
        SubscribeEvents(false);
    }

    void OnTransactionFailed(Exception exception, Transaction transaction)
    {
        TransactionFailed.Invoke();
        SubscribeEvents(false);
    }

    //Subscription checking method
    private IEnumerator CheckSubscriptionTransactionStatus()
    {
        yield return new WaitForSeconds(relarioPay.intervalOfTransactionChecks);
        Transaction currentTransaction = relarioPay.GetLastSubscriptionTransaction();

        if (currentTransaction == null)
        {
            OnTransactionFailed(new Exception("Failed to check transaction"), null);
            //having to reference UI controller here
            UIController.instance.HandleTransactionFailed(new Exception("Failed to check transaction"), null);
        }
        else if (currentTransaction.IsFullyPaid())
        {
            OnTransactionPaid(currentTransaction);
            //having to reference UI controller here
            UIController.instance.HandleTransactionPaid(currentTransaction);
        }
        else if (currentTransaction.IsPartiallyPaid())
        {
            OnTransactionPartial(currentTransaction);
            //having to reference UI controller here
            UIController.instance.HandlePartialPaymentsRecieved(currentTransaction);
        }
        else if (currentTransaction.IsNotPaid())
        {
            OnTransactionFailed(null, currentTransaction);
            //having to reference UI controller here
            UIController.instance.HandleTransactionFailed(null, currentTransaction);
        }
        yield break;
    }
}