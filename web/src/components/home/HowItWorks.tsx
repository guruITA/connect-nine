import Eyebrow from "./Eyebrow";
import styles from "./HowItWorks.module.css";

const STEPS = [
  {
    number: "01",
    when: "Two minutes",
    title: "Make an account",
    text: "Your email address and a password are enough to start.",
  },
  {
    number: "02",
    when: "Right after",
    title: "Create your profile",
    text: "Your first name, age band and town. Never your address or school.",
  },
  {
    number: "03",
    when: "Any time",
    title: "Find your way in",
    text: "Discover people and organisations who share your tags.",
  },
  {
    number: "04",
    when: "All year",
    title: "Meet each other",
    text: "Not only at the gala: Connect stays open all year round.",
  },
];

export default function HowItWorks() {
  return (
    <section
      id="how-it-works"
      className={styles.section}
      aria-labelledby="how-it-works-title"
    >
      <div className={styles.inner}>
        <Eyebrow>From account to first step</Eyebrow>
        <h2 id="how-it-works-title" className={styles.title}>
          How does Connect work?
        </h2>
        <ol className={styles.steps}>
          {STEPS.map((step) => (
            <li key={step.number} className={styles.step}>
              <span className={styles.number} aria-hidden="true">
                {step.number}
              </span>
              <span className={styles.when}>{step.when}</span>
              <h3 className={styles.stepTitle}>{step.title}</h3>
              <p className={styles.stepText}>{step.text}</p>
            </li>
          ))}
        </ol>
      </div>
    </section>
  );
}
