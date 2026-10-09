import photoFoundation from "../../assets/home/photo-foundation.jpg";
import Eyebrow from "./Eyebrow";
import styles from "./FoundationStory.module.css";

export default function FoundationStory() {
  return (
    <section className={styles.section} aria-labelledby="foundation-title">
      <div className={styles.inner}>
        <div className={styles.tile}>
          <img
            src={photoFoundation}
            alt=""
            className={styles.photo}
            loading="lazy"
          />
          <div className={styles.text}>
            <Eyebrow>Background</Eyebrow>
            <h2 id="foundation-title" className={styles.title}>
              Made by a foundation that believes in young people
            </h2>
            <p className={styles.body}>
              Connect is an initiative of the WailSalutem Foundation. In its
              Social Innovation Lab young people work every week on technology,
              sustainability and social impact.
            </p>
            {/* No click target in the Figma prototype yet, so this is not a link. */}
            <span className={styles.pill}>About the foundation</span>
          </div>
        </div>
      </div>
    </section>
  );
}
