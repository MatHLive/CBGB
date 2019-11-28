using cbgb.Model.Enum;
using cbgb.Resources;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace cbgb.Sheet
{
    class Service
    {
        UserCredential credential;
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "cbgb";

        public SheetsService SheetService { get; private set; }
        public Service()
        {
            Credential();
            SheetService = CreateSheetService();
        }

        private void Credential()
        {
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                //new FileStream(Resource.GetPath(EPaths.cred), FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken .None,
                    new FileDataStore(credPath, true)).Result;
            }

        }

        private SheetsService CreateSheetService()
        {
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }
    }
}
