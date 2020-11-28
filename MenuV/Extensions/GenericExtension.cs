namespace MenuV
{
    using System;
    using System.Reflection;

    public static class GenericExtension
    {
        public static bool IsNullOrDefault<T>(this T argument)
        {
            if (Equals(argument, default(T)))
            {
                return true;
            }

            var methodType = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(methodType);

            if (underlyingType != null && Equals(argument, Activator.CreateInstance(underlyingType)))
            {
                return true;
            }

            var argumentType = argument.GetType();

            #if NET40 || NET45 || NET46
                if (argumentType.IsValueType && argumentType != methodType)
                {
                    var obj = Activator.CreateInstance(argument.GetType());
                    return obj.Equals(argument);
                }
            #else
                if (argumentType.GetTypeInfo().IsValueType && argumentType != methodType)
                {
                    var obj = Activator.CreateInstance(argument.GetType());
                    return obj.Equals(argument);
                }
            #endif

            return false;
        }

        public static TO Ensure<TI, TO>(this TI input, TO defaultOutput)
        {
            if (defaultOutput == null) { return default; }

            var requiredType = typeof(TO);
            var inputType = typeof(TI);

            if (requiredType == inputType)
            {
                return input.IsNullOrDefault() ? defaultOutput : input is TO v ? v : defaultOutput;
            }

            if (requiredType == typeof(int))
            {
                if (inputType == typeof(bool))
                {
                    var b = (bool)(object)input ? 1 : 0;

                    return b is TO v ? v : defaultOutput;
                }

                var n = Convert.ToInt32(input);

                return n is TO v2 ? v2 : defaultOutput;
            }

            if (requiredType == typeof(string))
            {
                if (inputType == typeof(bool))
                {
                    var s = (bool)(object)input ? "yes" : "no";

                    return s is TO v ? v : defaultOutput;
                }

                var s2 = Convert.ToString(input);

                if (s2 == "nil" || s2 == "null")
                {
                    return defaultOutput;
                }

                return s2 is TO v2 ? v2 : defaultOutput;
            }

            if (requiredType == typeof(bool))
            {
                if (inputType == typeof(string))
                {
                    var s = ((string)(object)input)?.ToLower();

                    switch (s)
                    {
                        case "true":
                        case "1":
                        case "yes":
                        case "y":
                            return true is TO v ? v : defaultOutput;
                        case "false":
                        case "0":
                        case "no":
                        case "n":
                            return true is TO v2 ? v2 : defaultOutput;
                        default:
                            return defaultOutput;
                    }
                }

                if (inputType == typeof(int))
                {
                    var n = (int)(object)input;

                    switch (n)
                    {
                        case 1:
                            return true is TO v ? v : defaultOutput;
                        case 0:
                            return false is TO v2 ? v2 : defaultOutput;
                        default:
                            return defaultOutput;
                    }
                }
            }

            return defaultOutput;
        }
    }
}
