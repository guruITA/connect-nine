import ButtonLink from "../ButtonLink";
import styles from "./ClosingCall.module.css";

export default function ClosingCall() {
  return (
    <section className={styles.section} aria-labelledby="closing-title">
      <div className={styles.inner}>
        <h2 id="closing-title" className={styles.title}>
          Ready to join?
        </h2>
        <p className={styles.lead}>
          Making an account is free and takes two minutes.
        </p>
        <ButtonLink to="/create-account">Make an account</ButtonLink>
      </div>
    </section>
  );
}
