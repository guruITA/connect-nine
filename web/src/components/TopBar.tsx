import { Link, NavLink } from "react-router";
import glyph from "../assets/brand/glyph-header.svg";
import ButtonLink from "./ButtonLink";
import styles from "./TopBar.module.css";

const TABS = [
  { to: "/", label: "Home" },
  { to: "/discover", label: "Discover" },
  { to: "/events", label: "Events" },
  { to: "/projects", label: "Projects" },
  { to: "/login", label: "Log in" },
];

export default function TopBar() {
  return (
    <header className={styles.topBar}>
      <div className={styles.inner}>
        <Link to="/" className={styles.brand}>
          <img src={glyph} alt="" />
          <span className={styles.brandText}>
            <span className={styles.brandName}>Young Amstelland</span>{" "}
            <span className={styles.brandProduct}>Connect</span>
          </span>
        </Link>
        <nav className={styles.nav} aria-label="Main">
          <ul className={styles.tabs}>
            {TABS.map((tab) => (
              <li key={tab.to}>
                <NavLink
                  to={tab.to}
                  end
                  className={({ isActive }) =>
                    isActive ? `${styles.tab} ${styles.current}` : styles.tab
                  }
                >
                  {tab.label}
                </NavLink>
              </li>
            ))}
          </ul>
        </nav>
        <div className={styles.action}>
          <ButtonLink to="/create-account">Make an account</ButtonLink>
        </div>
      </div>
    </header>
  );
}
