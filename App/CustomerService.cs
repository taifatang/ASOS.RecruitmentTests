using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public interface ICustomerService
    {
        bool AddCustomer(string firname, string surname, string email, DateTime dateOfBirth, int companyId);
    }
    public class CustomerService : ICustomerService
    {
        private ICompanyRepository _companyRepo;
        private ICustomerCreditService _creditClient;
        private ICustomerDataAccess _customerDataAccess;

        public CustomerService(ICompanyRepository companyRepo,
            ICustomerCreditService customerCreditServiceClient,
            ICustomerDataAccess customerDataAccess)
        {
            _companyRepo = companyRepo;
            _creditClient = customerCreditServiceClient;
            _customerDataAccess = customerDataAccess;
        }

        //TODO: take a NewCustomer Type instead ? for cleaner implementation
        public bool AddCustomer(string firname, string surname, string email, DateTime dateOfBirth, int companyId)
        {
            if (IsEntryValid(firname, surname, email, dateOfBirth, companyId))
            {
                var newCustomer = NewCustomerFactory(firname, surname, email, dateOfBirth, companyId);

                if (IsCreditLimitQualified(newCustomer))
                {
                    //TODO: Maybe have a return type to indicate Add method is successful allowing:
                    //return _customerDataAccess.AddCustomer(newCustomer);
                    _customerDataAccess.AddCustomer(newCustomer);
                    return true;
                }
            }
            return false;
        }
        public Customer NewCustomerFactory(string firname, string surname, string email, DateTime dateOfBirth, int companyId)
        {
            var company = _companyRepo.GetById(companyId);

            var customer = new Customer
            {
                Company = company,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firname,
                Surname = surname
            };

            switch (company.Name)
            {
                case "VeryImportantClient":
                    // Skip credit check
                    customer.HasCreditLimit = false;
                    break;
                case "ImportantClient":
                    // Do credit check and double credit limit
                    customer.HasCreditLimit = true;
                    customer.CreditLimit = RetriveCreditLimit(customer, bonusCredit: 2);
                    break;
                default:
                    // Do credit check
                    customer.HasCreditLimit = true;
                    customer.CreditLimit = RetriveCreditLimit(customer);
                    break;
            }
            return customer;
        }
        public bool IsEntryValid(string firname, string surname, string email, DateTime dateOfBirth, int companyId)
        {
            if (string.IsNullOrEmpty(firname) || string.IsNullOrEmpty(surname))
            {
                return false;
            }

            if (!email.Contains("@") && !email.Contains("."))
            {
                return false;
            }

            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            if (age < 21)
            {
                return false;
            }

            return true;
        }
        public int RetriveCreditLimit(Customer customer, int bonusCredit = 1)
        {
            var creditLimit = _creditClient.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
            creditLimit = creditLimit * bonusCredit;
            return creditLimit;
        }
        public bool IsCreditLimitQualified(Customer customer)
        {
            if (customer.HasCreditLimit && customer.CreditLimit < 500)
            {
                return false;
            }
            return true;
        }


    }
}
