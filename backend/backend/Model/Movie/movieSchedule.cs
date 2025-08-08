﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using backend.Model.Auth;
using backend.Model.Booking;
using backend.Model.Cinemas;
using backend.Model.CinemaRoom;
using backend.Model.Movie;
using backend.Model.Price;
using backend.Model.Product;
using backend.Model.ScheduleList;
using Microsoft.EntityFrameworkCore;

namespace backend.Model.Movie
{
    public class movieSchedule
    {
        [Key]
        [Column(TypeName = "varchar(100)")]
        public string movieScheduleId { get; set; } = "";

        [ForeignKey("cinemaRoom")]
        [Column(TypeName = "varchar(100)")]
        [Required]

        public string cinemaRoomId { get; set; } = "";

        [ForeignKey("movieInformation")]
        [Column(TypeName = "varchar(100)")]
        [Required]
        public string movieId { get; set; } = "";

        [ForeignKey("movieVisualFormat")]
        [Column(TypeName = "varchar(100)")]
        public string movieVisualFormatID { get; set; } = "";

        // Khóa ngoại tham chiếu tới thứ
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string DayInWeekendSchedule { get; set; } = string.Empty;

        // Khóa ngoại tham chiếu tới giờ chiếu
        [ForeignKey("HourSchedule")]
        [Required]
        public string HourScheduleID { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduleDate { get; set; }

        // Trạng thái của lịch chiếu
        [Required]
        public bool IsDelete { get; set; }

        public cinemaRoom cinemaRoom { get; set; } = null!;

        public movieInformation movieInformation { get; set; } = null!;

        public List<orderDetailTicket> orderDetailTicket { get; set; } = [];

        public HourSchedule HourSchedule { get; set; } = null!;

        public movieVisualFormat movieVisualFormat { get; set; } = null!;
    }
}
