using System.Collections.Generic;
using Xunit;
using SearchApi.Models;

namespace SearchApi.Tests.Models
{
    public class MciSearchResponseSpec
    {
        [Fact]
        public void MciSearchResponse_contains_SearchResponsePerson()
        {
            // Arrange
            SearchResponsePerson person = new SearchResponsePerson();
            person.VirtualId = "TestVirtualId";
            person.MatchPercentage = "TestMatchPercentage";
            person.Title = "TestTitle";
            person.FirstName = "TestFirstName";
            person.MiddleName = "TestMiddleName";
            person.LastName = "TestLastName";
            person.Suffix = "TestSuffix";
            person.Gender = "TestGender";
            person.Registrations = new Registrations();
            person.Names = new Names
            {
                Name = new List<Name>
                {
                    new Name
                    {
                        NameType = "mockType",
                        Title = "mockTitle",
                        FirstName = "mockFirstName",
                        MiddleName = "mockMiddleName",
                        LastName = "mockLastName",
                        Suffix = "mockSuffix"
                    }
                }
            };

            var model = new MciSearchResponse();

            // Act
            model.SearchResponsePerson = new List<SearchResponsePerson>();
            model.SearchResponsePerson.Add(person);

            // Assert
            Assert.Single(model.SearchResponsePerson);
            Assert.IsType<SearchResponsePerson>(model.SearchResponsePerson[0]);
            Assert.Equal(person, model.SearchResponsePerson[0]);
        }

        [Fact]
        public void SearchResponsePerson_returns_middle_initial()
        {
            // Arrange
            SearchResponsePerson person = new SearchResponsePerson();
            person.MiddleName = "TestMiddleName";

            // Act
            var middleInitial = person.MiddleInitial;

            // Assert
            Assert.Equal("T", middleInitial);
        }

        [Theory]
        [InlineData("TestFirstName", "TestMiddleName", "TestLastName", "TestFirstName T. TestLastName")]
        [InlineData("TestFirstName", "", "TestLastName", "TestFirstName TestLastName")]
        public void SearchResponsePerson_returns_formatted_name(string firstName, string middleName, string lastName, string expected)
        {
            // Arrange
            SearchResponsePerson person = new SearchResponsePerson();
            person.FirstName = firstName;
            if (!string.IsNullOrEmpty(middleName))
                person.MiddleName = middleName;
            person.LastName = lastName;

            // Act
            var formattedName = person.FormattedName;

            // Assert
            Assert.Equal(expected, formattedName);
        }
    }
}