﻿namespace PayItGlobal.Application.Models
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
