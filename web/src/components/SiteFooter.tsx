import { Link } from "react-router";
import glyph from "../assets/brand/glyph-footer.svg";
import styles from "./SiteFooter.module.css";

const COLUMNS = [
  {
    title: "Connect",
    links: [
      { to: "/", label: "Home" },
      { to: "/discover", label: "Discover" },
      { to: "/events", label: "Events" },
      { to: "/projects", label: "Projects" },
    ],
  },
  {
    title: "Account",
    links: [
      { to: "/login", label: "Log in" },
      { to: "/create-account", label: "Make an account" },
    ],
  },
  {
    title: "About",
    links: [
      { to: "/privacy", label: "Privacy" },
      { to: "/house-rules", label: "House rules" },
    ],
  },
];

export default function SiteFooter() {
  return (
    <footer className={styles.footer}>
      <div className={styles.inner}>
        <div className={styles.top}>
          <div className={styles.about}>
            <p className={styles.brand}>
              <img src={glyph} alt="" />
              <span className={styles.brandText}>
                <span className={styles.brandName}>Young Amstelland</span>{" "}
                <span className={styles.brandProduct}>Connect</span>
              </span>
            </p>
            <p className={styles.summary}>
              Young people, organisations and the foundation in Amstelland find
              each other here, all year.
            </p>
          </div>
          <nav className={styles.columns} aria-label="Footer">
            {COLUMNS.map((column) => (
              <div key={column.title} className={styles.column}>
                <h2 className={styles.columnTitle}>{column.title}</h2>
                <ul className={styles.links}>
                  {column.links.map((link) => (
                    <li key={link.to}>
                      <Link to={link.to} className={styles.link}>
                        {link.label}
                      </Link>
                    </li>
                  ))}
                </ul>
              </div>
            ))}
          </nav>
        </div>
        <div className={styles.credit}>
          <p className={styles.creditLabel}>An initiative of</p>
          <p className={styles.creditOrg}>
            <span className={styles.creditName}>Stichting WailSalutem</span>
            <span className={styles.creditLine}>
              Equal chances for young people.
            </span>
          </p>
        </div>
        <div className={styles.bottom}>
          <p className={styles.copyright}>
            © 2026 Stichting WailSalutem · Privacy
          </p>
          <p className={styles.product}>Young Amstelland Connect</p>
        </div>
      </div>
    </footer>
  );
}
