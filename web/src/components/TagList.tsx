import styles from "./TagList.module.css";

type TagListProps = {
  tags: string[];
  heading?: string;
};

// The tags of a profile as a plain read only list, with no button, link or field, for the
// page of another member (#72). TagInput is never reused here. The heading is only shown
// when given, so the page can render its own h2 instead without two labels.
export default function TagList({ tags, heading }: TagListProps) {
  return (
    <div className={styles.tagList}>
      {heading && <h2 className={styles.heading}>{heading}</h2>}
      <ul className={styles.list}>
        {tags.map((tag) => (
          <li key={tag} className={styles.tag}>
            {tag}
          </li>
        ))}
      </ul>
    </div>
  );
}
