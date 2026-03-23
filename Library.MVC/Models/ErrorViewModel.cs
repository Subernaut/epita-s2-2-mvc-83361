using System;

namespace Library.MVC.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public bool IsDevelopment { get; set; } = false;

        public string? FriendlyMessage { get; set; }

        public string? ExceptionDetails { get; set; }
    }
}