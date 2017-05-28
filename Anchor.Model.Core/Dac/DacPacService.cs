using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dac;

namespace Anchor.Model.Core
{
    public class DacPacService
    {
        public DacPacService()
        {
            MessageList = new List<string>();
        }

        public List<string> MessageList { get; set; }

        public bool ProcessDacPac(string connectionString,
            string databaseName,
            string dacpacName)
        {
            var success = true;

            MessageList.Add("*** Start of processing for " +
                            databaseName);

            var dacOptions = new DacDeployOptions()
            {
                BlockOnPossibleDataLoss = false
            };
            var dacServiceInstance = new DacServices(connectionString);
            dacServiceInstance.ProgressChanged +=
                (s, e) =>
                    MessageList.Add(e.Message);
            dacServiceInstance.Message +=
                (s, e) =>
                    MessageList.Add(e.Message.Message);

            try
            {
                using (var dacpac = DacPackage.Load(dacpacName))
                {
                    dacServiceInstance.Deploy(dacpac, databaseName,
                        true,
                        dacOptions);
                }
            }
            catch (Exception ex)
            {
                success = false;
                MessageList.Add(ex.Message);
            }

            return success;
        }
    }
}