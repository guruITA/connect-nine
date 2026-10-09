import ButtonLink from "../ButtonLink";
import Eyebrow from "./Eyebrow";
import styles from "./ForOrganisations.module.css";

export default function ForOrganisations() {
  return (
    <section className={styles.section} aria-labelledby="partner-title">
      <div className={styles.inner}>
        <div className={styles.panel}>
          <div className={styles.text}>
            <Eyebrow tone="onGreen">For organisations</Eyebrow>
            <h2 id="partner-title" className={styles.title}>
              Reach young talent in Amstelland
            </h2>
            <p className={styles.lead}>
              Clubs, schools, companies and municipalities show who they are to
              young people who want to help.
            </p>
          </div>
          <div className={styles.actions}>
            <ButtonLink to="/create-account" variant="lightOnGreen" wide>
              Make an account
            </ButtonLink>
            {/* No click target in the Figma prototype yet, so this is not a link. */}
            <span className={styles.pill}>Contact the foundation</span>
          </div>
        </div>
      </div>
    </section>
  );
}
