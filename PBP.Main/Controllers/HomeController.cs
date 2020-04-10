using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmailSender;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using PBP.BusinessLogic;
using PBP.DataAccess;
using PBP.Main.Models;
using PBP.Main.View_Models;
using PBP.Pocos;
using Rotativa.AspNetCore;
using Stripe;

namespace PBP.Main.Controllers
{
    public class HomeController : Controller
    {
        //private DBModel db;
        private readonly IConfiguration _config;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, IWebHostEnvironment env)
        {
            _env = env;
            _config = config;
            _logger = logger;
            //db = new DBModel();
        }

        public IActionResult Index()
        {
            IDataRepository<CalendarPoco> repo = new DataRepository<CalendarPoco>();
            IList<DateTime> UnAvailableDates = repo.GetList(days => days.AvailableSlots <= 0).Select(d => d.Date).ToList();
            
            return View(UnAvailableDates);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(
              string datesReserved
            , string carPlateNumber
            , string email
            , string fullName
            , string phone
            , DateTime transactionDate
            , int numberOfDaysReserved
            , decimal amountPaid)
        {
            var charge = new Charge();
            // Check to see that Payment was successful
            if (charge.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                #region Creating a Reservation and Transaction Objects before saving
                var Reservation = new ReservationPoco()
                {
                    Id = Guid.NewGuid()
                    ,
                    CarPlateNumber = carPlateNumber
                    ,
                    FullName = fullName
                    ,
                    Email = email
                    ,
                    Phone = phone
                    ,
                    Date = DateTime.Now
                    ,
                    DatesReserved = datesReserved
                    ,
                    NumberOfDaysReserved = numberOfDaysReserved
                };

                var Transaction = new TransactionPoco()
                {
                    Id = Guid.NewGuid()
                    ,
                    AmountPaid = amountPaid
                    ,
                    ReservationId = Reservation.Id
                    ,
                    Details = charge.StripeResponse.Content
                    ,
                    MerchantId = charge.Id
                    ,
                    Reservation = Reservation
                };
                #endregion

                // Save/Update all Information into Database
                //Parallel.Invoke( () => { saveToDatabase(Transaction, Reservation, splitDates(dateReserved)); });

                //Generate Receipt View Model
                var receipt = new ReceiptVM (fullName,
                    Reservation.Id,
                    Reservation.DatesReserved,
                    Reservation.CarPlateNumber,
                    Transaction.AmountPaid,
                    Transaction.MerchantId
                );

                //Generate Receipt HTML string
                var ReceiptHtmlString =  getStringAsync("~/Views/_TransactionSuccessful.cshtml", receipt);

                //Convert HTML string to PDF Page
                var pdfFile = WkhtmltopdfDriver.ConvertHtml(_env.WebRootPath + @"\Rotativa", "-q", await ReceiptHtmlString);
                ViewBag.File = File( pdfFile,"application/pdf","Booking.pdf");

                
                Parallel.Invoke(
                    // Save/Update all Information into Database
                    () => saveToDatabase(Transaction, Reservation, splitDates(datesReserved)),

                    //Send Email to client attaching the PDF as receipt
                    () => {
                                            MailSender.SendMail(new EMail()
                                            {
                                                ToAddress = email,
                                                FromAddress = "Parking Booking Portal",
                                                BodyHtml = ReceiptHtmlString,
                                                MessageBody = "",
                                                Subject = "Booking Confirmation/Receipt",
                                            }, 
                                            new MemoryStream(pdfFile));
                                        }
                );

                Task.WaitAll();
                //Display Success page
                return RedirectToAction(nameof(TransactionSuccessful), receipt);

            }
            // If Payment wasn't successful
            return View("Payment was unssucessful, Please try again with another card or contact your Bank ");
           
        }

        private async Task<string> getStringAsync(string ViewPath, ReceiptVM model)
        {
            //string result = await this.RenderViewToStringAsync(ViewPath, model);
            
            return await RenderViewToStringAsync(this,ViewPath, model);
        }

        private async Task<string> RenderViewToStringAsync<TModel>(Controller controller, string viewNamePath, TModel model)
        {
            if (string.IsNullOrEmpty(viewNamePath))
                viewNamePath = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            using (StringWriter writer = new StringWriter())
            {
                try
                {
                    IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                    ViewEngineResult viewResult = null;

                    if (viewNamePath.EndsWith(".cshtml"))
                        viewResult = viewEngine.GetView(viewNamePath, viewNamePath, false);
                    else
                        viewResult = viewEngine.FindView(controller.ControllerContext, viewNamePath, false);

                    if (!viewResult.Success)
                        return $"A view with the name '{viewNamePath}' could not be found";

                    ViewContext viewContext = new ViewContext(
                        controller.ControllerContext,
                        viewResult.View,
                        controller.ViewData,
                        controller.TempData,
                        writer,
                        new HtmlHelperOptions()
                    );

                    await viewResult.View.RenderAsync(viewContext);

                    return writer.GetStringBuilder().ToString();
                }
                catch (Exception exc)
                {
                    return $"Failed - {exc.Message}";
                }
            }
        }

        public IActionResult TransactionSuccessful(object [] receipt)
        {
            return View(receipt.ToList());
        }
    

        //This method splits/seperates the dates and returns an array of dates
        private DateTime[] splitDates(string dateReserved)
        {
            string[] singleDate = dateReserved.Split(',');

            DateTime[] dates = new DateTime[singleDate.Length];

            for (int i=0; i<singleDate.Length; i++)
            {
                dates[i] = Convert.ToDateTime(singleDate[i]);
            }
            return dates.ToArray();
        }

        private void saveToDatabase(TransactionPoco transaction, ReservationPoco reservation, DateTime[] Dates)
        {
            #region Begin Saving / Updating the Calendar Table
            for (int i = 0; i < Dates.Length; i++)
            {
                var logic = new CalendarLogic(new DataRepository<CalendarPoco>());
                var dateRecord = logic.Get(d => d.Date == Dates[i]); //Tries to search if Date already exists in DB
                if (dateRecord == null)
                {
                     dateRecord = new CalendarPoco() { Id = Guid.NewGuid()
                        , AvailableSlots = _config.GetValue<int>("Calendar:Slots") - 1
                        , ReservedSlots = 1
                        , TotalSlots = _config.GetValue<int>("Calendar:Slots")
                        , Date = Dates[i] };
                    logic.Add(dateRecord);
                }
                else
                {
                    dateRecord.AvailableSlots = dateRecord.AvailableSlots - 1;
                    dateRecord.ReservedSlots = dateRecord.ReservedSlots + 1;
                    logic.Update(dateRecord);
                }
            }
            #endregion //End Saving / Updating the Calendar Table


            #region Begin Saving to the Reservations Table
            {
                var logic = new ReservationLogic(new DataRepository<ReservationPoco>());
                logic.Add(reservation);
            }
            #endregion //End Saving to the Reservations Table

            #region Begin Saving to the Transactions Table
            {
                var logic = new TransactionLogic(new DataRepository<TransactionPoco>());
                logic.Add(transaction);
            }
            #endregion //End Saving to the Transactions Table
           
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
