// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;


///////////////////////////////////////////////////////////
// IMPORTS
///////////////////////////////////////////////////////////
import "hardhat/console.sol";
import "contracts/Gold.sol";
import "contracts/TreasurePrize.sol";
import "classes/Reward.sol";
import { TheGameLibrary } from "libraries/TheGameLibrary.sol";


///////////////////////////////////////////////////////////
// CLASS
//      *   Description         :   The proxy contact
//                                  for all other contracts
//      *   Deployment Address  :   
///////////////////////////////////////////////////////////
contract TheGameContract
{

    ///////////////////////////////////////////////////////////
    // FIELDS
    //      *   Values stored on contract
    ///////////////////////////////////////////////////////////

    // Stores address of the Gold contract, to be called
    address _goldContractAddress;

    // Stores address of the TreasurePrize contract, to be called
    address _treasurePrizeContractAddress;

    mapping(address => bool) private _isRegistered;

    // Stores the most recent reward
    mapping (address => Reward) private _lastReward;

    ///////////////////////////////////////////////////////////
    // CONSTRUCTOR
    //      *   Runs when contract is executed
    ///////////////////////////////////////////////////////////
    constructor(address goldContractAddress, address treasurePrizeContractAddress) 
    {
        _goldContractAddress = goldContractAddress;
        _treasurePrizeContractAddress = treasurePrizeContractAddress;

        console.log(
            "TheGameContract.constructor() _goldContractAddress = %s, _treasurePrizeContractAddress = %s",
            _goldContractAddress,
            _treasurePrizeContractAddress
        );
    }
    
    
    ///////////////////////////////////////////////////////////
    // MODIFIERS 
    ///////////////////////////////////////////////////////////
    modifier ensureIsRegistered (address userAddress)
    {
        // Validate
        require(_isRegistered[userAddress], "Must be registered");

        // Execute rest of function
      _;
    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: GETTERS
    ///////////////////////////////////////////////////////////
    function getIsRegistered(address userAddress) public view returns (bool isRegistered) 
    {
        // DISCLAIMER -- NOT A PRODUCTION READY CONTRACT
        // CONSIDER TO ADD MORE SECURITY CHECKS TO EVERY FUNCTION
        // require(msg.sender == _owner);
        isRegistered = _isRegistered[userAddress];
    }


    function getGold(address userAddress) public view ensureIsRegistered (userAddress) returns (uint256 balance) 
    {
        
        balance = Gold(_goldContractAddress).getGold(userAddress);
    }


    function getRewardsHistory(address userAddress) external view ensureIsRegistered (userAddress) returns (string memory rewardString)
    {
        rewardString =  TheGameLibrary.convertRewardToString(_lastReward[userAddress]);
    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: REGISTRATION
    ///////////////////////////////////////////////////////////
    function register() public
    {
        _isRegistered[msg.sender] = true;
        setGold(TheGameLibrary.GoldOnRegister);
    }


    function unregister() public ensureIsRegistered (msg.sender)
    {

        //Update gold first
        setGold(TheGameLibrary.GoldOnUnregister);

        //Then unregister
        _isRegistered[msg.sender] = false;

    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: REWARDS
    ///////////////////////////////////////////////////////////
    function startGameAndGiveRewards(uint256 goldAmount) ensureIsRegistered (msg.sender) external
    {
        require(goldAmount > 0, "goldAmount must be > 0 to start the game");

        require(getGold(msg.sender) >= goldAmount, "getGold() must be >= goldAmount to start the game");

        // Deduct gold
        setGoldBy(-int(goldAmount));

        // The higher the goldAmount paid, the higher the POTENTIAL Prize Price Value
        uint random = TheGameLibrary.randomRange (0, 100 + goldAmount, 1);
        uint price = random;
        uint theType = 0;
        string memory title = "";

        if (random < TheGameLibrary.MaxRandomForGold)
        {
            // REWARD: Gold!
            theType = TheGameLibrary.GoldType;
            title = "This is gold.";
            setGoldBy (int(price));
        } 
        else 
        {
            // REWARD: Prize!
            theType = TheGameLibrary.PrizeType;
            title = "This is an nft.";
        }

        _lastReward[msg.sender] = Reward (
        {
            Title: title,
            Type: theType,
            Price: price
        });

        if (theType == 2)
        {
            //NOTE: Metadata structure must match in both: TheGameContract.sol and TreasurePrizeDto.cs
            string memory metadata = TheGameLibrary.convertRewardToString(_lastReward[msg.sender]);
            addTreasurePrize (metadata);     
        }
    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: CLEAR DATA
    ///////////////////////////////////////////////////////////
    function safeReregisterAndDeleteAllTreasurePrizes(uint256[] calldata tokenIds) external
    {
        // Do not require isRegistered for this method to run
        bool isRegistered = getIsRegistered(msg.sender);
        if (isRegistered)
        {
            unregister();
        }

        register();
        deleteAllTreasurePrizes(tokenIds);
    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: GOLD
    ///////////////////////////////////////////////////////////
    function setGold(uint256 targetBalance) ensureIsRegistered (msg.sender) public
    {
        Gold(_goldContractAddress).setGold(msg.sender, targetBalance);
    }


    function setGoldBy(int delta) ensureIsRegistered (msg.sender) public
    {
        Gold(_goldContractAddress).setGoldBy(msg.sender, delta); 
    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: TREASURE PRIZE
    ///////////////////////////////////////////////////////////
    function addTreasurePrize(string memory tokenURI) ensureIsRegistered (msg.sender)  public 
    {
        TreasurePrize(_treasurePrizeContractAddress).mintNft(msg.sender, tokenURI);
    }


    function deleteAllTreasurePrizes(uint256[] calldata tokenIds) ensureIsRegistered (msg.sender)  public
    {
        TreasurePrize(_treasurePrizeContractAddress).burnNfts(tokenIds); 
    }


    function sellTreasurePrize(uint256 tokenId) ensureIsRegistered (msg.sender)  external
    {
        //TODO reward gold for the specific prize. Can I find the metadata from the tokenId here in Solidity?

        //Then burn the prize
        TreasurePrize(_treasurePrizeContractAddress).burnNft(tokenId);
    }
}


