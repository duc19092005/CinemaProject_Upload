﻿namespace backend.ModelDTO.Auth.AuthRespond
{
    public class loginRespondDTO
    {
        public string tokenID {  get; set; } = string.Empty;

        public string message { get; set; } = string.Empty;

        public string userID { get; set; } = string.Empty;

        public string expDate { get; set; } = string.Empty;
        
        public string RoleName { get; set; } = string.Empty;
    }
}
