﻿using EventNotifier.Models;

namespace EventNotifier.DTOs
{
    public class CreateEventDTO
    {
        public string Name { get; set; } = null!;
        public DateTime Date { get; set; }

        public string Description { get; set; } = null!;



    }
}