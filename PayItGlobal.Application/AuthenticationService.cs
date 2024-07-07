//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using PayEz.Application.Interfaces;
//using PayEz.Domain.Entities;
//using PayEz.Domain.Interfaces;

//namespace PayEz.Application.Services
//{
//    public class AuthenticationService : IAuthenticationService
//    {
//        private readonly IUserRepository _userRepository;

//        public AuthenticationService(IUserRepository userRepository)
//        {
//            _userRepository = userRepository;
//        }

//        public async Task<User> AuthenticateUserAsync(string username, string password)
//        {
//            // Delegate the call to the UserRepository's AuthenticateUser method
//            var user = await _userRepository.AuthenticateUser(username, password);
//            return user;
//        }
//    }
//}
