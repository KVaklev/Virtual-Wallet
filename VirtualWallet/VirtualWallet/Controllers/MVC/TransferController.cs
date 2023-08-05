using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    [Route ("transfer")]
    public class TransferController : Controller
    {
        private readonly ITransferService transferService;
        private readonly IUserRepository userRepository;

        public TransferController(ITransferService transferService, IUserRepository userRepository)
        {
            this.transferService = transferService;
            this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // logic to display a list of transfers 

            var loggedUser = await FindLoggedUserAsync();
            var model = transferService.GetAll(loggedUser).Select(m => new GetTransferDto { Username = m.Account.User.Username, DateCreated = m.DateCreated }).ToList();
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> Details(int id)
        //{
        //    //  logic to retrieve transfer details by ID
        //    // Call the service method to get transfer details and pass it to the view
        //    return View();
        //}
        //// GET: /Transfer/Create
        //public IActionResult Create()
        //{
        //    //  logic to prepare data for the Create view (e.g.,  accounts, cards, etc.)
        //    return View();
        //}

        //// POST: /Transfer/Create
        //[HttpPost]
        //public async Task<IActionResult> Create(CreateTransferDto transferDto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the currently logged-in user
        //        var loggedUser = await GetLoggedInUserAsync();

        //        // Call the service to create the transfer
        //        var result = await transferService.CreateAsync(transferDto, loggedUser);

        //        if (result.IsSuccessful)
        //        {
        //            // Successful transfer creation, redirect to a success page or show a success message
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            // Transfer creation failed, show an error message
        //            ModelState.AddModelError("", result.Message);
        //        }
        //    }

        //    // If the model state is invalid or transfer creation failed, redisplay the create form with errors
        //    return View(transferDto);
        //}


        //// GET: /Transfer/Execute/5
        //public async Task<IActionResult> Execute(int id)
        //{
        //    // Your logic to execute a transfer by ID
        //    // Call the service method to execute the transfer
        //    return RedirectToAction("Details", new { id });
        //}

        //// Helper method to get the currently logged-in user
        //private async Task<User> GetLoggedInUserAsync()
        //{
        //    // logic to retrieve the logged-in user from the authentication system
        //    // You may use the user claims or the user ID to fetch the user from your user repository
        //    // Example:
        //    // var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
        //    // var loggedUser = await userRepository.GetByUsernameAsync(loggedUsersUsername);
        //    // return loggedUser;
        //    return null; // Replace with your actual implementation
        //}

        private async Task<User> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await this.userRepository.GetByUsernameAsync(loggedUsersUsername);
            return loggedUser; // to move to user Service
        }
    }
}
