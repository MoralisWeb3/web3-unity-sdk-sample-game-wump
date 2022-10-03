import { getSession } from "next-auth/react";
import { Table, Modal } from "antd";
import styles from "./styles.module.css";
import Moralis from "moralis";
import { useEffect, useState } from "react";
import { useAccount, useContractWrite, usePrepareContractWrite } from "wagmi";
import abi from "../abi.json";
import { ExclamationCircleOutlined, CheckCircleOutlined } from "@ant-design/icons";

const { confirm, success } = Modal;

function List({ user, nfts }) {
  const [id, setId] = useState();
  const { isConnected } = useAccount();
  const { config } = usePrepareContractWrite({
    addressOrName: "0x8039b52ce1b54f4700f53100a203e1fe2e76b4f3",
    contractInterface: abi,
    functionName: "burnPropertyNft",
    args: [id],
  });
  const { write, isSuccess } = useContractWrite(config);

  const showDeleteConfirm = () => {
    confirm({
      title: "Are you sure you want to burn this NFT?",
      icon: <ExclamationCircleOutlined />,
      content: `SimCity Propery with Token ID ${id}`,
      okText: "Yes",
      okType: "danger",
      okButtonProps: { disabled: !write },
      cancelText: "No",
      onOk() {
        write();
      },
      onCancel() {
        window.location.reload();
      },
    });
  };

  const showSuccess = () => {
    success({
      title: "NFT Succesfully Burnt!",
      icon: <CheckCircleOutlined />,
      content: 'Note, it may take a while for the changes in the blockchain to be reflected in your list.',
      okText: "Great",
      okType: "primary",
      onOk() {
        window.location.reload();
      }
    });
  };

  useEffect(() => {
    if (id && write && isConnected) {
      showDeleteConfirm();
    }
  }, [write]);

  useEffect(() => {
    if(isSuccess){
      showSuccess();
    }
  }, [isSuccess]);

  const columns = [
    {
      title: "Latitude",
      dataIndex: "latitude",
      key: "token_id",
    },
    {
      title: "Longitude",
      dataIndex: "longitude",
      key: "token_id",
    },
    {
      title: "Owner",
      dataIndex: "owner_of",
      key: "token_id",
    },
    {
      title: "Action",
      dataIndex: "",
      key: "token_id",
      render: (e) => {
        if (user && (user.address.toLowerCase() === e.owner_of.toLowerCase())) {
          return <a onClick={() => setId(e.token_id)}>Delete</a>;
        } else {
        return <div>--</div>;
        }
      },
    }
  ];

  return (
    <>
      <Table className={styles.table} dataSource={nfts} columns={columns} />
    </>
  );
}

export async function getServerSideProps(context) {
  const session = await getSession(context);

  await Moralis.start({ apiKey: process.env.MORALIS_API_KEY });

  const response = await Moralis.EvmApi.token.getNFTOwners({
    address: "0x8039b52ce1b54f4700f53100a203e1fe2e76b4f3",
    chain: 80001,
  });

  const nftList = response.raw.result;

  //Loop to get lat and long from token_uri

  for (let i = 0; i < nftList.length; i++) {
    let latLong = nftList[i].token_uri;
    const array = latLong.split("|");
    nftList[i].latitude = array[0];
    nftList[i].longitude = array[1];
  }

  ////////////////////////////////////////////

  if (!session) {
    return {
      props: { user: false, nfts: nftList },
    };
  }

  return {
    props: { user: session.user, nfts: nftList },
  };
}

export default List;
