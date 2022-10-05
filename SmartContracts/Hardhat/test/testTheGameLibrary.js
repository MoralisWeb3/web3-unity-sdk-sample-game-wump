///////////////////////////////////////////////////////////
// REQUIRES
///////////////////////////////////////////////////////////
const {
    time,
    loadFixture,
} = require("@nomicfoundation/hardhat-network-helpers");
const { anyValue } = require("@nomicfoundation/hardhat-chai-matchers/withArgs");
const { expect } = require("chai");
const EmptyAddress = "0x0000000000000000000000000000000000000000";

///////////////////////////////////////////////////////////
// TEST
///////////////////////////////////////////////////////////
describe("TheGameLibrary", function ()
{
    async function deployTokenFixture() 
    {
        const [owner, addr1, addr2] = await ethers.getSigners();

        // TheGameLibrary
        const TheGameLibrary = await ethers.getContractFactory("TheGameLibrary");
        const theGameLibrary = await TheGameLibrary.deploy();
        await theGameLibrary.deployed();

        return { TheGameLibrary, theGameLibrary, addr1, addr2  };
    
    }


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Deploys with no exceptions", async function ()
    {
        // Arrange
        const { TheGameLibrary, theGameLibrary, addr1 } = await loadFixture(deployTokenFixture);

        // Act

        // Expect
        expect(true).to.equal(true);
    }),
    

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("createNewTransferLogForTesting returns proper TransferLog", async function ()
    {
        // Arrange
        const { theGameLibrary } = await loadFixture(deployTokenFixture);

        // Act
        const transferLog = await theGameLibrary.createNewTransferLogForTesting();
        
        // Expect
       expect(transferLog.FromAddress).to.equal(EmptyAddress);
        expect(transferLog.ToAddress).to.equal(EmptyAddress);
        expect(transferLog.Type).to.equal(1);
        expect(transferLog.Amount).to.equal(1);
    }),

    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("convertTransferLogToString returns proper format", async function ()
    {
        // Arrange
        const { theGameLibrary } = await loadFixture(deployTokenFixture);

        // Act
        var result = await theGameLibrary.convertTransferLogToStringForTesting();
        
        // Expect
        var expectedString = "FromAddress="+EmptyAddress+"|ToAddress="+EmptyAddress+"|Type=1|Amount=1";
        expect(result).to.equal(expectedString);
    })
});



