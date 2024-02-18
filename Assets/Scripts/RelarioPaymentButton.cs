using System;
using Relario.Network.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Android;
using Relario;

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
            Debug.Log("Subscription Transaction started");
        }            
        TransactionLaunch.Invoke();
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