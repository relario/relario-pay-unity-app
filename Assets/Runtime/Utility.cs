using Relario.Network.Models;

namespace Relario {
    public static class Utility {
        public static string paymentTypeToString (PaymentType paymentType) {
            switch (paymentType) {
                case PaymentType.sms:
                    return "sms";
                case PaymentType.voice:
                    return "voice";
                default:
                    return paymentType.ToString ();
            }
        }
    }
}