import ButtonLink from "./ButtonLink";
import styles from "./LoggedOutMessage.module.css";

type LoggedOutMessageProps = {
  path: string;
};

// Shown instead of a member page to a visitor who is not logged in, so no name, age band,
// region or short line of a member ever reaches the browser (#31). Only a path that starts with
// /members/ goes into next, so the link can never send a visitor to another site.
export default function LoggedOutMessage({ path }: LoggedOutMessageProps) {
  const next = path.startsWith("/members/")
    ? `?next=${encodeURIComponent(path)}`
    : "";

  return (
    <div className={styles.card}>
      <h2 className={styles.title}>Log in to see members</h2>
      <p className={styles.text}>
        Member pages on Connect are only for people who are logged in, to keep
        the young people on Connect safe.
      </p>
      <ButtonLink to={`/login${next}`}>Log in</ButtonLink>
    </div>
  );
}
