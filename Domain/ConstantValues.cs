namespace Domain
{
    public static class ConstantValues
    {
        public const string XSRFTokenKey = "X-BZXSRF-TOKEN";
        public const string BZAuth = ".BZAUTH";
        public const string AspNetSessionId = "ASP.NET_SessionId";
        public const string SyncTimerTimeout = "syncTimerTimeout";
        public const string AlwaysAsk = "alwaysAsk";
        public const string SetCookie = "Set-Cookie";
        public const string Cookie = "Cookie";
        public const string Domain = "domain";
        public const string LoginOptions = "loginOptions";
        public const string Success = "success";
        public const string Admon = "admon";
        public const string Data = "data";
        public const string Error = "error";
        public const string ValidationMessages = "validationMessages";
        public const string AlertMessages = "alertMessages";
        public const string WorkportalException = "WorkportalException";
        public const string ValidationException = "ValidationException";
        public const string SoapAction = "SOAPAction";
        public const string TimestampTykModel = "@timestamp";
        public const string ApiNameKeyword = "api_name.keyword";
        public const int ElasticMax = 10000;
        public const string Keyword = ".keyword";
        public static string InputMethod = string.Empty;
        public const string FailedLicenseResponse = "FAILED_LICENSE_RESPONSE";
        public const string NullValue = "NULL";
        public const string SpSyncSsoWithBizagi = "EXEC BizagiAPI_TransferSsoUsersToBizagiUsers";
        public const string Female = "Female";
        public const string Male = "Male";
        public const string Table = "Table";
        public const string Bar = "Bar";
        public const string Pie = "Pie";
        public const string Pid = "pid";
        public const string Number = "Number";
        public const string LicenseSubmitRequestUrl = "/license";
        public const string DateFormatWithoutMark = "yyyyMMddHHmmss";
        public const string RequestId = "requestId";
        public const string Basic = "Basic";
        public const string UserName = "username";
        public const string Password = "password";
        public const string Ext_UserName = "ext_username";
        public const string Ext_Password = "ext_password";
        public const string GrantType = "grant_type";
        public const string Bearer = "Bearer";
        public const string FormUrlEncodedContent = "FormUrlEncodedContent";
        public const string ClientCredentials = "client_credentials";
        public const string CorsPolicySpecificOrigins = "CorsPolicySpecificOrigins";
        public const string ButtonRule = "buttonRule";
        public const string Referrer = "referrer";
        public const string Idle = "Idle";
        public const string UserKey = "USER_KEY";
        public const string BundleId = "bundle-id";
        public const string ContentType = "Content-Type";
        public const string FdaApikey = "fda-api-key";
        public const string Authorization = "Authorization";
        public const string fda_api_key = "fda-api-key";
        public const string X_Authorization = "X_Authorization";

        public static string FrontUrlForVerifyPayment(string frontUrl, bool isPaid) => $"{frontUrl}?isPaid={isPaid}";

        public static class Organizations
        {
            public const string Varzesh = "varzesh";
            public const string Daroo = "Daroo";
            public const string Kanoon = "Kanoon";
            public const string Moshavere = "Moshavere";
            public const string Estekhdam = "Estekhdam";
            public const string Hamyari = "hamyari";
            public const string Payment = "Payment";
            public const string Evaluation = "Evaluation";
        }

        public static class HAction
        {
            public const string LoadForm = "LOADFORM";
            public const string LoadStartForm = "LOADSTARTFORM";
            public const string ProcessPropertyValue = "PROCESSPROPERTYVALUE";
            public const string SubmitOnChange = "SUBMITONCHANGE";
            public const string AddRelation = "ADDRELATION";
            public const string EditRelation = "EDITRELATION";
            public const string RemoveRelation = "REMOVERELATION";
            public const string SaveRelation = "SAVERELATION";
            public const string RollBack = "ROLLBACK";
            public const string InvalidatePageCache = "INVALIDATEPAGECACHE";
            public const string SubmitData = "SUBMITDATA";
            public const string CheckPoint = "CHECKPOINT";
            public const string Commit = "COMMIT";

            public const string MultiAction = "multiaction";
            public const string CreateNewCase = "createnewcase";
            public const string PerformAction = "performAction";
            public const string Save = "save";
            public const string Next = "next";
            public const string NextWithoutValidation = "nextWithoutValidations";
            public const string ReassignCase = "reassignItems";
        }

        public static class HTag
        {
            public const string SubmitData = "submitData";
            public const string CheckPoint = "checkpoint";
            public const string ExecuteRule = "executeRule";
            public const string Commit = "commit";
        }

        public static class RequestedForm
        {
            public const string AddForm = "addForm";
            public const string EditForm = "editForm";
            public const string DetailForm = "detailForm";
        }

        public static class CacheKey
        {
            public const string PostToken = "PostToken";
        }
    }
}
