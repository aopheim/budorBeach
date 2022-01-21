using System;

namespace Shared.Models
{
    public class BirdPresenceRegistration
    {
        public int Id { get; set; }
        public DateTime StartedAt { get; set; }
        public int DurationInSeconds { get; set; }
    }
}