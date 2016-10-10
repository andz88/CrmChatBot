using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk;
using CrmChatBot.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xrm.Tools.WebAPI;

namespace CrmChatBot.CRM
{
    [Serializable]
    public class CrmLead
    {
        #region CRM Attributes
        public static string EntityName = "lead";
        public static string Field_Subject = "subject";
        public static string Field_Description = "description";
        public static string Field_FirstName = "firstname";
        #endregion

        public static void CreateTestDrive(TestDriveDetail testDrive, CRMWebAPI api)
        {
            Task.Run(async () =>
            {
                dynamic data = new ExpandoObject();
                data.subject = $"Test Drive Request by {testDrive.CustomerName}";
                data.firstname = testDrive.CustomerName;
                data.description = $@"Test drive request summary:
                                    {Environment.NewLine}Car Make: {testDrive.CarMake},
                                    {Environment.NewLine}Car Model: {testDrive.CarModel},
                                    {Environment.NewLine}Requested Time: {testDrive.RequestedTime},
                                    {Environment.NewLine}Customer Name: {testDrive.CustomerName},
                                    {Environment.NewLine}Phone Number: {testDrive.PhoneNumber}";


                var leadGuid = await api.Create("leads", data);
            });                        

        }

        public static void CreateTestDrive(TestDriveDetail testDrive, IOrganizationService crmService)
        {
            var lead = new Microsoft.Xrm.Sdk.Entity(EntityName);
            //lead.Attributes
            lead.Attributes.Add(Field_Subject, $"Test Drive Request by {testDrive.CustomerName}");
            lead.Attributes.Add(Field_FirstName, testDrive.CustomerName);
            lead.Attributes.Add(Field_Description, $@"Test drive request summary:
                                    {Environment.NewLine}Car Make: {testDrive.CarMake},
                                    {Environment.NewLine}Car Model: {testDrive.CarModel},
                                    {Environment.NewLine}Requested Time: {testDrive.RequestedTime},
                                    {Environment.NewLine}Customer Name: {testDrive.CustomerName},
                                    {Environment.NewLine}Phone Number: {testDrive.PhoneNumber}");            

            crmService.Create(lead);
        }
    }
}