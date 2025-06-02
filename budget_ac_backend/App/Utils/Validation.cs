namespace budget_ac_backend.App.Utils;

public static class Validation {
    public static T ThrowIfArgumentNull<T>(this T? argument) {
        if (argument == null) {
            throw new ArgumentNullException($"{typeof(T).Name}");
        }

        return argument;
    }

    public static string ThrowIfNullOrWhiteSpace(this string? argument, string parameterName) {
        if (string.IsNullOrWhiteSpace(argument)) {
            throw new ArgumentNullException(parameterName, $"{parameterName} cannot be null, empty, or whitespace.");
        }

        return argument;
    }
}