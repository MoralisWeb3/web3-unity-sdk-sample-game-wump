// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;


///////////////////////////////////////////////////////////
// IMPORTS
///////////////////////////////////////////////////////////
import "hardhat/console.sol";
import "contracts/Gold.sol";
import "contracts/Prize.sol";
import "classes/TransferLog.sol";
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

    // Stores address of the Prize contract, to be called
    address _prizeContractAddress;

    mapping(address => bool) private _isRegistered;

    // Stores the most recent TransferLog
    mapping (address => TransferLog) private _lastTransferLog;

    ///////////////////////////////////////////////////////////
    // CONSTRUCTOR
    //      *   Runs when contract is executed
    ///////////////////////////////////////////////////////////
    constructor(address goldContractAddress, address prizeContractAddress) 
    {
        _goldContractAddress = goldContractAddress;
        _prizeContractAddress = prizeContractAddress;

        console.log(
            "TheGameContract.constructor() _goldContractAddress = %s, _prizeContractAddress = %s",
            _goldContractAddress,
            _prizeContractAddress
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


    function getTransferLogHistory(address userAddress) external view ensureIsRegistered (userAddress) returns (string memory transferLogString)
    {
        //Check for null
        if (_lastTransferLog[userAddress].Type == 0)
        {
            transferLogString = "";
        }
        else 
        {
            transferLogString =  TheGameLibrary.convertTransferLogToString(_lastTransferLog[userAddress]);
        }
    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: REGISTRATION
    ///////////////////////////////////////////////////////////
    function register() public
    {
        _isRegistered[msg.sender] = true;

        // Give prizes to new registrant
        for (uint i = 0; i < TheGameLibrary.PrizesOnRegister; i++)
        {
            string memory imageUrl = TheGameLibrary.getRandomImageUrl();
            addPrize (imageUrl);
        }

        // Give gold to new registrant
        setGold(TheGameLibrary.GoldOnRegister);
    }


    function unregister(uint256[] calldata tokenIds) public ensureIsRegistered (msg.sender)
    {
        // Update gold
        setGold(TheGameLibrary.GoldOnUnregister);

        // Update prizes
        deleteAllPrizes(tokenIds);

        //Then unregister
        _isRegistered[msg.sender] = false;

    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: CLEAR DATA
    ///////////////////////////////////////////////////////////
    function safeReregisterAndDeleteAllPrizes(uint256[] calldata tokenIds) external
    {
        // Do not require isRegistered for this method to run
        bool isRegistered = getIsRegistered(msg.sender);
        if (isRegistered)
        {
            unregister(tokenIds);
        }

        register();
    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: GOLD
    ///////////////////////////////////////////////////////////
    function setGold(uint256 targetBalance) ensureIsRegistered (msg.sender) public
    {
        Gold(_goldContractAddress).setGold(msg.sender, targetBalance);
    }


    // For simplicity: The toAddress is not required to be IsRegistered
    function transferGold(address toAddress) ensureIsRegistered (msg.sender) public
    {
        uint256 amount = TheGameLibrary.GoldOnTransfer;
        Gold(_goldContractAddress).transferGold(msg.sender, toAddress, amount);

        _lastTransferLog[msg.sender] = TransferLog (
        {
            FromAddress: msg.sender,
            ToAddress: toAddress,
            Type: TheGameLibrary.GoldType,
            Amount: amount
        });
    }


    function setGoldBy(int delta) ensureIsRegistered (msg.sender) public
    {
        Gold(_goldContractAddress).setGoldBy(msg.sender, delta); 
    }


    ///////////////////////////////////////////////////////////
    // FUNCTIONS: PRIZE
    ///////////////////////////////////////////////////////////
    function addPrize(string memory tokenURI) ensureIsRegistered (msg.sender)  public 
    {
        Prize(_prizeContractAddress).mintNft(msg.sender, tokenURI);
    }


    function deleteAllPrizes(uint256[] calldata tokenIds) ensureIsRegistered (msg.sender)  public
    {
        Prize(_prizeContractAddress).burnNfts(tokenIds); 
    }

    // For simplicity: The toAddress is not required to be IsRegistered
    function transferPrize(address toAddress, uint256 tokenId) ensureIsRegistered (msg.sender) public
    {
        Prize(_prizeContractAddress).transferNft(msg.sender, toAddress, tokenId);

        uint256 amount = TheGameLibrary.PrizesOnTransfer;
        _lastTransferLog[msg.sender] = TransferLog (
        {
            FromAddress: msg.sender,
            ToAddress: toAddress,
            Type: TheGameLibrary.PrizeType,
            Amount: amount 
        });
    }

    //TODO: Keep this?

    function getIsOwnerOfPrize(uint256 tokenId) public view returns (bool isOwnerOfPrize) 
    {
        isOwnerOfPrize = Prize(_prizeContractAddress).ownerOf(tokenId) == msg.sender;
    }
}


