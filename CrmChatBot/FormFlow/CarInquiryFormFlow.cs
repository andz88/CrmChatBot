using CrmChatBot.CRM;
using CrmChatBot.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmChatBot.FormFlow
{
    public enum CarMakeOptions { Unknown, Honda, Toyota };

    public enum CarModelOptions { Unknown, Jazz, City, CRV, Accord, HRV, Yaris, Corolla, Camry };
    
    [Serializable]
    public class CarInquiryFormFlow
    {
        public CarMakeOptions CarMake;
        public CarModelOptions CarModel;
        public string PreferredTime;
        public string Name;
        public string ContactNumber;

        public static IForm<CarInquiryFormFlow> BuildForm()
        {
            OnCompletionAsyncDelegate<CarInquiryFormFlow> processRequest = async (context, state) =>
            {
                await context.PostAsync($@"Your test drive request summary:
                                    {Environment.NewLine}Car Make: {state.CarMake.ToString()},
                                    {Environment.NewLine}Car Model: {state.CarModel.ToString()},
                                    {Environment.NewLine}Requested Time: {state.PreferredTime},
                                    {Environment.NewLine}Customer Name: {state.Name},
                                    {Environment.NewLine}Phone Number: {state.ContactNumber}");

                var testDriveDetail = new TestDriveDetail
                {
                    CarMake = state.CarMake.ToString(),
                    CarModel = state.CarModel.ToString(),
                    RequestedTime = state.PreferredTime,
                    CustomerName = state.Name,
                    PhoneNumber = state.ContactNumber
                };

                // save the data to CRM
                CrmLead.CreateTestDrive(testDriveDetail, CrmDataConnection.GetOrgService());
            };

            return new FormBuilder<CarInquiryFormFlow>()
                   .Message("Welcome to the car test drive bot!")
                   .Field(nameof(CarMake))
                   .Field(nameof(CarModel))
                   .Field(nameof(PreferredTime))
                   .Field(nameof(Name))
                   .Field(nameof(ContactNumber))
                   .AddRemainingFields()
                   .Message("Thank you for your interest, your request has been logged. Our sales team will get back to you shortly.")
                   .OnCompletion(processRequest)
                   .Build();
        }


    }
}