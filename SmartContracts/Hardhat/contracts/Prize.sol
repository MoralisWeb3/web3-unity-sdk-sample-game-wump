// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;


///////////////////////////////////////////////////////////
// IMPORTS
///////////////////////////////////////////////////////////
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721URIStorage.sol";
import "@openzeppelin/contracts/utils/Counters.sol";
import "hardhat/console.sol";


///////////////////////////////////////////////////////////
// CLASS
//      *   Description         :    
//      *   Deployment Address  :   
///////////////////////////////////////////////////////////
contract Prize is ERC721URIStorage 
{

    ///////////////////////////////////////////////////////////
    // FIELDS
    //      *   Values stored on contract
    ///////////////////////////////////////////////////////////

    // Auto generates tokenIds
    using Counters for Counters.Counter;
    Counters.Counter private _tokenIds;

    // User address who owns this contract instance
    address _owner;


    ///////////////////////////////////////////////////////////
    // CONSTRUCTOR
    //      *   Runs when contract is executed
    ///////////////////////////////////////////////////////////
    constructor(string memory name, string memory symbol) ERC721 (name, symbol) 
    {
        _owner = msg.sender;

        console.log(
            "Prize.constructor() _owner = %s",
            _owner
        );
    }


    ///////////////////////////////////////////////////////////
    // FUNCTION: 
    //      *   Create New Prize
    ///////////////////////////////////////////////////////////
    function mintNft(address origin, string memory tokenURI) public
    {
        uint256 newItemId = _tokenIds.current();
        _tokenIds.increment();

        _mint(origin, newItemId);
        _setTokenURI(newItemId, tokenURI);
    }

    ///////////////////////////////////////////////////////////
    // FUNCTION: 
    //      *   Delete Existing Prize
    ///////////////////////////////////////////////////////////
    function burnNft (uint256 tokenId) public
    {
        _burn(tokenId);
    }

    ///////////////////////////////////////////////////////////
    // FUNCTION: 
    //      *   Delete Existing Prizes
    ///////////////////////////////////////////////////////////
    function burnNfts(uint256[] calldata tokenIds) public 
    {
        for (uint i=0; i<tokenIds.length; i++) {
            burnNft (tokenIds[i]);
        }
    }
}


