import type { ChangeEvent, FormEvent } from "react";
import type { Tag } from "../api/tags";
import styles from "./TagInput.module.css";

type TagInputProps = {
  chosenTags: Tag[];
  suggestedTags: Tag[];
  value: string;
  onChange: (value: string) => void;
  onAdd: (name: string) => void;
  busyName: string | null;
  fieldError?: string;
};

function sameName(a: string, b: string): boolean {
  return a.trim().toLowerCase() === b.trim().toLowerCase();
}

// Chosen and suggested tags as real buttons, and a field to type a tag of your own. Every
// tap or Add saves at once through onAdd, with no fetch and no address in this component.
export default function TagInput({
  chosenTags,
  suggestedTags,
  value,
  onChange,
  onAdd,
  busyName,
  fieldError,
}: TagInputProps) {
  const fieldBusy = busyName !== null && sameName(busyName, value);

  // An empty field goes to the server too, which answers "Fill in a tag", so the message of an
  // empty tag comes from the same place as every other message about a tag.
  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    onAdd(value);
  }

  return (
    <div className={styles.tagInput}>
      <div className={styles.block}>
        <h3 className={styles.blockTitle}>Your tags</h3>
        {chosenTags.length === 0 ? (
          <p className={styles.empty}>You have no tags yet</p>
        ) : (
          <ul className={styles.chosenList}>
            {chosenTags.map((tag) => (
              <li key={tag.id} className={styles.chosenTag}>
                {tag.name}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className={styles.block}>
        <h3 className={styles.blockTitle}>Suggested tags</h3>
        <ul className={styles.suggestedList}>
          {suggestedTags.map((tag) => {
            const isChosen = chosenTags.some((chosen) =>
              sameName(chosen.name, tag.name),
            );
            const isBusy = busyName !== null && sameName(busyName, tag.name);
            return (
              <li key={tag.id}>
                <button
                  type="button"
                  className={styles.suggestedTag}
                  aria-pressed={isChosen}
                  disabled={isBusy}
                  onClick={() => {
                    if (isChosen) {
                      return;
                    }
                    onAdd(tag.name);
                  }}
                >
                  {isBusy ? "Saving..." : tag.name}
                </button>
              </li>
            );
          })}
        </ul>
      </div>

      <form className={styles.addForm} onSubmit={handleSubmit}>
        <div className={styles.field}>
          <label htmlFor="tagInputValue" className={styles.label}>
            Add your own
          </label>
          <input
            id="tagInputValue"
            type="text"
            placeholder="Type a tag"
            value={value}
            onChange={(event: ChangeEvent<HTMLInputElement>) =>
              onChange(event.target.value)
            }
            aria-describedby={fieldError ? "tagInputError" : undefined}
            aria-invalid={fieldError ? true : undefined}
            disabled={fieldBusy}
            className={styles.input}
          />
          {fieldError && (
            <p id="tagInputError" className={styles.fieldError}>
              {fieldError}
            </p>
          )}
        </div>
        <button type="submit" className={styles.addButton} disabled={fieldBusy}>
          {fieldBusy ? "Saving..." : "Add"}
        </button>
      </form>

      <p className={styles.hint}>
        Skills and interests only. Nothing about your health, faith or other
        private matters.
      </p>
    </div>
  );
}
