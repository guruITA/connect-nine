import type { ReactNode } from "react";
import { Link } from "react-router";
import styles from "./ButtonLink.module.css";

type ButtonLinkVariant =
  "primary" | "light" | "outline" | "outlineLight" | "lightOnGreen";

type ButtonLinkProps = {
  to: string;
  variant?: ButtonLinkVariant;
  wide?: boolean;
  children: ReactNode;
};

// A link drawn as a pill button. It goes somewhere, so it is a link and not a button.
// An address that starts with # scrolls to a part of the same page.
export default function ButtonLink({
  to,
  variant = "primary",
  wide = false,
  children,
}: ButtonLinkProps) {
  const className = [
    styles.buttonLink,
    styles[variant],
    wide ? styles.wide : "",
  ].join(" ");

  if (to.startsWith("#")) {
    return (
      <a href={to} className={className}>
        {children}
      </a>
    );
  }

  return (
    <Link to={to} className={className}>
      {children}
    </Link>
  );
}
