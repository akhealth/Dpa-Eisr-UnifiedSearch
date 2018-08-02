namespace SearchApi.Utilities
{
    /// <summary>
    /// A creator class for Json responses from the API endpoints.
    /// </summary>
    /// <remarks>
    /// All returned json object should have the following form:
    /// { success = [bool], data = [results or error data] }
    /// </remarks>
    public class JsonResponse
    {
        public static object error(object errorData)
        {
            return new
            {
                success = false,
                data = errorData
            };
        }

        public static object ok()
        {
            return new
            {
                success = true,
                data = (object)null
            };
        }

        public static object ok(object data)
        {
            return new
            {
                success = true,
                data = data
            };
        }
    }
}