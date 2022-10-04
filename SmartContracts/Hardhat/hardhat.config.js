///////////////////////////////////////////////////////////
// REQUIRES
///////////////////////////////////////////////////////////
require("dotenv").config();
require("@nomicfoundation/hardhat-toolbox");
require("hardhat-gas-reporter");

///////////////////////////////////////////////////////////
// EXPORTS
///////////////////////////////////////////////////////////
/**
 * @type import('hardhat/config').HardhatUserConfig
 */
module.exports = {
  solidity: "0.8.9",
  networks: {
    hardhat: {},
    polygonMumbai: {
      url: process.env.POLYGON_MUMBAI_NETWORK_URL,
      accounts: [process.env.WEB3_WALLET_PRIVATE_KEY]
    }
  },
  etherscan: {
    apiKey: {
      polygonMumbai: process.env.POLYGON_MUMBAI_API_KEY
    }
  },
  gasReporter: {
    currency: 'USD',
    enabled: true
  }
};


///////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////
task("ccct", "Clean, Compile, Coverage, & Test the Greeter.sol").setAction(async () => {

  // Works!
  await hre.run("clean");
  await hre.run("compile");
  await hre.run("coverage");
  await hre.run("test");
});