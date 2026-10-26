import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { getCurrentMemberProfile } from "../api/memberProfiles";
import type { CurrentMemberProfile } from "../api/memberProfiles";
import Eyebrow from "../components/home/Eyebrow";
import ProfileDetails from "../components/ProfileDetails";
import styles from "./MyProfile.module.css";

type PageState =
  | { status: "loading" }
  | { status: "loaded"; profile: CurrentMemberProfile }
  | { status: "notLoggedIn" }
  | { status: "notReachable" };

// The page My profile, as the Figma frame "My profile" draws it: a band with the title,
// the profile in a card below it.
export default function MyProfile() {
  const navigate = useNavigate();
  const [state, setState] = useState<PageState>({ status: "loading" });

  const load = useCallback(() => {
    getCurrentMemberProfile().then((result) => {
      if (result.outcome === "loaded") {
        setState({ status: "loaded", profile: result.profile });
        return;
      }

      if (result.outcome === "noProfile") {
        navigate("/profile/new", { replace: true });
        return;
      }

      if (result.outcome === "notLoggedIn") {
        setState({ status: "notLoggedIn" });
        return;
      }

      setState({ status: "notReachable" });
    });
  }, [navigate]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    document.title = "Connect, My profile";
  }, []);

  return (
    <>
      <header className={styles.band}>
        <div className={styles.bandInner}>
          <Eyebrow>Profiles</Eyebrow>
          <h1 className={styles.title}>
            {state.status === "loaded" ? state.profile.firstName : "My profile"}
          </h1>
        </div>
      </header>

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
            <button
              type="button"
              className={styles.retry}
              onClick={() => {
                setState({ status: "loading" });
                load();
              }}
            >
              Try again
            </button>
          </div>
        )}

        {state.status === "loaded" && (
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
        )}
      </div>
    </>
  );
}
