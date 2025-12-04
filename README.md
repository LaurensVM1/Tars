# Tars Job Scheduler

A lightweight, thread-safe job scheduler for .NET with built-in retry logic and async support.

## Features

- ⚡ Simple API for scheduling jobs
- 🔄 Automatic retry with exponential backoff
- 🧵 Thread-safe operations
- ⏰ Priority-based execution
- 🎯 Support for both sync and async jobs

## Installation
```bash
dotnet add package Tars.JobScheduler
```

## Quick Start
```csharp
using Tars.Core;

// Run immediately
TarsEngine.Run(() => Console.WriteLine("Hello!"));

// Run async
TarsEngine.Run(async () => await SendEmailAsync());

// Schedule for later
TarsEngine.Schedule(() => Console.WriteLine("Later!"))
    .At(DateTime.UtcNow.AddHours(1));

// With retry logic
TarsEngine.Schedule(() => CallExternalAPI())
    .At(DateTime.UtcNow.AddMinutes(5))
    .WithRetries(3);
```

## License

MIT