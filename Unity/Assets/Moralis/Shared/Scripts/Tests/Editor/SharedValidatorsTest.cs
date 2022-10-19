using MoralisUnity.Samples.Shared.Utilities;
using NUnit.Framework;

namespace MoralisUnity.Samples.Shared.Validators
{
    public class SharedValidatorsTest
    {
        //  Properties ------------------------------------
        
        
        //  Fields ----------------------------------------
        private static string[] Web3AddressValuesNotValid = new string[]
        {
            "",
            "0",
            "0x",
            "0xasdfg",
            "0xD034739C2aE807C70Cd703092b946f62a49509D",    //has missing last char
            "0xD034739C2aE807C70Cd703092b946f62a49509D12",  //has additional last char
            
        };
        private static string[] Web3AddressValuesValid = new string[]
        {
            "0xD034739C2aE807C70Cd703092b946f62a49509D1",   //camel
            "0XD034739C2AE807C70CD703092B946F62A49509D1",   //upper
            "0xd034739c2ae807c70cd703092b946f62a49509d1",   //lower
        };
        
        
        //  Unity Methods----------------------------------
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Executes BEFORE ALL test methods of this test class
        }
        
        [SetUp]
        public void Setup()
        {
            // Executes BEFORE EACH test methods of this test class
        }
        
        [TearDown]
        public void TearDown()
        {
            // Executes AFTER EACH test methods of this test class
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Executes AFTER ALL test methods of this test class
        }
        
        
        //  General Methods -------------------------------
        [Test]
        public void IsValidWeb3Address_IsTrue_WhenStringIsValid([ValueSource("Web3AddressValuesValid")] string web3Address)
        {
            // Arrange
            
            // Act
            bool isValidWeb3Address = SharedValidators.IsValidWeb3Address(web3Address);
                
            // Assert
            Assert.That(isValidWeb3Address, Is.True);
        }
        
        [Test]
        public void IsValidWeb3Address_IsFalse_WhenStringIsNotValid([ValueSource("Web3AddressValuesNotValid")] string web3Address)
        {
            // Arrange
            
            // Act
            bool isValidWeb3Address = SharedValidators.IsValidWeb3Address(web3Address);
                
            // Assert
            Assert.That(isValidWeb3Address, Is.False);
        }
        
        //  Event Handlers --------------------------------
    }
}
