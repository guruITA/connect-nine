import { useEffect } from "react";
import { Link, useParams } from "react-router";
import Eyebrow from "../components/home/Eyebrow";
import ProfileDetails from "../components/profile/ProfileDetails";
import { useMemberProfile } from "../hooks/useMemberProfile";
import styles from "./MemberProfile.module.css";

// The page of another member, as the Figma frame "Member profile" draws it: a band with
// the title, the profile in a card below it. Nothing on the page can change the profile.
export default function MemberProfile() {
  const { id } = useParams<{ id: string }>();
  const { state, retry } = useMemberProfile(id);

  useEffect(() => {
    document.title = "Connect, Member profile";
  }, []);

  return (
    <>
      <div className={styles.band}>
        <div className={styles.bandInner}>
          <Eyebrow>Profiles</Eyebrow>
          <h1 className={styles.title}>
            {state.status === "loaded" ? state.profile.firstName : "Member"}
          </h1>
        </div>
      </div>

      <div className={styles.body}>
        {state.status === "loading" && (
          <div className={styles.loadingCard}>
            <p className={styles.stateText}>Loading...</p>
          </div>
        )}

        {state.status === "notFound" && (
          <div className={styles.stateCard}>
            <h2 className={styles.stateTitle}>This member does not exist</h2>
            <Link to="/account" className={styles.goLink}>
              Go to My account
            </Link>
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
          <div className={styles.columns}>
            <div className={styles.main}>
              {state.profile.bio && (
                <section className={styles.card}>
                  <h2 className={styles.sectionTitle}>About you</h2>
                  <p className={styles.bio}>{state.profile.bio}</p>
                </section>
              )}
            </div>
            <div className={styles.side}>
              <section className={styles.card}>
                <h2 className={styles.sectionTitle}>Profile details</h2>
                <ProfileDetails
                  ageBand={state.profile.ageBand}
                  region={state.profile.region}
                  showBio={false}
                />
              </section>
            </div>
          </div>
        )}
      </div>
    </>
  );
}
