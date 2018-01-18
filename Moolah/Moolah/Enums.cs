using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moolah
{
    public enum PaymentEnvironment
    {
        Test = 0,
        Live = 1
    }

    public enum Cv2AvsPolicy
    {
        [Display(Name = "0: Unspecified - Will use the policy set by the merchant account")]
        UNSPECIFIED = 0,

        [Display(Name = "1: The AVS elements have been checked and the data matched")]
        AVS = 1,

        [Display(Name = "2: The CV2 element has been checked and the data matched")]
        CV2 = 2,

        [Display(Name = "3: All elements have been checked and all the data matched")]
        AVS_CV2 = 3,

        [Display(Name = "5: Either the AVS elements have been checked and matched, or all elements returned not checked")]
        AVS_NOTCHECKED = 5,

        [Display(Name = "6: Either the CV2 element has been checked and matched or all elements returned not checked")]
        CV2_NOTCHECKED = 6,

        [Display(Name = "7: Either all the elements have been checked and matched, or all elements returned not checked")]
        AVS_CV2_NOTCHECKED = 7,
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Successful = 2,
        Failed = 3,
    }
}