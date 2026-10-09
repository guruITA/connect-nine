import styles from "./Home.module.css";

export default function Home() {
  return (
    <section className={styles.home}>
      <h1 className={styles.title}>Connect</h1>
      <p className={styles.intro}>
        On Connect, young people from Amstelland and the organisations around
        them find each other.
      </p>
    </section>
  );
}
