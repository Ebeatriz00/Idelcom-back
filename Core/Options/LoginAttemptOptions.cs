namespace Core.Options
{
    public class LockoutStep
    {
        public int AfterAttempts { get; set; }
        public int LockoutSeconds { get; set; }
    }

    public class LoginAttemptOptions
    {
        public int WindowMinutes { get; set; } = 60;
        public List<LockoutStep> Steps { get; set; } =
        [
            new() { AfterAttempts = 6,  LockoutSeconds = 20    },
            new() { AfterAttempts = 8,  LockoutSeconds = 60    },
            new() { AfterAttempts = 10, LockoutSeconds = 300   },
            new() { AfterAttempts = 13, LockoutSeconds = 1800  },
        ];
    }
}
