using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Type { get; private set; } = string.Empty;
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ReadAt { get; private set; }

        private Notification() { }

        public static Notification Create(
        Guid userId, string type, string title, string message)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
