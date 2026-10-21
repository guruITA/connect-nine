import type { CreateAccountResult } from "../../api/accounts";
import FormField from "../FormField";
import styles from "./CreateAccountForm.module.css";

type CreateAccountFormProps = {
  email: string;
  password: string;
  busy: boolean;
  result?: CreateAccountResult;
  onEmailChange: (value: string) => void;
  onPasswordChange: (value: string) => void;
  onSubmit: () => void;
};

export default function CreateAccountForm({
  email,
  password,
  busy,
  result,
  onEmailChange,
  onPasswordChange,
  onSubmit,
}: CreateAccountFormProps) {
  const emailError =
    result?.kind === "invalid"
      ? result.fields.email
      : result?.kind === "taken"
        ? "This email address already has an account"
        : undefined;
  const passwordError =
    result?.kind === "invalid" ? result.fields.password : undefined;

  return (
    <form
      className={styles.form}
      noValidate
      aria-labelledby="create-account-title"
      onSubmit={(event) => {
        event.preventDefault();
        void onSubmit();
      }}
    >
      <div className={styles.fields}>
        <FormField
          id="email"
          label="Email"
          type="email"
          autoComplete="email"
          value={email}
          error={emailError}
          onChange={(event) => onEmailChange(event.target.value)}
        />
        <FormField
          id="password"
          label="Password"
          type="password"
          autoComplete="new-password"
          value={password}
          error={passwordError}
          onChange={(event) => onPasswordChange(event.target.value)}
        />
      </div>
      <button type="submit" disabled={busy}>
        {busy ? "Making your account..." : "Make account"}
      </button>
    </form>
  );
}
