using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using Microsoft.SharePoint.Client;
using File = Microsoft.SharePoint.Client.File;
using System.Configuration;


namespace ThroughputCalculation.GetTheData
{
    public class WriteToFile
    {
        readonly string userName = ConfigurationManager.AppSettings["user"];
        readonly string password = ConfigurationManager.AppSettings["pass"];
        readonly string site = @"https://collaboration.volvocars.net/sites/dspa/";
        readonly string fileLocation = @"https://collaboration.volvocars.net/sites/dspa/Common%20Services/KPI_Leadtimes/";
        readonly string fileName = "times.csv";

      
        internal void Write(double sOLeadTime, double sOCycleTime, double uraxLeadTime, double uraxCycleTime, double cLELeadTime, double cLECycleTime)
        {
           string filePath = DownloadFilesFromSharePoint();

            if (filePath.Equals(""))
            {
                filePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".csv";
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate,FileAccess.ReadWrite))
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
            System.IO.File.AppendAllText(filePath, line + Environment.NewLine);


            uploadDocument(filePath);

            System.IO.File.Delete(filePath);
        }


        public void uploadDocument(string filePath)
        {
            Console.WriteLine("Start Uploading of Document");
            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            using (var clientContext = new ClientContext(site))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassword);
                Web web = clientContext.Web;
                clientContext.Load(web, a => a.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                List documentsList = clientContext.Web.Lists.GetByTitle("Common Services");

                var fileCreationInformation = new FileCreationInformation();
                //Assign to content byte[] i.e. documentStream

                fileCreationInformation.Content = System.IO.File.ReadAllBytes(filePath);
                //Allow owerwrite of document

                fileCreationInformation.Overwrite = true;
                //Upload URL

                fileCreationInformation.Url = fileLocation + fileName;

                File uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);

                //Update the metadata for a field having name "DocType"
                uploadFile.ListItemAllFields["Title"] = "UploadedviaCSOM";

                uploadFile.ListItemAllFields.Update();
                clientContext.ExecuteQuery();

            }
            Console.WriteLine("Uploading of Document Completed");
        }

 
        public string DownloadFilesFromSharePoint()
        {

            Console.WriteLine("Downloading Files Started");
            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
                
            }
            ConfigurationManager.AppSettings["pass"] = securePassword.ToString();
            using (var clientContext = new ClientContext(site))
            {
                clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassword);
                Web web = clientContext.Web;
                clientContext.Load(web, a => a.ServerRelativeUrl);
                clientContext.ExecuteQuery();

                FileCollection files = clientContext.Web.GetFolderByServerRelativeUrl(fileLocation).Files;

                clientContext.Load(files);
                clientContext.ExecuteQuery();
                foreach (File file in files)
                {
                    FileInformation fileInfo = File.OpenBinaryDirect(clientContext, file.ServerRelativeUrl);
                    clientContext.ExecuteQuery();

                    var filePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".csv";
                    FileStream fileStream = new FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);

                    fileInfo.Stream.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                    Console.WriteLine("Downloading Files Completed");
                    return filePath;

                }
            }
            Console.WriteLine("Downloading Files Completed");
            return "";


        }
    }
}