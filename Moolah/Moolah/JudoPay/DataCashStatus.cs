using System;

namespace Moolah.JudoPay
{
    /// <summary>
    /// See https://testserver.datacash.com/software/returncodes.shtml for full list.
    /// </summary>
    public static class DataCashStatus
    {
        public const int Success = 1;
        public const int NotAuthorised = 7;
        public const int RequiresThreeDSecureAuthentication = 150;
        static readonly int[] CanAuthoriseStatuses = new[] { 158, 159, 160, 162, 163, 164, 165, 173, 170, 183, 186, 187, 188, 189 };

        /// <summary>
        /// Whether this transaction can be immediately authorised without 3DS payer verification.
        /// </summary>
        public static bool CanImmediatelyAuthorise(int dataCashStatus)
        {
            return Array.IndexOf(CanAuthoriseStatuses, dataCashStatus) != -1;
        }

        public static bool IsSystemFailure(int dataCashStatus)
        {
            return dataCashStatus != Success && dataCashStatus != NotAuthorised &&
                   dataCashStatus != RequiresThreeDSecureAuthentication
                   && !JudoPayFailureReasons.CleanFailures.ContainsKey(dataCashStatus);
        }

        public static JudoPayFailureReason FailureReason(int dataCashStatus)
        {
            JudoPayFailureReason failureReason;
            
            if (IsSystemFailure(dataCashStatus))
                return JudoPayFailureReasons.SystemFailures.TryGetValue(dataCashStatus, out failureReason)
                                              ? failureReason
                                              : new JudoPayFailureReason(string.Format("Unknown DataCash status code: {0}", dataCashStatus), CardFailureType.General);

            JudoPayFailureReasons.CleanFailures.TryGetValue(dataCashStatus, out failureReason);
            return failureReason;
        }
    }
}