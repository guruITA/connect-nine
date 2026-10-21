import CreateAccountForm from "../components/account/CreateAccountForm";
import { useCreateAccount } from "../hooks/useCreateAccount";
import styles from "./CreateAccount.module.css";

const NOT_REACHABLE = "Connect is not reachable right now, try again later";

export default function CreateAccount() {
  const { email, password, busy, result, setEmail, setPassword, submit } =
    useCreateAccount();

  if (result?.kind === "made") {
    return (
      <section className={styles.page} aria-labelledby="create-account-title">
        <div className={styles.content}>
          <p className={styles.eyebrow}>Account</p>
          <h1 id="create-account-title">Make an account</h1>
          <div className={styles.status} role="status">
            Your account is made
          </div>
        </div>
      </section>
    );
  }

  return (
    <section className={styles.page} aria-labelledby="create-account-title">
      <div className={styles.content}>
        <p className={styles.eyebrow}>Account</p>
        <h1 id="create-account-title">Make an account</h1>
        <p className={styles.intro}>
          It takes two minutes. Your profile comes right after.
        </p>
        {result?.kind === "not reachable" && (
          <div className={styles.alert} role="alert">
            {NOT_REACHABLE}
          </div>
        )}
        <CreateAccountForm
          email={email}
          password={password}
          busy={busy}
          result={result}
          onEmailChange={setEmail}
          onPasswordChange={setPassword}
          onSubmit={submit}
        />
      </div>
    </section>
  );
}
