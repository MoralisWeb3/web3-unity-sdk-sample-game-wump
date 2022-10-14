import { getSession } from "next-auth/react";
import { Table, Button } from "antd";
import styles from "./styles.module.css";
import Moralis from "moralis";
import { useState } from "react";



function List({ user, nfts }) {


  const [dataS, setDataS] = useState(nfts);
  const [edit, setEdit] = useState(false);


  function changeData(addrs){

    console.log(addrs);
    if(!addrs) return;

    console.log("starting");
    let tempData = nfts.filter((e)=>e.address === addrs);

    setDataS(tempData);
    setEdit(true);

  }


  const columns = [
    {
      title: "Address",
      dataIndex: "address",
      key: "token_id",
    },
    {
      title: "Gold",
      dataIndex: "gold",
      key: "token_id",
      defaultSortOrder: 'descend',
      sorter: (a, b) => a.gold - b.gold,
    },
    {
      title: "Prizes",
      dataIndex: "prizes",
      key: "token_id",
      sorter: (a, b) => a.prizes - b.prizes,
    },

    {
      title: "Action",
      dataIndex: "",
      key: "token_id",
      render: (e) => {
        if (user && user.address.toLowerCase() === e.address.toLowerCase()) {
          return <a onClick={() => changeData(e.address)}>Edit</a>;
        } else {
          return <div>--</div>;
        }
      },
    },
  ];

  return (
    <>
      <Table
        className={styles.table}
        dataSource={dataS}
        columns={columns}
        pagination={{ pageSize: 8 }} /* scroll={{ y: 400 }} */
      />
      {edit && <Button danger onClick={()=> {
       setEdit(false);
       setDataS(nfts); 
      }}>Close</Button>}
    </>
  );
}

export async function getServerSideProps(context) {
  const session = await getSession(context);

  await Moralis.start({ apiKey: process.env.MORALIS_API_KEY });

  const response = await Moralis.EvmApi.nft.getNFTOwners({
    address: "0xa428423695D2052676A1d6e4a8C5d78d8C5dB7e8",
    chain: 80001,
  });

  const nftList = response.raw.result;

  //Loop to get number of prizes and gold

  const owners = nftList.map((e) => e.owner_of);
  const uniqueOwners = [...new Set(owners)];

  let ownerArray = [];



  function getGold(i) {
    return new Promise((resolve) => {
      setTimeout(async () => {
      let num = owners.filter((e) => e === uniqueOwners[i]).length;

      const tokensResponse = await Moralis.EvmApi.token.getWalletTokenBalances({
        address: uniqueOwners[i],
        chain: 80001,
        token_addresses: ["0xce800a0f3890fb6f97665b48ed97fed34d4ad30a"],
      });
      let token = 0;
      if (tokensResponse.raw[0]) {
        token = tokensResponse.raw[0].balance;
      }
      ownerArray.push({
        address: uniqueOwners[i],
        prizes: num,
        gold: token,
      });
      resolve();
    }, [300])
    })
  }

  async function getPromises() {
    for (let i = 0; i < uniqueOwners.length; i++) {
      await getGold(i)
      console.log(i);
      console.log(ownerArray);
    }
  }

  await getPromises();

  ////////////////////////////////////////////

  if (!session) {
    return {
      props: { user: false, nfts: ownerArray },
    };
  }

  return {
    props: { user: session.user, nfts: ownerArray },
  };
}

export default List;
