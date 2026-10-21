import type { ChangeEventHandler } from "react";
import styles from "./FormField.module.css";

type FormFieldProps = {
  id: string;
  label: string;
  type: "email" | "password" | "text";
  autoComplete: string;
  value: string;
  error?: string;
  onChange: ChangeEventHandler<HTMLInputElement>;
};

export default function FormField({
  id,
  label,
  type,
  autoComplete,
  value,
  error,
  onChange,
}: FormFieldProps) {
  const errorId = `${id}-error`;

  return (
    <div className={styles.field}>
      <label htmlFor={id}>{label}</label>
      <input
        id={id}
        name={id}
        type={type}
        autoComplete={autoComplete}
        value={value}
        aria-describedby={error ? errorId : undefined}
        aria-invalid={error ? true : undefined}
        onChange={onChange}
      />
      {error && (
        <p className={styles.error} id={errorId}>
          {error}
        </p>
      )}
    </div>
  );
}
