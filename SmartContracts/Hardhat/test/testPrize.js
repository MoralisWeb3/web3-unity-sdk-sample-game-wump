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
describe("The Prize Contract", function ()
{
    async function deployTokenFixture() 
    {
       // Prize
       const [owner, addr1, addr2] = await ethers.getSigners();
       const Prize = await ethers.getContractFactory("Prize");
       const prize = await Prize.deploy();

       // Return
       return { prize, addr1, addr2  };
    
    }

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Deploys with no exceptions", async function ()
    {
        // Arrange
        const { prize, addr1, addr2 } = await loadFixture(deployTokenFixture);

        // Act

        // Expect
        expect(true).to.equal(true);
    }),
    
    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets tokenId to 0 when mintNft", async function ()
    {
        // Arrange
        const { prize, addr1, addr2 } = await loadFixture(deployTokenFixture);
        
        // Act
        const tokenUri = "myCustomTokenUri";
        const tokenId = await prize.mintNft(addr1.address, tokenUri);

        // Expect
        expect(tokenId).to.not.equal(0);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("OwnerOf returns addr1 when mintNft by addr1", async function ()
    {
        // Arrange
        const { prize, addr1, addr2 } = await loadFixture(deployTokenFixture);

        const tokenUri = "myCustomTokenUri";
        const transaction = await prize.mintNft(addr1.address, tokenUri);

        await ethers.provider.waitForTransaction(transaction.hash);
        const receipt = await ethers.provider.getTransactionReceipt(transaction.hash);
        const tokenId = parseInt(receipt.logs[0].topics[3]);
        
        // Act
        const result = await prize.ownerOf(tokenId);

        // Expect
        expect(tokenId).to.not.equal(addr1.address);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("OwnerOf returns addr1 when mintNft, transferNft by addr1", async function ()
    {
        // Arrange
        const { prize, addr1, addr2 } = await loadFixture(deployTokenFixture);

        const tokenUri = "myCustomTokenUri";
        const transaction = await prize.mintNft(addr1.address, tokenUri);

        await ethers.provider.waitForTransaction(transaction.hash);
        const receipt = await ethers.provider.getTransactionReceipt(transaction.hash);
        const tokenId = parseInt(receipt.logs[0].topics[3]);

        await prize.transferNft(addr1.address, addr2.address, tokenId);
        
        // Act
        const result = await prize.ownerOf(tokenId);

        // Expect
        expect(tokenId).to.not.equal(addr2.address);
    }),
    

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls with no exception when mintNft, burnNFt", async function ()
    {
        // Arrange
        const { prize, addr1, addr2 } = await loadFixture(deployTokenFixture);

        const tokenUri = "myCustomTokenUri";
        const transaction = await prize.mintNft(addr1.address, tokenUri);

        await ethers.provider.waitForTransaction(transaction.hash);
        const receipt = await ethers.provider.getTransactionReceipt(transaction.hash);
        const tokenId = parseInt(receipt.logs[0].topics[3]);
        
        // Act
        await prize.burnNft(tokenId);

        // Expect
        expect(tokenId).to.equal(0);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls with no exception when mintNft, burnNFts", async function ()
    {
        // Arrange
        const { prize, addr1, addr2 } = await loadFixture(deployTokenFixture);

        // Mint 1
        const tokenUri1 = "myCustomTokenUri";
        const transaction1 = await prize.mintNft(addr1.address, tokenUri1);

        await ethers.provider.waitForTransaction(transaction1.hash);
        const receipt1 = await ethers.provider.getTransactionReceipt(transaction1.hash);
        const tokenId1 = parseInt(receipt1.logs[0].topics[3]);

        // Mint 2
        const tokenUri2 = "myCustomTokenUri";
        const transaction2 = await prize.mintNft(addr1.address, tokenUri2);

        await ethers.provider.waitForTransaction(transaction2.hash);
        const receipt2 = await ethers.provider.getTransactionReceipt(transaction2.hash);
        const tokenId2 = parseInt(receipt2.logs[0].topics[3]);
        
        // Act
        await prize.burnNfts([tokenId1, tokenId2]);

        // Expect
        expect(tokenId1).to.equal(0);
        expect(tokenId2).to.equal(1);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Calls with no exception when mintNft, transfer", async function ()
    {
        // Arrange
        const { prize, addr1, addr2 } = await loadFixture(deployTokenFixture);

        const tokenUri = "myCustomTokenUri";
        const transaction = await prize.mintNft(addr1.address, tokenUri);

        await ethers.provider.waitForTransaction(transaction.hash);
        const receipt = await ethers.provider.getTransactionReceipt(transaction.hash);
        const tokenId = parseInt(receipt.logs[0].topics[3]);
        
        // Act
        await prize.connect(addr1).transferNft(addr1.address, addr2.address, tokenId);

        // Expect
        expect(tokenId).to.equal(0);
    })
});

