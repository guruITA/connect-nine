import { Link } from "react-router";
import { AGE_BAND_LABELS } from "../api/memberProfiles";
import type { AgeBand } from "../api/memberProfiles";
import shieldIcon from "../assets/icons/shield.svg";
import FormField from "../components/FormField";
import Eyebrow from "../components/home/Eyebrow";
import { useCreateProfile } from "../hooks/useCreateProfile";
import styles from "./CreateProfile.module.css";

const AGE_BANDS: AgeBand[] = ["Adolescent", "YoungAdult", "Adult"];
const BIO_MAX_LENGTH = 280;

// The page Create your profile, as the Figma frame "Create your profile" draws it: a band
// with the title, the form in a card and a panel beside it that says what Connect never asks.
export default function CreateProfile() {
  const {
    firstName,
    ageBand,
    region,
    bio,
    fieldErrors,
    generalMessage,
    saving,
    saved,
    firstNameRef,
    ageBandRef,
    regionRef,
    bioRef,
    setFirstName,
    setAgeBand,
    setRegion,
    setBio,
    submit,
  } = useCreateProfile();

  const bioCharactersLeft = BIO_MAX_LENGTH - bio.length;
  const bioCounter =
    bioCharactersLeft >= 0
      ? `${bioCharactersLeft} ${bioCharactersLeft === 1 ? "character" : "characters"} left.`
      : `${-bioCharactersLeft} ${bioCharactersLeft === -1 ? "character" : "characters"} too many.`;

  return (
    <>
      <div className={styles.band}>
        <div className={styles.bandInner}>
          <Eyebrow>Profiles</Eyebrow>
          <h1 className={styles.title}>Create your profile</h1>
        </div>
      </div>

      <div className={styles.body}>
        {generalMessage && (
          <p role="alert" className={styles.alert}>
            {generalMessage}
          </p>
        )}

        {saved ? (
          <section role="status" className={styles.savedCard}>
            <h2 className={styles.savedTitle}>Your profile is saved</h2>
            <Link to="/profile" className={styles.savedLink}>
              See my profile
            </Link>
          </section>
        ) : (
          <div className={styles.columns}>
            <form
              className={styles.card}
              noValidate
              onSubmit={(event) => {
                event.preventDefault();
                void submit();
              }}
            >
              <FormField
                id="firstName"
                label="First name"
                type="text"
                autoComplete="given-name"
                value={firstName}
                error={fieldErrors.firstName}
                ref={firstNameRef}
                onChange={(event) => setFirstName(event.target.value)}
              />

              <FormField
                as="select"
                id="ageBand"
                label="Age band"
                value={ageBand}
                error={fieldErrors.ageBand}
                ref={ageBandRef}
                onChange={(event) =>
                  setAgeBand(event.target.value as AgeBand | "")
                }
              >
                <option value="" disabled>
                  Choose an age band
                </option>
                {AGE_BANDS.map((band) => (
                  <option key={band} value={band}>
                    {AGE_BAND_LABELS[band]}
                  </option>
                ))}
              </FormField>

              <FormField
                id="region"
                label="Region"
                type="text"
                hint="Your town or area, for example Amstelveen. Not your street, house number or postcode."
                value={region}
                error={fieldErrors.region}
                ref={regionRef}
                onChange={(event) => setRegion(event.target.value)}
              />

              <FormField
                as="textarea"
                id="bio"
                label="About you"
                hint={`Do not write your school, address or phone number here. ${bioCounter}`}
                value={bio}
                error={fieldErrors.bio}
                ref={bioRef}
                onChange={(event) => setBio(event.target.value)}
              />

              <button type="submit" className={styles.submit} disabled={saving}>
                {saving ? "Saving..." : "Save"}
              </button>
            </form>

            <aside
              className={`${styles.aside} ${generalMessage ? styles.asideAlert : ""}`}
              aria-label="What Connect asks"
            >
              <span className={styles.asideIcon}>
                <img src={shieldIcon} alt="" />
              </span>
              <p className={styles.asidePurpose}>
                Tell Connect who you are: your first name, your age band, your
                region and, if you want, a short line about yourself.
              </p>
              <ul className={styles.checks}>
                <li>
                  Never asks for your exact age, your birth date, your school or
                  your street address
                </li>
              </ul>
            </aside>
          </div>
        )}
      </div>
    </>
  );
}
