import { useRef, useState } from "react";
import type { FormEvent } from "react";
import { Link } from "react-router";
import { AGE_BAND_LABELS, createMemberProfile } from "../api/memberProfiles";
import type { AgeBand } from "../api/memberProfiles";
import shieldIcon from "../assets/icons/shield.svg";
import Eyebrow from "../components/home/Eyebrow";
import styles from "./CreateProfile.module.css";

const AGE_BANDS: AgeBand[] = ["Adolescent", "YoungAdult", "Adult"];
const BIO_MAX_LENGTH = 280;

type FieldName = "firstName" | "ageBand" | "region" | "bio";
type FieldErrors = Partial<Record<FieldName, string>>;

// The order the focus looks for the first wrong field in, top to bottom.
const FIELD_ORDER: FieldName[] = ["firstName", "ageBand", "region", "bio"];

function describedBy(...ids: (string | false | undefined)[]) {
  const present = ids.filter(Boolean).join(" ");
  return present || undefined;
}

// The page Create your profile, as the Figma frame "Create your profile" draws it: a band
// with the title, the form in a card and a panel beside it that says what Connect never asks.
export default function CreateProfile() {
  const [firstName, setFirstName] = useState("");
  const [ageBand, setAgeBand] = useState<AgeBand | "">("");
  const [region, setRegion] = useState("");
  const [bio, setBio] = useState("");
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [generalMessage, setGeneralMessage] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [saved, setSaved] = useState(false);

  const firstNameRef = useRef<HTMLInputElement>(null);
  const ageBandRef = useRef<HTMLSelectElement>(null);
  const regionRef = useRef<HTMLInputElement>(null);
  const bioRef = useRef<HTMLTextAreaElement>(null);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setFieldErrors({});
    setGeneralMessage(null);
    setSaving(true);

    const result = await createMemberProfile({
      firstName,
      ageBand,
      region,
      bio,
    });
    setSaving(false);

    if (result.outcome === "saved") {
      setSaved(true);
      return;
    }

    if (result.outcome === "fieldErrors") {
      setFieldErrors(result.fieldErrors);
      const fieldElements = {
        firstName: firstNameRef.current,
        ageBand: ageBandRef.current,
        region: regionRef.current,
        bio: bioRef.current,
      };
      const firstWrong = FIELD_ORDER.find((field) => result.fieldErrors[field]);
      if (firstWrong) {
        fieldElements[firstWrong]?.focus();
      }
      return;
    }

    if (result.outcome === "alreadyHasProfile") {
      setGeneralMessage(result.message);
      return;
    }

    if (result.outcome === "notLoggedIn") {
      setGeneralMessage("Log in first to save your profile");
      return;
    }

    setGeneralMessage("Connect is not reachable right now, your text is kept");
  }

  const bioCharactersLeft = BIO_MAX_LENGTH - bio.length;
  const bioCounter =
    bioCharactersLeft >= 0
      ? `${bioCharactersLeft} ${bioCharactersLeft === 1 ? "character" : "characters"} left.`
      : `${-bioCharactersLeft} ${bioCharactersLeft === -1 ? "character" : "characters"} too many.`;

  return (
    <>
      <header className={styles.band}>
        <div className={styles.bandInner}>
          <Eyebrow>Profiles</Eyebrow>
          <h1 className={styles.title}>Create your profile</h1>
        </div>
      </header>

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
            <form className={styles.card} onSubmit={handleSubmit} noValidate>
              <div className={styles.field}>
                <label htmlFor="firstName" className={styles.label}>
                  First name
                </label>
                <input
                  id="firstName"
                  ref={firstNameRef}
                  type="text"
                  autoComplete="given-name"
                  value={firstName}
                  onChange={(event) => setFirstName(event.target.value)}
                  aria-describedby={describedBy(
                    fieldErrors.firstName && "firstNameError",
                  )}
                  aria-invalid={fieldErrors.firstName ? true : undefined}
                  className={styles.input}
                />
                {fieldErrors.firstName && (
                  <p id="firstNameError" className={styles.fieldError}>
                    {fieldErrors.firstName}
                  </p>
                )}
              </div>

              <div className={styles.field}>
                <label htmlFor="ageBand" className={styles.label}>
                  Age band
                </label>
                <select
                  id="ageBand"
                  ref={ageBandRef}
                  value={ageBand}
                  onChange={(event) =>
                    setAgeBand(event.target.value as AgeBand | "")
                  }
                  aria-describedby={describedBy(
                    fieldErrors.ageBand && "ageBandError",
                  )}
                  aria-invalid={fieldErrors.ageBand ? true : undefined}
                  className={styles.input}
                >
                  <option value="" disabled>
                    Choose an age band
                  </option>
                  {AGE_BANDS.map((band) => (
                    <option key={band} value={band}>
                      {AGE_BAND_LABELS[band]}
                    </option>
                  ))}
                </select>
                {fieldErrors.ageBand && (
                  <p id="ageBandError" className={styles.fieldError}>
                    {fieldErrors.ageBand}
                  </p>
                )}
              </div>

              <div className={styles.field}>
                <label htmlFor="region" className={styles.label}>
                  Region
                </label>
                <p id="regionHint" className={styles.hint}>
                  Your town or area, for example Amstelveen. Not your street,
                  house number or postcode.
                </p>
                <input
                  id="region"
                  ref={regionRef}
                  type="text"
                  value={region}
                  onChange={(event) => setRegion(event.target.value)}
                  aria-describedby={describedBy(
                    "regionHint",
                    fieldErrors.region && "regionError",
                  )}
                  aria-invalid={fieldErrors.region ? true : undefined}
                  className={styles.input}
                />
                {fieldErrors.region && (
                  <p id="regionError" className={styles.fieldError}>
                    {fieldErrors.region}
                  </p>
                )}
              </div>

              <div className={styles.field}>
                <label htmlFor="bio" className={styles.label}>
                  About you
                </label>
                <p id="bioHint" className={styles.hint}>
                  Do not write your school, address or phone number here.{" "}
                  {bioCounter}
                </p>
                <textarea
                  id="bio"
                  ref={bioRef}
                  value={bio}
                  onChange={(event) => setBio(event.target.value)}
                  aria-describedby={describedBy(
                    "bioHint",
                    fieldErrors.bio && "bioError",
                  )}
                  aria-invalid={fieldErrors.bio ? true : undefined}
                  className={styles.textarea}
                />
                {fieldErrors.bio && (
                  <p id="bioError" className={styles.fieldError}>
                    {fieldErrors.bio}
                  </p>
                )}
              </div>

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
