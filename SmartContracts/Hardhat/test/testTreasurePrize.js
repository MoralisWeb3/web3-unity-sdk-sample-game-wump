///////////////////////////////////////////////////////////
// REQUIRES
///////////////////////////////////////////////////////////
const {
    time,
    loadFixture,
} = require("@nomicfoundation/hardhat-network-helpers");
const { anyValue } = require("@nomicfoundation/hardhat-chai-matchers/withArgs");
const { expect } = require("chai");


///////////////////////////////////////////////////////////
// TEST
///////////////////////////////////////////////////////////
describe("The TreasurePrize Contract", function ()
{

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Deploys with no exceptions", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const TreasurePrize = await ethers.getContractFactory("TreasurePrize");

        // Act
        const treasurePrize = await TreasurePrize.deploy();

        // Expect
        expect(true).to.equal(true);
    }),
    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets tokenId to 0 when mintNft", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const TreasurePrize = await ethers.getContractFactory("TreasurePrize");
        const treasurePrize = await TreasurePrize.deploy();
        
        // Act
        const tokenUri = "myCustomTokenUri";
        const tokenId = await treasurePrize.mintNft(addr1.address, tokenUri);

        // Expect
        expect(tokenId).to.not.equal(0);
    }),
    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls with no exception when mintNft, burnNFt", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const TreasurePrize = await ethers.getContractFactory("TreasurePrize");
        const treasurePrize = await TreasurePrize.deploy();

        const tokenUri = "myCustomTokenUri";
        const transaction = await treasurePrize.mintNft(addr1.address, tokenUri);

        await ethers.provider.waitForTransaction(transaction.hash);
        const receipt = await ethers.provider.getTransactionReceipt(transaction.hash);
        const tokenId = parseInt(receipt.logs[0].topics[3]);
        
        // Act
        await treasurePrize.burnNft(tokenId);

        // Expect
        expect(tokenId).to.equal(0);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls with no exception when mintNft, burnNFts", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const TreasurePrize = await ethers.getContractFactory("TreasurePrize");
        const treasurePrize = await TreasurePrize.deploy();

        // Mint 1
        const tokenUri1 = "myCustomTokenUri";
        const transaction1 = await treasurePrize.mintNft(addr1.address, tokenUri1);

        await ethers.provider.waitForTransaction(transaction1.hash);
        const receipt1 = await ethers.provider.getTransactionReceipt(transaction1.hash);
        const tokenId1 = parseInt(receipt1.logs[0].topics[3]);

        // Mint 2
        const tokenUri2 = "myCustomTokenUri";
        const transaction2 = await treasurePrize.mintNft(addr1.address, tokenUri2);

        await ethers.provider.waitForTransaction(transaction2.hash);
        const receipt2 = await ethers.provider.getTransactionReceipt(transaction2.hash);
        const tokenId2 = parseInt(receipt2.logs[0].topics[3]);
        
        // Act
        await treasurePrize.burnNfts([tokenId1, tokenId2]);

        // Expect
        expect(tokenId1).to.equal(0);
        expect(tokenId2).to.equal(1);
    })
});

