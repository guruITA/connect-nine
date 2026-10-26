import { useRef, useState } from "react";
import type { FormEvent } from "react";
import { AGE_BAND_LABELS, createMemberProfile } from "../api/memberProfiles";
import type { AgeBand } from "../api/memberProfiles";
import styles from "./CreateProfile.module.css";

const AGE_BANDS: AgeBand[] = ["From15To17", "From18To21", "From22To30"];
const BIO_MAX_LENGTH = 280;

type FieldErrors = Partial<
  Record<"firstName" | "ageBand" | "region" | "bio", string>
>;

type GeneralMessage = {
  kind: "alreadyHasProfile" | "notLoggedIn" | "notReachable";
  text: string;
};

// The page Create your profile, as the Figma frame "Create your profile" draws it.
export default function CreateProfile() {
  const [firstName, setFirstName] = useState("");
  const [ageBand, setAgeBand] = useState<AgeBand | "">("");
  const [region, setRegion] = useState("");
  const [bio, setBio] = useState("");
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [generalMessage, setGeneralMessage] = useState<GeneralMessage | null>(
    null,
  );
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
      if (result.fieldErrors.firstName) {
        firstNameRef.current?.focus();
      } else if (result.fieldErrors.ageBand) {
        ageBandRef.current?.focus();
      } else if (result.fieldErrors.region) {
        regionRef.current?.focus();
      } else if (result.fieldErrors.bio) {
        bioRef.current?.focus();
      }
      return;
    }

    if (result.outcome === "alreadyHasProfile") {
      setGeneralMessage({
        kind: "alreadyHasProfile",
        text: "You already have a profile",
      });
      return;
    }

    if (result.outcome === "notLoggedIn") {
      setGeneralMessage({
        kind: "notLoggedIn",
        text: "Log in first to save your profile",
      });
      return;
    }

    setGeneralMessage({
      kind: "notReachable",
      text: "Connect is not reachable right now, your text is kept",
    });
  }

  if (saved) {
    return (
      <div className={styles.inner}>
        <h1 className={styles.title}>Create your profile</h1>
        <p role="status" className={styles.statusPanel}>
          Your profile is saved
        </p>
      </div>
    );
  }

  const bioCharactersLeft = BIO_MAX_LENGTH - bio.length;

  return (
    <div className={styles.inner}>
      <h1 className={styles.title}>Create your profile</h1>
      <p className={styles.lead}>
        Tell Connect who you are: your first name, your age band, your region
        and, if you want, a short line about yourself.
      </p>

      {generalMessage && (
        <p
          role={
            generalMessage.kind === "alreadyHasProfile" ? undefined : "alert"
          }
          className={styles.messagePanel}
        >
          {generalMessage.text}
        </p>
      )}

      <form className={styles.form} onSubmit={handleSubmit} noValidate>
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
            aria-describedby={
              fieldErrors.firstName ? "firstNameError" : undefined
            }
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
            onChange={(event) => setAgeBand(event.target.value as AgeBand | "")}
            aria-describedby={fieldErrors.ageBand ? "ageBandError" : undefined}
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
          <input
            id="region"
            ref={regionRef}
            type="text"
            value={region}
            onChange={(event) => setRegion(event.target.value)}
            aria-describedby={fieldErrors.region ? "regionError" : "regionHint"}
            className={styles.input}
          />
          {fieldErrors.region ? (
            <p id="regionError" className={styles.fieldError}>
              {fieldErrors.region}
            </p>
          ) : (
            <p id="regionHint" className={styles.hint}>
              Your town or area, for example Amstelveen.
            </p>
          )}
        </div>

        <div className={styles.field}>
          <label htmlFor="bio" className={styles.label}>
            About you
          </label>
          <textarea
            id="bio"
            ref={bioRef}
            value={bio}
            onChange={(event) => setBio(event.target.value)}
            aria-describedby={fieldErrors.bio ? "bioError" : "bioHint"}
            className={styles.textarea}
          />
          {fieldErrors.bio ? (
            <p id="bioError" className={styles.fieldError}>
              {fieldErrors.bio}
            </p>
          ) : (
            <p id="bioHint" className={styles.hint}>
              Do not write your school, address or phone number here.{" "}
              {bioCharactersLeft === 1
                ? "1 character left."
                : `${bioCharactersLeft} characters left.`}
            </p>
          )}
        </div>

        <button type="submit" className={styles.submit} disabled={saving}>
          {saving ? "Saving..." : "Save"}
        </button>
      </form>
    </div>
  );
}
