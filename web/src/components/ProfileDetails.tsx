import { AGE_BAND_LABELS } from "../api/memberProfiles";
import type { AgeBand } from "../api/memberProfiles";
import styles from "./ProfileDetails.module.css";

type ProfileDetailsProps = {
  ageBand: AgeBand;
  region?: string;
  bio?: string | null;
  showBio?: boolean;
};

// Age band, region and short line of a profile, as a description list so a screen reader
// reads each label with its value. An empty or missing value is left out with its label.
// About you keeps its dt for a screen reader but shows no visible label, as in the frame.
export default function ProfileDetails({
  ageBand,
  region,
  bio,
  showBio = true,
}: ProfileDetailsProps) {
  return (
    <dl className={styles.list}>
      <div className={styles.field}>
        <dt className={styles.term}>Age band</dt>
        <dd className={styles.description}>{AGE_BAND_LABELS[ageBand]}</dd>
      </div>
      {region && (
        <div className={styles.field}>
          <dt className={styles.term}>Region</dt>
          <dd className={styles.description}>{region}</dd>
        </div>
      )}
      {showBio && bio && (
        <div className={`${styles.field} ${styles.bioField}`}>
          <dt className={styles.visuallyHidden}>About you</dt>
          <dd className={styles.bio}>{bio}</dd>
        </div>
      )}
    </dl>
  );
}
