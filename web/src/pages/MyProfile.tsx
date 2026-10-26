import { useEffect } from "react";
import Eyebrow from "../components/home/Eyebrow";
import ParentConsentBanner from "../components/profile/ParentConsentBanner";
import ProfileDetails from "../components/profile/ProfileDetails";
import TagInput from "../components/TagInput";
import { useMyProfile } from "../hooks/useMyProfile";
import styles from "./MyProfile.module.css";

// The page My profile, as the Figma frame "My profile" draws it: a band with the title,
// the profile and Skills and interests in cards below it.
export default function MyProfile() {
  const {
    state,
    retry,
    suggestedTags,
    tagValue,
    tagFieldError,
    tagBusyName,
    tagStatusMessage,
    tagAlertMessage,
    changeTagValue,
    addTag,
  } = useMyProfile();

  useEffect(() => {
    document.title = "Connect, My profile";
  }, []);

  return (
    <>
      {state.status === "loaded" &&
        state.profile.parentConsent === "pending" && <ParentConsentBanner />}

      <div className={styles.band}>
        <div className={styles.bandInner}>
          <Eyebrow>Profiles</Eyebrow>
          <h1 className={styles.title}>
            {state.status === "loaded" ? state.profile.firstName : "My profile"}
          </h1>
        </div>
      </div>

      <div className={styles.body}>
        {state.status === "loading" && (
          <div className={styles.loadingCard}>
            <p className={styles.stateText}>Loading your profile...</p>
          </div>
        )}

        {state.status === "notLoggedIn" && (
          <p role="alert" className={styles.alert}>
            Log in first to see this page
          </p>
        )}

        {state.status === "notReachable" && (
          <div className={styles.stateCard}>
            <p role="alert" className={styles.alert}>
              Connect is not reachable right now, try again later
            </p>
            <button type="button" className={styles.retry} onClick={retry}>
              Try again
            </button>
          </div>
        )}

        {state.status === "loaded" && (
          <>
            <section className={styles.card}>
              <div className={styles.profileHeader}>
                <span className={styles.avatar} aria-hidden="true">
                  {state.profile.firstName.charAt(0).toUpperCase()}
                </span>
                <ProfileDetails
                  ageBand={state.profile.ageBand}
                  region={state.profile.region}
                  bio={state.profile.bio}
                />
              </div>
            </section>

            <section className={styles.card}>
              <h2 className={styles.sectionTitle}>Skills and interests</h2>

              {tagStatusMessage && (
                <p role="status" className={styles.status}>
                  {tagStatusMessage}
                </p>
              )}
              {tagAlertMessage && (
                <p role="alert" className={styles.alert}>
                  {tagAlertMessage}
                </p>
              )}

              {suggestedTags === null ? (
                <p className={styles.stateText}>Loading your tags...</p>
              ) : (
                <TagInput
                  chosenTags={state.profile.tags}
                  suggestedTags={suggestedTags}
                  value={tagValue}
                  onChange={changeTagValue}
                  onAdd={addTag}
                  busyName={tagBusyName}
                  fieldError={tagFieldError}
                />
              )}
            </section>
          </>
        )}
      </div>
    </>
  );
}
