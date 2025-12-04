using Tars.Core;

Console.WriteLine("Starting TARS...");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();


// 1. Run a job immediately
TarsEngine.Run(() => Console.WriteLine("Job 1: Running immediately!"));


// 2. Schedule a delayed job using After()
TarsEngine.Schedule(() => Console.WriteLine("Job 2: Executed after 5 seconds"))
          .After(TimeSpan.FromSeconds(5));


// 3. Schedule a job to run at a specific timestamp using At()
var dateOfExecution = DateTime.UtcNow.AddSeconds(10);
TarsEngine.Schedule(() => Console.WriteLine($"Job 3: Executed at {dateOfExecution}"))
          .At(dateOfExecution);


// 4. Schedule a job with retry logic
var attempt = 0;
TarsEngine.Schedule(() =>
    {
        attempt++;
        Console.WriteLine($"Executing... attempt {attempt}");
        if (attempt < 3) throw new Exception("Not yet!");
        Console.WriteLine("Success!");
    })
    .WithRetry(5)
    .After(TimeSpan.FromSeconds(1));
