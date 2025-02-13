using System;

public class CommonChecks
{
    public static void CheckLogin(string sessionUser)
    {
        if (string.IsNullOrEmpty(sessionUser))
        {
            throw new UnauthorizedAccessException("401 (Unauthorized)");
        }
    }

    public static void CheckActor(string userRole, string requiredRole)
    {
        if (userRole != requiredRole)
        {
            throw new UnauthorizedAccessException("403 (Forbidden)");
        }
    }
}

public class InputChecks
{
    public static void CheckRequired(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("400 (Bad Request)");
        }
    }

    public static void CheckMinLength(string input, int minLength)
    {
        if (input.Length < minLength)
        {
            throw new ArgumentException("400 (Bad Request)");
        }
    }

    public static void CheckMaxLength(string input, int maxLength)
    {
        if (input.Length > maxLength)
        {
            throw new ArgumentException("400 (Bad Request)");
        }
    }

    // 他のチェックメソッドも同様に実装
}
