using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using CrmChatBot.Model;
using CrmChatBot.CRM;

namespace CrmChatBot.Dialogs
{
    [Serializable]
    public class CarInquiryDialog :IDialog<object>
    {
        protected TestDriveDetail testDriveDetail;

        public async Task StartAsync(IDialogContext context)
        {            
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            //CrmDataConnection.GetAPI();

            if (message.Text.Contains("test drive"))
            {
                testDriveDetail = new TestDriveDetail();

                PromptDialog.Text(
                    context: context,
                    resume: CarMakeHandler,
                    prompt: "What car make do you want to test?",
                    retry: "Sorry, I don't understand that."
                );
            }
            else if (message.Text == "No")
            {
                    
            }
            else
            {
                await context.PostAsync("Hi there, anything that I can help for you today?");
                context.Wait(MessageReceivedAsync);
            }
        }

        public virtual async Task CarMakeHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var carMake = await argument;
            testDriveDetail.CarMake = carMake;
            PromptDialog.Text(
                context: context, 
                resume: CarModelHandler, 
                prompt: "What car model do you want to test?", 
                retry: "Sorry, I don't understand that."
            );
        }


        public async Task CarModelHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var carModel = await argument;
            testDriveDetail.CarModel = carModel;
            PromptDialog.Text(
                context: context, 
                resume: PreferredTimeHandler, 
                prompt: "When would you like to come for test drive?", 
                retry: "Sorry, I don't understand that."
            );
        }

        public async Task PreferredTimeHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var prefTime = await argument;
            testDriveDetail.RequestedTime = prefTime;
            PromptDialog.Text(
                context: context, 
                resume: CustomerNameHandler,
                prompt: "Your name please?",
                retry: "Sorry, I don't understand that."
            );
        }

        public async Task CustomerNameHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var customerName = await argument;
            testDriveDetail.CustomerName = customerName;
            PromptDialog.Text(
                context: context,
                resume: ContactNumberHandler,
                prompt: "What is the best number to contact you?",
                retry: "Sorry, I don't understand that."
            );
        }

        public async Task ContactNumberHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var contactNumber = await argument;
            testDriveDetail.PhoneNumber = contactNumber;

            await context.PostAsync($@"Thank you for your interest, your request has been logged. Our sales team will get back to you shortly.
                                    {Environment.NewLine}Your test drive request summary:
                                    {Environment.NewLine}Car Make: {testDriveDetail.CarMake},
                                    {Environment.NewLine}Car Model: {testDriveDetail.CarModel},
                                    {Environment.NewLine}Requested Time: {testDriveDetail.RequestedTime},
                                    {Environment.NewLine}Customer Name: {testDriveDetail.CustomerName},
                                    {Environment.NewLine}Phone Number: {testDriveDetail.PhoneNumber}");

            //CrmLead.CreateTestDrive(testDriveDetail, CrmDataConnection.GetAPI());

            CrmLead.CreateTestDrive(testDriveDetail, CrmDataConnection.GetOrgService());

            context.Done<string>("Test drive has been logged");

        }        
    }
}