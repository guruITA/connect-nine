import type { ReactNode } from "react";
import hatchLight from "../../assets/home/hatch-light.svg";
import hatch from "../../assets/home/hatch.svg";
import styles from "./Eyebrow.module.css";

type EyebrowTone = "onLight" | "onNavy" | "onGreen";

type EyebrowProps = {
  tone?: EyebrowTone;
  children: ReactNode;
};

// The small label with a hatched line above a heading on the home page.
export default function Eyebrow({ tone = "onLight", children }: EyebrowProps) {
  return (
    <p className={`${styles.eyebrow} ${styles[tone]}`}>
      <img
        src={tone === "onGreen" ? hatchLight : hatch}
        alt=""
        className={styles.hatch}
      />
      {children}
    </p>
  );
}
