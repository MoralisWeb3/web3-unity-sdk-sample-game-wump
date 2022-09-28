///////////////////////////////////////////////////////////
// REQUIRES
///////////////////////////////////////////////////////////
const hre = require("hardhat");
const fs = require('fs');

///////////////////////////////////////////////////////////
// MAIN
///////////////////////////////////////////////////////////
async function main()
{

  ///////////////////////////////////////////////////////////
  // DEPLOYMENT
  ///////////////////////////////////////////////////////////

  // TheGameLibrary
  const TheGameLibrary = await ethers.getContractFactory("TheGameLibrary");
  const theGameLibrary = await TheGameLibrary.deploy();
  await theGameLibrary.deployed();

  // Gold Contract
  const Gold = await hre.ethers.getContractFactory("Gold");
  const gold = await Gold.deploy();
  await gold.deployed();

  // TreasurePrize Contract
  const TreasurePrize = await ethers.getContractFactory("TreasurePrize");
  const treasurePrize = await TreasurePrize.deploy();
  await treasurePrize.deployed();

  // TheGameContract
  const TheGameContract = await ethers.getContractFactory("TheGameContract", {
    libraries: {
        TheGameLibrary: theGameLibrary.address,
    },
  });
  const theGameContract = await TheGameContract.deploy(gold.address, treasurePrize.address);
  await theGameContract.deployed();


  ///////////////////////////////////////////////////////////
  // UNITY-FRIENDLY OUTPUT
  ///////////////////////////////////////////////////////////
  const abiFile = JSON.parse(fs.readFileSync('./artifacts/contracts/TheGameContract.sol/TheGameContract.json', 'utf8'));
  const abi = JSON.stringify(abiFile.abi).replaceAll ('"','\\"',);
  console.log("\n");
  console.log("DEPLOYMENT COMPLETE: COPY TO UNITY...");
  console.log("\n");
  console.log("       protected override void SetContractDetails()");
  console.log("       {\n");
  console.log("         _treasurePrizeContractAddress  = \"%s\";", treasurePrize.address);
  console.log("         _address  = \"%s\";", theGameContract.address);
  console.log("         _abi      = \"%s\";\n", abi);
  console.log("       }\n");
  console.log("\n");


  ///////////////////////////////////////////////////////////
  // WAIT
  ///////////////////////////////////////////////////////////
  console.log("WAIT ...");
  console.log("\n");
  await theGameContract.deployTransaction.wait(7);


  ///////////////////////////////////////////////////////////
  // VERIFY
  ///////////////////////////////////////////////////////////
  console.log("VERIFICATION STARTING ...");
  console.log("\n");
  await hre.run("verify:verify", {
    address: theGameContract.address,
    constructorArguments: [
      gold.address, 
      treasurePrize.address],
  });

  ///////////////////////////////////////////////////////////
  // LOG OUT DATA FOR USAGE IN UNITY
  ///////////////////////////////////////////////////////////
  console.log("VERIFICATION COMPLETE");
  console.log("\n");

}


///////////////////////////////////////////////////////////
// EXECUTE
///////////////////////////////////////////////////////////
main()
    .then(() => process.exit(0))
    .catch((error) => {
      console.error(error);
      process.exit(1);
    });
