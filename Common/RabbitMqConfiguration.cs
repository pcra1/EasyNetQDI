﻿using System;

namespace Common
{
    public class RabbitMqConfiguration
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int HostPort { get; set; }
        
        public string HealthCheckConnectionString =>
            "amqp://" + Username + ":"
            + Password + "@"
            + Host + ":"
            + HostPort
            + VirtualHost;
    }
}