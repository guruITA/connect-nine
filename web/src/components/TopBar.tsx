import { Link } from "react-router";
import styles from "./TopBar.module.css";

export default function TopBar() {
  return (
    <header className={styles.topBar}>
      <nav className={styles.nav} aria-label="Main">
        <Link to="/" className={styles.brand}>
          Connect
        </Link>
        <Link to="/create-account" className={styles.action}>
          Make an account
        </Link>
      </nav>
    </header>
  );
}
