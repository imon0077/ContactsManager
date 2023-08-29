﻿using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as a DTO for inserting a new person
    /// </summary>
    public class PersonAddRequest
    {

        [Required(ErrorMessage = "Person name can't be empty.")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be empty.")]
        [EmailAddress(ErrorMessage = "Email should be valid format.")]
        public string? Email { get; set; }

        public GenderOptions? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converts the current object of PersonAddRequest into a new Person type
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonName = PersonName,
                Email = Email,
                Gender = Gender.ToString(),
                DateOfBirth = DateOfBirth,
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
