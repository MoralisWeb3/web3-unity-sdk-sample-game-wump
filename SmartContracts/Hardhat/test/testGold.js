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
describe("The Gold Contract", function ()
{

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Deploys with no exceptions", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const Gold = await ethers.getContractFactory("Gold");

        // Act
        const gold = await Gold.deploy();

        // Expect
        expect(true).to.equal(true);
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets totalSupply to 0 when deployed", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const Gold = await ethers.getContractFactory("Gold");
        const gold = await Gold.deploy();

        // Act
        const totalSupply = await gold.totalSupply();

        // Expect
        expect(totalSupply).to.equal(0);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets totalSupply to 10 when setGoldBy 10", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const Gold = await ethers.getContractFactory("Gold");
        const gold = await Gold.deploy();
        await gold.setGoldBy(addr1.address, 10);

        // Act
        const totalSupply = await gold.totalSupply();

        // Expect
        expect(totalSupply).to.equal(10);
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets goldBalance to 0 when deployed", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const Gold = await ethers.getContractFactory("Gold");
        const gold = await Gold.deploy();

        // Act
        const goldBalance = await gold.getGold(addr1.address);

        // Expect
        expect(goldBalance).to.equal(0);
    })

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets goldBalance to 10 when setGold 10", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const Gold = await ethers.getContractFactory("Gold");
        const gold = await Gold.deploy();

        // Act
        const goldBalanceBefore = await gold.getGold(addr1.address);
        await gold.setGold(addr1.address, goldBalanceBefore + 10);
        const goldBalanceAfter = await gold.getGold(addr1.address);

        // Expect
        expect(goldBalanceAfter).to.equal(10);
    }),


     ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("getGold is 05 when setGold +10 and setGold -05", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const Gold = await ethers.getContractFactory("Gold");
        const gold = await Gold.deploy();

        // Act
        const goldBalanceBefore1 = await gold.getGold(addr1.address);
        await gold.setGold(addr1.address, goldBalanceBefore1 + 10);

        const goldBalanceBefore2 = await gold.getGold(addr1.address);
        await gold.setGold(addr1.address, goldBalanceBefore2 - 5);
        const goldBalanceAfter = await gold.getGold(addr1.address);

        // Expect
        expect(goldBalanceAfter).to.equal(5);
        
    }),

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("getGold is 05 when setGoldBy +10 and setGoldBy -05", async function ()
    {
        // Arrange
        const [owner, addr1] = await ethers.getSigners();
        const Gold = await ethers.getContractFactory("Gold");
        const gold = await Gold.deploy();

        // Act
        await gold.setGoldBy(addr1.address, 10);
        await gold.setGoldBy(addr1.address, -5);
        const goldBalanceAfter = await gold.getGold(addr1.address);

        // Expect
        expect(goldBalanceAfter).to.equal(5);
        
    })

});

