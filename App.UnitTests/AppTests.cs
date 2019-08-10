using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace App.UnitTests
{
    //TODO: Intergration Testing for CustomerDataAccess & ICompanyRepository
    //TODO: Further mocking of other components CustomerDataAccess & ICompanyRepository
    //TODO: Further testing of the entry validation of CustomerService.AddCustomer()
    //TODO: refactor some of the tests to achieve a true 1 test per test

    [TestClass]
    public class AppTests
    {
        private Mock<ICompanyRepository> _companyRepoMock;
        private Mock<ICustomerCreditService> _creditClientMock;
        private Mock<ICustomerDataAccess> _customerDataAccessMock;
        private Company _company;
        private DateTime _validBirthday = new DateTime(1950, 12, 12);
        private DateTime _invalidBirthday = DateTime.Now;

        [TestInitialize]
        public void Initialize()
        {
            _companyRepoMock = new Mock<ICompanyRepository>();
            _creditClientMock = new Mock<ICustomerCreditService>();
            _customerDataAccessMock = new Mock<ICustomerDataAccess>();
            _company = new Company { Id = 1, Name = "ImportantClient", Classification = Classification.Gold };
        }
        [TestMethod]
        public void Can_Add_New_Valid_Customer_SuccesfullyDataAccess_Is_Called()
        {
            //arrange
            _companyRepoMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(_company);
            _creditClientMock.Setup(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .Returns(1000);
            ICustomerService cs = new CustomerService(_companyRepoMock.Object,
                _creditClientMock.Object, _customerDataAccessMock.Object);

            //act
            var result = cs.AddCustomer("Test", "T2", "testt@hotmail.com", _validBirthday, 1);

            //assert
            _customerDataAccessMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()));
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void Can_Not_Add_UnderAge_Customer_And_DataAccess_Is_Not_Called()
        {
            //arrange
            _companyRepoMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(_company);
            _creditClientMock.Setup(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .Returns(1000);
            ICustomerService cs = new CustomerService(_companyRepoMock.Object,
                _creditClientMock.Object, _customerDataAccessMock.Object);

            //act
            var result = cs.AddCustomer("Test", "T2", "testt@hotmail.com", DateTime.Now, 1);

            //assert
            _customerDataAccessMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never());
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void Can_Not_Add_Not_Qualified_Customer_And_DataAccess_Is_Not_Called()
        {
            //arrange
            _companyRepoMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(_company);
            _creditClientMock.Setup(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>()))
                .Returns(1); // > 500 credit limit
            ICustomerService cs = new CustomerService(_companyRepoMock.Object,
                _creditClientMock.Object, _customerDataAccessMock.Object);

            //act
            var result = cs.AddCustomer("Test", "T2", "testt@hotmail.com", _invalidBirthday, 1);

            //assert
            _customerDataAccessMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never());
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void Get_Credit_Limit_Is_Called_If_Entry_Model_Is_Valid()
        {
            //arrange
            _companyRepoMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(_company);
            ICustomerService cs = new CustomerService(_companyRepoMock.Object,
                _creditClientMock.Object, _customerDataAccessMock.Object);

            //act
            var result = cs.AddCustomer("Test", "T2", "testt@hotmail.com", _validBirthday, 1);

            //assert
            _creditClientMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<DateTime>()));
        }

        [TestMethod]
        public void Get_Credit_Limit_Is_Not_Called_If_Entry_Model_Is_Valid()
        {
            //arrange
            _companyRepoMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(_company);
            ICustomerService cs = new CustomerService(_companyRepoMock.Object,
                _creditClientMock.Object, _customerDataAccessMock.Object);

            //act
            var result = cs.AddCustomer("Test", "T2", "testt@hotmail.com", _invalidBirthday, 1);

            //assert
            _creditClientMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>()), Times.Never());
        }

        #region OldTests
        //Characteriztion test impossible due to no DB connection, all tests would fail
        //
        //[TestMethod]
        //public void Can_Add_Bad_Customer_Data_Returns_False()
        //{
        //    //arrange
        //    var cs = new CustomerService();
        //    //act
        //    bool result = cs.AddCustomer(
        //        firname: "Dan",
        //        surname: "Tan",
        //        email: "dantan@hotmail.com",
        //        dateOfBirth: DateTime.Now,
        //        companyId: 1);
        //    //assert
        //    Assert.IsTrue(result);
        //}
        //[TestMethod]
        //public void Can_Add_Customer_Returns_True()
        //{
        //    //arrange
        //    var cs = new CustomerService();
        //    //act
        //    bool result = cs.AddCustomer(
        //        firname: "Dan",
        //        surname: "Tan",
        //        email: "dantan@hotmail.com",
        //        dateOfBirth: new DateTime(1990,12,12),
        //        companyId: 1);
        //    //assert
        //    Assert.IsTrue(result);
        //}

        [TestClass]
        public class DataAccessTests
        {
            //[TestMethod]
            //public void a()
            //{
            //    //arrange
            //    //act
            //    CustomerDataAccess.AddCustomer(new Customer());
            //    //assert
            //    Assert.IsTrue(true);
            //}
        }


        #endregion
    }
}
