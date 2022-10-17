import "antd/dist/antd.css";
import { Layout } from "antd";
import styles from "../styles.module.css";
import { useRouter } from "next/router";
import { getSession } from "next-auth/react";

const { Header, Content, Footer } = Layout;
const githubLink =
  "https://github.com/MoralisWeb3/web3-unity-sdk-sample-game-wump";

const getShortWallet = (wal) => {
  return `${wal.slice(0, 4)}...${wal.slice(38)}`;
};

function Structure({ children, user }) {
  const { push } = useRouter();
  return (
    <Layout className={styles.container}>
      <Header className={styles.header}>WUMP - Web Companion App</Header>
      <Content className={styles.content}>{children}</Content>
      <Footer className={styles.footer}>
        <div
          className={styles.menuItem}
          onClick={() => window.open(githubLink)}
        >
          Github
        </div>
        <div className={styles.menuItem} onClick={() => push("/")}>
          Home
        </div>
        <div className={styles.menuItem} onClick={() => push("/authenticate")}>
          {children.props.user
            ? getShortWallet(children.props.user.address)
            : "Authenticate"}
        </div>
        <div className={styles.menuItem} onClick={() => push("/list")}>
          List
        </div>
      </Footer>
    </Layout>
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

export default Structure;
