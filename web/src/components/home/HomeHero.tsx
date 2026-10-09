import glyph from "../../assets/brand/glyph-emblem.svg";
import circle from "../../assets/home/emblem-circle.svg";
import ButtonLink from "../ButtonLink";
import Eyebrow from "./Eyebrow";
import styles from "./HomeHero.module.css";

const STATS = [
  { value: "5", label: "towns in Amstelland" },
  { value: "15 to 30", label: "the age of our young people" },
  { value: "1", label: "place to find each other" },
];

export default function HomeHero() {
  return (
    <section className={styles.hero} aria-labelledby="home-title">
      <div className={styles.inner}>
        <div className={styles.row}>
          <div className={styles.text}>
            <Eyebrow tone="onNavy">
              Connect · young people aged 15 to 30
            </Eyebrow>
            <h1 id="home-title" className={styles.title}>
              Do work that matters, close to home
            </h1>
            <p className={styles.lead}>
              Connect brings young people in Amstelland together with
              organisations, projects and events, all year round and not only at
              the gala.
            </p>
            <div className={styles.actions}>
              <ButtonLink to="/create-account">Make an account</ButtonLink>
              <ButtonLink to="#how-it-works" variant="light">
                How does it work?
              </ButtonLink>
            </div>
          </div>
          <div className={styles.emblem} aria-hidden="true">
            <img src={circle} alt="" className={styles.circle} />
            <img src={glyph} alt="" className={styles.glyph} />
          </div>
        </div>
        <ul className={styles.stats}>
          {STATS.map((stat) => (
            <li key={stat.label} className={styles.stat}>
              <span className={styles.statValue}>{stat.value}</span>
              <span className={styles.statLabel}>{stat.label}</span>
            </li>
          ))}
        </ul>
      </div>
    </section>
  );
}
