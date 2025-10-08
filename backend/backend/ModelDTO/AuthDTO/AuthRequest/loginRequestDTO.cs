﻿using System.ComponentModel.DataAnnotations;

namespace backend.ModelDTO.Auth.AuthRequest
{
    public class loginRequestDTO
    {
        [Required(ErrorMessage = "Không được để trống")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập không được vượt quá 100 ký tự")]
        public string loginUserName { get; set; } = "";


        [Required(ErrorMessage = "Không được để trống")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu ít nhất phải 8 ký tự")]
        public string loginUserPassword { get; set; } = "";
    }
}
