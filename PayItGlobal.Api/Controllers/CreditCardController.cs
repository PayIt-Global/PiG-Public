using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PayEzPaymentApi.Helper;
using PayEzPaymentApi.Models;
using System.Security.Claims;
using PayItGlobal.Infrastructure.Interfaces;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.DTOs;
using PayItGlobal.Domain.Models;

namespace PayEzPaymentApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [PayEzPaymentApi.Helper.Authorize] // This ensures all methods in the controller are accessible only by authenticated users
    public class CreditCardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        public CreditCardController(IUserService userService, IPaymentService paymentService, IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._userService = userService;
            this._paymentService = paymentService;
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }

        [HttpPost("SubmitTransaction")]
        public async Task<IActionResult> SubmitTransaction(CreditCardRequest model)
        {

            var userIdValue = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized("User ID not found in claims.");
            }

            var user = await _userService.GetByIdForAPI_Async(userIdValue); // Use the userId (of type Guid) in your subsequent operations
            CreditCardResponse modelResponse = new CreditCardResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    var paymentToCreate = new PaymentDataCCDto
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        CCNumber = model.CCNumber,
                        BusinessName = model.BusinessName,
                        Zip = model.Zipcode,
                        Addr1 = model.Street,
                        City = model.City,
                        StateName = model.StateAbbreviation,
                        ExpMonth = model.ExpirationMonth,
                        ExpYear = model.ExpirationYear,
                        CSC = model.CVV
                    };

                    var response = await _paymentService.EnterCCTransactionAsync(paymentToCreate, model.Amount, user);
                    modelResponse.ConfirmationNumber = response.TransactionID;
                    modelResponse.ResponseCode = response.ResponseCD;
                    modelResponse.ResponseText = response.ResponseDesc;

                    return Ok(modelResponse);

                }
                catch (Exception ex)
                {
                    //AppLogging.GetLogger().Error(GeneralMethods.GetStackNames(), ex);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

            }

        }

    }
}
