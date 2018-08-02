namespace SearchApi.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class MciSearchResponse
    {
        public List<SearchResponsePerson> SearchResponsePerson { get; set; }
    }

    public partial class SearchResponsePerson
    {
        public string VirtualId { get; set; }
        public string MatchPercentage { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public System.DateTimeOffset DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Registrations Registrations { get; set; }
        public Names Names { get; set; }

        [JsonIgnore]
        public string MiddleInitial
        {
            get
            {
                return !String.IsNullOrEmpty(MiddleName) ?
                    MiddleName.Substring(0, 1) :
                    null;
            }
        }

        public string FormattedDateOfBirth
        {
            get
            {
                return DateOfBirth.Date.ToShortDateString();
            }
        }

        [JsonIgnore]
        public string FormattedName
        {
            get
            {
                var middleInitial = MiddleInitial;
                if (middleInitial != null)
                {
                    return $"{FirstName} {MiddleInitial}. {LastName}";
                }
                return $"{FirstName} {LastName}";
            }
        }
    }

    public partial class Registrations
    {
        public List<Registration> Registration { get; set; }
    }

    public partial class Names
    {
        public List<Name> Name { get; set; }
    }

    public partial class Name
    {
        public string NameType { get; set; }
        public object Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
    }

    public partial class Registration
    {
        public string RegistrationName { get; set; }
        public string RegistrationValue { get; set; }
    }
}