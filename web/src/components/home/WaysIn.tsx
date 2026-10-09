import { Link } from "react-router";
import photoDiscover from "../../assets/home/photo-discover.jpg";
import bulb from "../../assets/icons/bulb.svg";
import calendar from "../../assets/icons/calendar.svg";
import compass from "../../assets/icons/compass.svg";
import shield from "../../assets/icons/shield.svg";
import Eyebrow from "./Eyebrow";
import styles from "./WaysIn.module.css";

type TileLinkProps = {
  to: string;
  label: string;
  onBlue?: boolean;
};

function TileLink({ to, label, onBlue = false }: TileLinkProps) {
  return (
    <Link
      to={to}
      className={
        onBlue ? `${styles.tileLink} ${styles.onBlue}` : styles.tileLink
      }
    >
      {label} <span aria-hidden="true">→</span>
    </Link>
  );
}

export default function WaysIn() {
  return (
    <section className={styles.section} aria-labelledby="ways-in-title">
      <div className={styles.inner}>
        <Eyebrow>Where do you start?</Eyebrow>
        <h2 id="ways-in-title" className={styles.title}>
          Start where it suits you
        </h2>
        <div className={styles.grid}>
          <article className={`${styles.tile} ${styles.discover}`}>
            <div className={styles.tileBody}>
              <span className={styles.mark}>
                <img src={compass} alt="" />
              </span>
              <h3 className={styles.tileTitle}>Discover</h3>
              <p className={styles.tileText}>
                People and organisations near you who share your skills and
                interests.
              </p>
              <TileLink to="/discover" label="Open Discover" />
            </div>
            <img
              src={photoDiscover}
              alt=""
              className={styles.discoverPhoto}
              loading="lazy"
            />
          </article>

          <article className={`${styles.tile} ${styles.events}`}>
            <span className={`${styles.mark} ${styles.markOnBlue}`}>
              <img src={calendar} alt="" />
            </span>
            <h3 className={styles.tileTitle}>Events</h3>
            <p className={styles.tileText}>
              Meet-ups, workshops and the Young Amstelland Awards gala.
            </p>
            <TileLink to="/events" label="Open Events" onBlue />
          </article>

          <article className={`${styles.tile} ${styles.projects}`}>
            <span className={styles.mark}>
              <img src={bulb} alt="" />
            </span>
            <h3 className={styles.tileTitle}>Projects</h3>
            <p className={styles.tileText}>
              Start your own idea or join one that needs hands.
            </p>
            <TileLink to="/projects" label="Open Projects" />
          </article>

          <article className={`${styles.tile} ${styles.promise}`}>
            <span className={styles.mark}>
              <img src={shield} alt="" />
            </span>
            <Eyebrow>Our promise</Eyebrow>
            <h3 className={styles.tileTitle}>Your profile, your choice</h3>
            <p className={styles.tileText}>
              Connect never asks for your exact age, birth date, school or
              street address. Young people under 16 join with a parent who says
              yes.
            </p>
          </article>
        </div>
      </div>
    </section>
  );
}
