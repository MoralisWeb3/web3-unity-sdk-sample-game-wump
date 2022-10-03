import { getSession, signOut } from "next-auth/react";
import { MetaMaskConnector } from "wagmi/connectors/metaMask";
import { signIn } from "next-auth/react";
import { useAccount, useConnect, useSignMessage, useDisconnect } from "wagmi";
import { useRouter } from "next/router";
import axios from "axios";
import { Button, message } from "antd";

const {warning} = message;

function Authenticate({ user }) {
  const { connectAsync, isError } = useConnect();
  const { disconnectAsync } = useDisconnect();
  const { isConnected } = useAccount();
  const { signMessageAsync } = useSignMessage();
  const { push } = useRouter();

  const showInfo = () => {
    warning({
        content:'Wallet request was rejected by the user',
        style: {
            position: "absolute",
            left: '10px',
            top: '6px'
          },})
  };

  const handleAuth = async () => {
    if (isConnected) {
      await disconnectAsync();
    }

    let userData = { network: "evm" };
    try {
      const { account, chain } = await connectAsync({
        connector: new MetaMaskConnector(),
      });
      userData.address = account;
      userData.chain = chain.id;
    } catch (e) {
      showInfo();
      return;
    }

    const { data } = await axios.post("/api/auth/request-message", userData, {
      headers: {
        "content-type": "application/json",
      },
    });

    const message = data.message;

    let signature;
    
    try{
        signature = await signMessageAsync({ message });
    }catch(e){
      showInfo();
      return;
    }
    

    // redirect user after success authentication to '/authenticate' page
    const { url } = await signIn("credentials", {
      message,
      signature,
      redirect: false,
      callbackUrl: "/authenticate",
    });
    /**
     * instead of using signIn(..., redirect: "/authenticate")
     * we get the url from callback and push it to the router to avoid page refreshing
     */
    push(url);
  };

  return (
    <div>
      {!user && (
        <Button type="primary" onClick={() => handleAuth()}>
          Authenticate via Metamask
        </Button>
      )}
      {user && (
        <>
          <div>Signed in user</div>
          <div>{user.address}</div>
          <Button type="danger" onClick={() => signOut()}>
            Sign Out
          </Button>
        </>
      )}
    </div>
  );
}

export async function getServerSideProps(context) {
  let session = await getSession(context);

  if (!session) {
    return {
      props: { user: false },
    };
  }

  return {
    props: { user: session.user },
  };
}

export default Authenticate;
