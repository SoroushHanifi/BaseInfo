namespace Bizagi.Application.Models
{
    public class BizagiApiResponse : RedirectToExistLicenseModel
    {
        public ActionResultStatus Status { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string HelpString { get; set; }
        public string ErrorFrom { get; set; }
        public object Object { get; set; }

        public string G4bErrorMessage { get; set; }
        public string G4bErrorCreateCase { get; set; }
        public bool RedirectToExistLicense { get; set; } = false;
    }

    public enum ActionResultStatus
    {
        None,
        Complete,
        ValidationError,
        Error,
        LoginFailed,
        Exception,
        NoAccess,
        FormIsNotPresent,
        ReloadPage
    }


    public class RedirectToExistLicenseModel
    {
        public string RedirectUrl { get; set; }
        public bool IsRedirect { get; set; } = false;
    }
}
