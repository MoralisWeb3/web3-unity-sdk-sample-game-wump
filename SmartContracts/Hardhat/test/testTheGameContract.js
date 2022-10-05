///////////////////////////////////////////////////////////
// REQUIRES
///////////////////////////////////////////////////////////
const {
    time,
    loadFixture,
} = require("@nomicfoundation/hardhat-network-helpers");
const { anyValue } = require("@nomicfoundation/hardhat-chai-matchers/withArgs");
const { expect } = require("chai");
const { string } = require("hardhat/internal/core/params/argumentTypes");

///////////////////////////////////////////////////////////
// TEST
///////////////////////////////////////////////////////////
describe("The Game Contract", function ()
{
    async function deployTokenFixture() 
    {

        const [owner, addr1, addr2] = await ethers.getSigners();

        // TheGameLibrary
        const TheGameLibrary = await ethers.getContractFactory("TheGameLibrary");
        const theGameLibrary = await TheGameLibrary.deploy();
        await theGameLibrary.deployed();

        // Gold Contract
        const Gold = await ethers.getContractFactory("Gold");
        const gold = await Gold.deploy();
        
        // Prize Contract
        const Prize = await ethers.getContractFactory("Prize");
        const prize = await Prize.deploy();

        // TheGameContract
        const TheGameContract = await ethers.getContractFactory("TheGameContract", {
            libraries: {
                TheGameLibrary: theGameLibrary.address,
            },
          });
        const theGameContract = await TheGameContract.deploy(gold.address, prize.address);

        return { theGameContract, addr1, addr2, theGameLibrary };
    
    }

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Deploys with no exceptions", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);

        // Act

        // Expect
        expect(true).to.equal(true);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 100 when deployed, register", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        const goldBalance = await theGameContract.connect(addr1).getGold(addr1.address);

        // Expect
        expect(goldBalance).to.equal(100);
    })

    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 10 when register, setGold 10", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        await theGameContract.connect(addr1).setGold(10);
        const goldBalance = await theGameContract.connect(addr1).getGold(addr1.address);

        // Expect
        expect(goldBalance).to.equal(10);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 105 when register, setGoldBy 10, setGoldBy -5", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        await theGameContract.connect(addr1).setGoldBy(10);
        await theGameContract.connect(addr1).setGoldBy(-5);
        const goldBalance = await theGameContract.connect(addr1).getGold(addr1.address);

        // Expect
        expect(goldBalance).to.equal(105);
        
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Returns getIsRegistered as false when deployed", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);

        // Act
        const isRegistered = await theGameContract.connect(addr1).getIsRegistered(addr1.address);

        // Expect
        expect(isRegistered).to.equal(false);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Returns getIsRegistered as true when register", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act  
        const isRegistered = await theGameContract.connect(addr1).getIsRegistered(addr1.address);

        // Expect
        expect(isRegistered).to.equal(true);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 100 when register", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        const goldBalance = await theGameContract.connect(addr1).getGold(addr1.address);

        // Expect
        expect(goldBalance).to.equal(100);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 75 when register, transferGold *OUT*", async function ()
    {
        // Arrange
        const { theGameContract, addr1, addr2 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        await theGameContract.connect(addr2).register();

        // Act
        await theGameContract.connect(addr1).transferGold(addr2.address);
        const goldBalance = await theGameContract.connect(addr1).getGold(addr1.address);

        // Expect
        expect(goldBalance).to.equal(75);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getGold to 125 when register, transferGold *IN*", async function ()
    {
        // Arrange
        const { theGameContract, addr1, addr2 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        await theGameContract.connect(addr2).register();

        // Act
        await theGameContract.connect(addr1).transferGold(addr2.address);
        const goldBalance = await theGameContract.connect(addr2).getGold(addr2.address);

        // Expect
        expect(goldBalance).to.equal(125);
    }),

        ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets getIsOwnerOfPrize to addr2 when register, transferPrize *TO* addr2", async function ()
    {
        // Arrange
        const { theGameContract, addr1, addr2 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        await theGameContract.connect(addr2).register();
        const tokenId = 0; //Since addr1 is registered first, addr1 owns tokenId of 0

        // Expect 1
        const result1 = await theGameContract.connect(addr1).getIsOwnerOfPrize(tokenId);
        expect(result1).to.equal(true);

        // Act
        await theGameContract.connect(addr1).transferPrize(addr2.address, tokenId);

        // Expect 2
        const result2 = await theGameContract.connect(addr2).getIsOwnerOfPrize(tokenId);
        expect(result2).to.equal(true);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Returns getIsRegistered as false when register, unregister", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        await theGameContract.connect(addr1).unregister([]);

        // Act
        const isRegistered = await theGameContract.connect(addr1).getIsRegistered(addr1.address);

        // Expect
        expect(isRegistered).to.equal(false);
    }),
    

    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets transferLog to NULL when register, getTransferLogHistory", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();

        // Act
        var transferLog = await theGameContract.connect(addr1).getTransferLogHistory (addr1.address);

        // Expect
        expect(transferLog.length).to.equal(0);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets transferLog to NOT NULL when register, transfer, getTransferLogHistory", async function ()
    {
        // Arrange
        const { theGameContract, addr1, addr2 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        await theGameContract.connect(addr2).register();
        await theGameContract.connect(addr1).transferGold(addr2.address);

        // Act
        var transferLog = await theGameContract.connect(addr1).getTransferLogHistory (addr1.address);

        // Expect
        var expected = "FromAddress="+addr1.address+"|ToAddress="+addr2.address+"|Type=1|Amount=25";
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls safeReregisterAndDeleteAllPrizes() without error when deployed", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        var tokenIds = [];

        // Act
        const result = await theGameContract.connect(addr1).safeReregisterAndDeleteAllPrizes(tokenIds);
        
        // Expect
        expect(true).to.equal(true);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls safeReregisterAndDeleteAllPrizes() without error when register()", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        var tokenIds = [];

        // Act
        const result = await theGameContract.connect(addr1).safeReregisterAndDeleteAllPrizes(tokenIds);
        
        // Expect
        expect(true).to.equal(true);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls safeReregisterAndDeleteAllPrizes() without error when register()", async function ()
    {
        // Arrange
        const { theGameContract, addr1 } = await loadFixture(deployTokenFixture);
        await theGameContract.connect(addr1).register();
        var tokenIds = [];

        // Act
        const result = await theGameContract.connect(addr1).safeReregisterAndDeleteAllPrizes(tokenIds);
        
        // Expect
        expect(true).to.equal(true);
    })

});

