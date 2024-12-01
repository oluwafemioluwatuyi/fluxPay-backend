using System;
using System.Collections.Generic;

namespace FluxPay.Models;

public class User
{
    public class Client
    {
        public Guid Id { get; set; } // Unique identifier for the client in your system
        public DateTime ActivationDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int GroupId { get; set; }
        public int LegalFormId { get; set; }
        public string MobileNumber { get; set; }
        public int OfficeId { get; set; }
        public string Locale { get; set; }
        public string DateFormat { get; set; }

        // Navigation properties
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Datatable> Datatables { get; set; }
    }

    public class Address
    {
        public Guid Id { get; set; } // Unique identifier for the address
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
        public string PostalCode { get; set; }
        public int StateProvinceId { get; set; }
        public string Street { get; set; }
        public int AddressTypeId { get; set; }
    }

    public class Datatable
    {
        public Guid Id { get; set; } // Unique identifier for the datatable
        public string Data { get; set; }
        public string RegisteredTableName { get; set; }
    }
}