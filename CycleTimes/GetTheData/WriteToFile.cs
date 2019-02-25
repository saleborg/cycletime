using System;
using System.IO;
using System.Security;
using System.Text;




namespace ThroughputCalculation.GetTheData
{
    internal class WriteToFile
    {



        internal void Write(double sOLeadTime, double sOCycleTime, double uraxLeadTime, double uraxCycleTime, double cLELeadTime, double cLECycleTime)
        {


            var csv = new StringBuilder();


            string line = string.Format("{0};{1};{2};{3};{4};{5};{6}", DateTime.Now, sOLeadTime, sOCycleTime, uraxLeadTime, uraxCycleTime, cLELeadTime, cLECycleTime);
            csv.AppendLine(line);
            File.AppendAllText("C:/temp/times.csv", line);







        }

        internal void uploadDocument()
        {

            string userName = "xxx@xxxx.onmicrosoft.com";
            string password = "xxx";
            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            using (var clientContext = new ClientContext("https://testlz.sharepoint.com/sites/jerrydev"))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassword);
                Web web = clientContext.Web;
                clientContext.Load(web, a => a.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                List documentsList = clientContext.Web.Lists.GetByTitle("Contact");

                var fileCreationInformation = new FileCreationInformation();
                //Assign to content byte[] i.e. documentStream

                fileCreationInformation.Content = System.IO.File.ReadAllBytes(@"D:\document.pdf");
                //Allow owerwrite of document

                fileCreationInformation.Overwrite = true;
                //Upload URL

                fileCreationInformation.Url = "https://testlz.sharepoint.com/sites/jerrydev/" + "Contact/demo" + "/document.pdf";

                Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);

                //Update the metadata for a field having name "DocType"
                uploadFile.ListItemAllFields["Title"] = "UploadedviaCSOM";

                uploadFile.ListItemAllFields.Update();
                clientContext.ExecuteQuery();

            }



        }
    }
}