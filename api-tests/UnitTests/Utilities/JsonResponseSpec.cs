using SearchApi.Utilities;
using Xunit;

namespace SearchApi.Tests.Utilities
{
    public class JsonResponseSpec
    {
        [Fact]
        public void JsonResponse_error_method_sets_success_to_false()
        {
            // Arrange / Act
            var result = JsonResponse.error("TestError");

            // Assert
            Assert.False(GetRequestObjectSuccess<bool>(result));
        }

        [Fact]
        public void JsonResponse_ok_method_sets_success_to_true()
        {
            // Arrange / Act
            var result = JsonResponse.ok();

            // Assert
            Assert.True(GetRequestObjectSuccess<bool>(result));
        }

        [Fact]
        public void JsonResponse_ok_with_param_sets_success_to_true()
        {
            // Arrange / Act
            var result = JsonResponse.ok("Test Data");

            // Assert
            Assert.True(GetRequestObjectSuccess<bool>(result));
        }

        public T GetRequestObjectSuccess<T>(object o)
        {
            var value = GetPropertyValue(o, "success");
            return (T)value;
        }

        public object GetPropertyValue(object o, string propertyName)
        {
            return GetPropertyValue<object>(o, propertyName);
        }

        public T GetPropertyValue<T>(object o, string propertyName)
        {
            return (T)o.GetType()
                .GetProperty(propertyName)
                .GetValue(o, null);
        }
    }
}