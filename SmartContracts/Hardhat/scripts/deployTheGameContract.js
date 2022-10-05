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

  // Prize Contract
  const Prize = await ethers.getContractFactory("Prize");
  const prize = await Prize.deploy();
  await prize.deployed();

  // TheGameContract
  const TheGameContract = await ethers.getContractFactory("TheGameContract", {
    libraries: {
        TheGameLibrary: theGameLibrary.address,
    },
  });

  const theGameContract = await TheGameContract.deploy(gold.address, prize.address);
  await theGameContract.deployed();


  ///////////////////////////////////////////////////////////
  // UNITY-FRIENDLY OUTPUT
  ///////////////////////////////////////////////////////////
  const abiFile = JSON.parse(fs.readFileSync('./artifacts/contracts/TheGameContract.sol/TheGameContract.json', 'utf8'));
  const abi = JSON.stringify(abiFile.abi).replaceAll ('"','\\"',);
  console.log("\n");
  console.log("DEPLOYMENT COMPLETE: COPY TO UNITY...");
  console.log("\n");
  console.log("\tprotected override void SetContractDetails()");
  console.log   ("\t{\n");
  console.log   ("\t\t_prizeContractAddress  = \"%s\";", prize.address);
  console.log   ("\t\t_address  = \"%s\";", theGameContract.address);
  console.log   ("\t\t_abi      = \"%s\";\n", abi);
  console.log   ("\t}\n");
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
  console.log("VERIFICATION STARTING (theGameContract)...");
  console.log("\n");
  await hre.run("verify:verify", {
    address: theGameContract.address,
    constructorArguments: [
      gold.address, 
      prize.address],
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
