using System;
using System.Net;
using UnityEngine;

namespace Relario.Network.Models {
    [Serializable]
    public class NewTransactionBody {
        [SerializeField]
        private string paymentType = Utility.paymentTypeToString (PaymentType.sms);

        private PaymentType _type;
        public PaymentType type {
            get { return this._type; }
            set {
                this._type = value;
                this.paymentType = Utility.paymentTypeToString (value);
            }
        }

        public int smsCount = 0;
        public int callDuration = 0;
        public string customerMccmncc;
        public string customerId;
        public string customerIpAddress;
        public string productId;
        public string productName;

        public NewTransactionBody (
            PaymentType type,
            int smsCountOrCallDuration,
            string customerMccmncc,
            string customerId,
            string customerIpAddress,
            string productId,
            string productName
        ) {
            this.type = type;

            if (type == PaymentType.sms) {
                this.smsCount = smsCountOrCallDuration;
            } else {
                this.callDuration = smsCountOrCallDuration;
            }

            this.customerMccmncc = customerMccmncc;
            this.customerId = customerId;
            this.customerIpAddress = customerIpAddress;
            this.productId = productId;
            this.productName = productName;
        }

        public string DebugPrint () {
            return "type: " + this.type + " | " +
                "smsCount: " + this.smsCount + " | " +
                "callDuration: " + this.callDuration + " | " +
                "customerMccmncc: " + this.customerMccmncc + " | " +
                "customerId: " + this.customerId + " | " +
                "customerMccmncc: " + this.customerMccmncc + " | " +
                "customerIpAddress: " + this.customerIpAddress + " | " +
                "customerMccmncc: " + this.customerMccmncc + " | " +
                "productId: " + this.productId + " | " +
                "customerMccmncc: " + this.customerMccmncc + " | " +
                "productName: " + this.productName + " | " +
                "customerMccmncc: " + this.customerMccmncc + " | ";
        }
        public string ToJson () {
            return JsonUtility.ToJson (this);
        }

        public string Validate () {
            if (this.type == PaymentType.sms && this.smsCount <= 0) {
                return "smsCount should be more than 0 for SMS payments.";
            } else if (this.type == PaymentType.voice && this.callDuration <= 0) {
                return "callDuration should be more than 0 for Voice payments.";
            }

            if (!ValidateIP (this.customerIpAddress)) {
                return "Invalid customerIpAddress";
            }

            if (String.IsNullOrEmpty (this.productId)) {
                return "productId is required";
            }

            return null;
        }

        public static bool ValidateIP (string IP) {
            IPAddress result = null;
            return !String.IsNullOrEmpty (IP) &&
                IPAddress.TryParse (IP, out result);
        }
    }

}