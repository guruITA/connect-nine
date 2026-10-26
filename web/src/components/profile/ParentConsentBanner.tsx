import styles from "./ParentConsentBanner.module.css";

// Shown on My profile while a parent has not given permission yet for a profile under 18
// (#392, #395). Never names an address or a date.
export default function ParentConsentBanner() {
  return (
    <p role="status" className={styles.banner}>
      Only you can see your profile for now. Other members see it once your
      parent gives permission through the email we sent.
    </p>
  );
}
