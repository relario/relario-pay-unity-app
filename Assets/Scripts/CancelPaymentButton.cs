using System;
using Relario.Network.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Android;
using Relario;

public class CancelPaymentButton : MonoBehaviour
{
    public RelarioPay relarioPay;
    [Header("Options")]
    public bool cancelLastPayment;
    public string productId;

    [Header("Callbacks")]
    public UnityEvent onCancel;

    void Start()
    {
        relarioPay = FindObjectOfType<RelarioPay>();
        GetComponent<Button>().onClick.AddListener(OnPaymentClick);
    }

    void OnPaymentClick()
    {
        if (cancelLastPayment)
        {
            var transaction = relarioPay.GetLastSubscriptionTransaction();
            relarioPay.CancelSubscription(productId: transaction.productId);
        }else{
            relarioPay.CancelSubscription(productId: productId);
        }
        onCancel.Invoke();
    }
}