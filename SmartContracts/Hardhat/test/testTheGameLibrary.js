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
describe("TheGameLibrary", function ()
{
    async function deployTokenFixture() 
    {
        const [owner, addr1, addr2] = await ethers.getSigners();

        // TheGameLibrary
        const TheGameLibrary = await ethers.getContractFactory("TheGameLibrary");
        const theGameLibrary = await TheGameLibrary.deploy();
        await theGameLibrary.deployed();

        return { theGameLibrary, addr1, addr2  };
    
    }


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Deploys with no exceptions", async function ()
    {
        // Arrange
        const { theGameLibrary, addr1 } = await loadFixture(deployTokenFixture);

        // Act

        // Expect
        expect(true).to.equal(true);
    }),
    

    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("Sets r between min/max when randomRange (min, max, n)", async function ()
    {
        // Arrange
        const { theGameLibrary, addr1 } = await loadFixture(deployTokenFixture);
        var nonce = 0;
        var min = 0;
        var max = 10;

        for (var i = 0; i< 100; i++)
        {
            // Act
            const r = await theGameLibrary.connect(addr1).randomRange(1, 10, nonce++);
            
            // Expect
            expect(r).to.greaterThanOrEqual(min).and.lessThanOrEqual(max);
        }
    }),


    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("createNcreateNewRewardForTestingewReward returns proper reward", async function ()
    {
        // Arrange
        const { theGameContract, theGameLibrary } = await loadFixture(deployTokenFixture);

        // Act
        const reward = await theGameLibrary.createNewRewardForTesting();
        
        // Expect
        expect(reward.Title).to.equal("");
        expect(reward.Type).to.equal(0);
        expect(reward.Price).to.equal(0);
    }),

    
    ///////////////////////////////////////////////////////////
    // TEST
    ///////////////////////////////////////////////////////////
    it("convertRewardToString returns proper format", async function ()
    {
        // Arrange
        const { theGameContract, theGameLibrary } = await loadFixture(deployTokenFixture);

        // Act
        const result = await theGameLibrary.convertRewardToStringForTesting();
        
        // Expect
        expect(result).to.equal("Title=|Type=0|Price=0");
    })
});



