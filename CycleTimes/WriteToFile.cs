using System;
using System.IO;
using System.Security;
using System.Text;
using Microsoft.SharePoint.Client;




namespace ThroughputCalculation.GetTheData
{
    public class WriteToFile
    {



        internal void Write(double sOLeadTime, double sOCycleTime, double uraxLeadTime, double uraxCycleTime, double cLELeadTime, double cLECycleTime)
        {
            
            if(!System.IO.File.Exists("times.csv"))
            {
                using (FileStream fs = new FileStream("times.csv", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                string headfing = string.Format("{0};{1};{2};{3};{4};{5};{6}", "Date", "Soft Offer Lead Time", "Soft Offer Cycle Time", "URAX Lead Time", "URAX Cycle Time", "CLE Lead Time", "CLE Cycle Time");
                    byte[] array = Encoding.ASCII.GetBytes(headfing + Environment.NewLine);
                    for(int i = 0; i<array.Length; i++)
                    {
                        fs.WriteByte(array[i]);
                    }
                    
                }
            }
            var csv = new StringBuilder();
            string line = string.Format("{0};{1};{2};{3};{4};{5};{6}", DateTime.Now, sOLeadTime, sOCycleTime, uraxLeadTime, uraxCycleTime, cLELeadTime, cLECycleTime);
            csv.AppendLine(line);
            System.IO.File.AppendAllText("times.csv", line + Environment.NewLine);


            uploadDocument();
        }


        public void uploadDocument()
        {
            string userName = "saleborg@volvocars.com";
            string password = "2VitHund!";
            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            using (var clientContext = new ClientContext("https://collaboration.volvocars.net/sites/dspa/"))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassword);
                Web web = clientContext.Web;
                clientContext.Load(web, a => a.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                List documentsList = clientContext.Web.Lists.GetByTitle("Common Services");

                var fileCreationInformation = new FileCreationInformation();
                //Assign to content byte[] i.e. documentStream

                fileCreationInformation.Content = System.IO.File.ReadAllBytes("times.csv");
                //Allow owerwrite of document

                fileCreationInformation.Overwrite = true;
                //Upload URL

                fileCreationInformation.Url = "https://collaboration.volvocars.net/sites/dspa/Common%20Services/KPI_Leadtimes/times.csv";

                Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);

                //Update the metadata for a field having name "DocType"
                uploadFile.ListItemAllFields["Title"] = "UploadedviaCSOM";

                uploadFile.ListItemAllFields.Update();
                clientContext.ExecuteQuery();

            }
           


        }
    }
}