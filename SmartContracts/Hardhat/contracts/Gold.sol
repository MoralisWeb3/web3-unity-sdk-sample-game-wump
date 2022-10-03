// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;


///////////////////////////////////////////////////////////
// IMPORTS
///////////////////////////////////////////////////////////
import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "hardhat/console.sol";
import "@openzeppelin/contracts/utils/Strings.sol";

///////////////////////////////////////////////////////////
// CLASS
//      *   Description         :   Each contract instance 
//                                  manages CRUD for its 
//                                  greeting text message
//      *   Deployment Address  :   
///////////////////////////////////////////////////////////
contract Gold is ERC20 
{

    ///////////////////////////////////////////////////////////
    // FIELDS
    //      *   Values stored on contract
    ///////////////////////////////////////////////////////////


    // User address who owns this contract instance
    address _owner;


    ///////////////////////////////////////////////////////////
    // CONSTRUCTOR
    //      *   Runs when contract is executed
    ///////////////////////////////////////////////////////////
    constructor() ERC20 ("Gold", "GOLD") 
    {
        _owner = msg.sender;

        console.log(
            "Gold.constructor() _owner = %s",
            _owner
        );
    }

    ///////////////////////////////////////////////////////////
    // MODIFIERS 
    ///////////////////////////////////////////////////////////
    modifier ensureHasBalance (address userAddress, uint256 amount)
    {
        // Validate
        require(getGold(userAddress) >= amount, "Must be registered");

        // Execute rest of function
      _;
    }

    ///////////////////////////////////////////////////////////
    // FUNCTION: CRUD
    //      *   Get gold amount for the calling address
    //      *   Changes no contract state, so call via 
    //          RunContractFunction
    ///////////////////////////////////////////////////////////
    function getGold(address origin) public view returns (uint256 balance)
    {
        balance = balanceOf(origin);
    }

    ///////////////////////////////////////////////////////////
    // FUNCTION: CRUD
    //      *   Set gold amount for the calling address
    //      *   Changes contract state, so call via 
    //          ExecuteContractFunction
    ///////////////////////////////////////////////////////////
    function setGold(address origin, uint256 targetBalance) public 
    {
        uint256 oldBalance = getGold(origin);
        int delta = int(targetBalance) - int(oldBalance);
        
        if (delta > 0)
        {
            // console.log ('delta %s POS ', uint256(delta));
            addGold (origin, uint256(delta));
        }
        else if (delta < 0)
        {
            //console.log ('delta %s NEG ', uint256(-delta));
            removeGold (origin, uint256(-delta));
        }
    }


    ///////////////////////////////////////////////////////////
    // FUNCTION: CRUD
    //      *   Set gold amount for the calling address
    //      *   Changes contract state, so call via 
    //          ExecuteContractFunction
    ///////////////////////////////////////////////////////////
    function setGoldBy(address origin, int deltaBalance) public
    {
        if (deltaBalance > 0)
        {
            addGold (origin, uint256(deltaBalance));
        }
        else if (deltaBalance < 0)
        {
            removeGold (origin, uint256(-deltaBalance));
        }
    }


    ///////////////////////////////////////////////////////////
    // FUNCTION: CRUD
    //      *   Transfer gold amount from origin to the toAddress
    //      *   Changes contract state, so call via 
    //          ExecuteContractFunction
    ///////////////////////////////////////////////////////////
    function transferGold(address origin, address toAddress, uint256 amount) ensureHasBalance (origin, amount) ensureHasBalance (toAddress, 0) public
    {
        removeGold(origin, amount);
        addGold(toAddress, amount);
    }


    ///////////////////////////////////////////////////////////
    // FUNCTION: CRUD
    //      *   Add gold to the calling address
    //      *   Changes contract state, so call via 
    //          ExecuteContractFunction
    ///////////////////////////////////////////////////////////
    function addGold(address origin, uint256 amount) private 
    {
        _mint(origin, amount);
    }


    ///////////////////////////////////////////////////////////
    // FUNCTION: CRUD
    //      *   Remove gold to the calling address
    //      *   Changes contract state, so call via 
    //          ExecuteContractFunction
    ///////////////////////////////////////////////////////////
    function removeGold(address origin, uint256 amount) private 
    {
        _burn(origin, amount);
    }
}


