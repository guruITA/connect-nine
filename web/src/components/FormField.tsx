import type { ChangeEventHandler, ReactNode, Ref } from "react";
import styles from "./FormField.module.css";

// One field of a form (frontend rule 12): the label, a hint when there is one, the control and
// its message, tied together with aria-describedby and aria-invalid.
type CommonProps = {
  id: string;
  label: string;
  hint?: ReactNode;
  error?: string;
};

type InputProps = CommonProps & {
  as?: "input";
  type: "email" | "password" | "text";
  autoComplete?: string;
  value: string;
  onChange: ChangeEventHandler<HTMLInputElement>;
  ref?: Ref<HTMLInputElement>;
};

type SelectProps = CommonProps & {
  as: "select";
  value: string;
  onChange: ChangeEventHandler<HTMLSelectElement>;
  children: ReactNode;
  ref?: Ref<HTMLSelectElement>;
};

type TextAreaProps = CommonProps & {
  as: "textarea";
  value: string;
  onChange: ChangeEventHandler<HTMLTextAreaElement>;
  ref?: Ref<HTMLTextAreaElement>;
};

type FormFieldProps = InputProps | SelectProps | TextAreaProps;

export default function FormField({ ref, ...props }: FormFieldProps) {
  const { id, label, hint, error } = props;
  const hintId = `${id}-hint`;
  const errorId = `${id}-error`;
  const describedBy =
    [hint ? hintId : undefined, error ? errorId : undefined]
      .filter(Boolean)
      .join(" ") || undefined;
  const shared = {
    id,
    name: id,
    value: props.value,
    "aria-describedby": describedBy,
    "aria-invalid": error ? true : undefined,
  };

  return (
    <div className={styles.field}>
      <label htmlFor={id}>{label}</label>
      {hint && (
        <p className={styles.hint} id={hintId}>
          {hint}
        </p>
      )}
      {props.as === "select" ? (
        <select
          {...shared}
          ref={ref as Ref<HTMLSelectElement>}
          onChange={props.onChange}
        >
          {props.children}
        </select>
      ) : props.as === "textarea" ? (
        <textarea
          {...shared}
          ref={ref as Ref<HTMLTextAreaElement>}
          onChange={props.onChange}
        />
      ) : (
        <input
          {...shared}
          ref={ref as Ref<HTMLInputElement>}
          type={props.type}
          autoComplete={props.autoComplete}
          onChange={props.onChange}
        />
      )}
      {error && (
        <p className={styles.error} id={errorId}>
          {error}
        </p>
      )}
    </div>
  );
}
