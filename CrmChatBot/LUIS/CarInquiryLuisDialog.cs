using CrmChatBot.CRM;
using CrmChatBot.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CrmChatBot.LUIS
{
    [LuisModel("LuisAppId", "LUIS Subscription Key")]
    [Serializable]
    public class CarInquiryLuisDialog : LuisDialog<object>
    {
        protected TestDriveDetail testDriveDetail;
        public const string Entity_Car_Make = "CarMake";
        public const string Entity_Car_Model = "CarModel";
        public const string Entity_Date = "builtin.datetime.date";
        private EntityRecommendation carMake;
        private EntityRecommendation carModel;
        private EntityRecommendation preferredDate;

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string message = $"Hi, is there anything that I could help you today?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Ending Conversation")]
        public async Task Ending(IDialogContext context, LuisResult result)
        {
            string message = $"Thanks for using Car Inquiry Bot. Hope you have a great day!";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Test Drive")]
        public async Task TestDrive(IDialogContext context, LuisResult result)
        {
            testDriveDetail = new TestDriveDetail();            

            PromptDialog.Text(
                context: context,
                resume: CarMakeHandler,
                prompt: "What car make do you want to test?",
                retry: "Sorry, I don't understand that."
            );
        }

        [LuisIntent("Brochure Request")]
        public async Task BrocureRequest(IDialogContext context, LuisResult result)
        {
            testDriveDetail = new TestDriveDetail();

            PromptDialog.Text(
                context: context,
                resume: BrochureRequestHandler,
                prompt: "Which car do you want to get the brochure information?",
                retry: "Sorry, I don't understand that."
            );
        }

        #region Brochure Request Handler
        public virtual async Task BrochureRequestHandler(IDialogContext context, IAwaitable<string> argument)
        {
            var car = await argument;
            await context.PostAsync($"We have received your brochure request for {car}. Our sales team will send it out to you.");

            PromptDialog.Confirm(
                context: context,
                resume: AnythingElseHandler,
                prompt: "Is there anything else that I could help?",
                retry: "Sorry, I don't understand that."
            );
        }
        #endregion

            #region Test Drive Prompt
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

            PromptDialog.Confirm(
                context: context,
                resume: AnythingElseHandler,
                prompt: "Is there anything else that I could help?",
                retry: "Sorry, I don't understand that."
            );

        }

        #endregion

        public async Task AnythingElseHandler(IDialogContext context, IAwaitable<bool> argument)
        {
            var answer = await argument;
            if (answer)
            {
                await GeneralGreeting(context, null);
            }
            else
            {
                string message = $"Thanks for using Car Inquiry Bot. Hope you have a great day!";
                await context.PostAsync(message);
                context.Done<string>("conversation ended.");
            }
        }

        public virtual async Task GeneralGreeting(IDialogContext context, IAwaitable<string> argument)
        {
            string message = $"Great! What else that can I help you?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}