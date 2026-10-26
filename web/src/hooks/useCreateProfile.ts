import { useRef, useState } from "react";
import { createMemberProfile } from "../api/memberProfiles";
import type { AgeBand } from "../api/memberProfiles";

export type ProfileFieldName = "firstName" | "ageBand" | "region" | "bio";
export type ProfileFieldErrors = Partial<Record<ProfileFieldName, string>>;

// The order the focus looks for the first wrong field in, top to bottom.
const FIELD_ORDER: ProfileFieldName[] = [
  "firstName",
  "ageBand",
  "region",
  "bio",
];

// The state of the page Create your profile and its call to the server, so the page only
// shows what it gets (frontend rule 13), the way useCreateAccount does for Create account.
export function useCreateProfile() {
  const [firstName, setFirstName] = useState("");
  const [ageBand, setAgeBand] = useState<AgeBand | "">("");
  const [region, setRegion] = useState("");
  const [bio, setBio] = useState("");
  const [fieldErrors, setFieldErrors] = useState<ProfileFieldErrors>({});
  const [generalMessage, setGeneralMessage] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [saved, setSaved] = useState(false);

  const firstNameRef = useRef<HTMLInputElement>(null);
  const ageBandRef = useRef<HTMLSelectElement>(null);
  const regionRef = useRef<HTMLInputElement>(null);
  const bioRef = useRef<HTMLTextAreaElement>(null);

  const submit = async () => {
    setFieldErrors({});
    setGeneralMessage(null);
    setSaving(true);

    const result = await createMemberProfile({
      firstName,
      ageBand,
      region,
      bio,
    });
    setSaving(false);

    if (result.outcome === "saved") {
      setSaved(true);
      return;
    }

    if (result.outcome === "fieldErrors") {
      setFieldErrors(result.fieldErrors);
      const fieldElements = {
        firstName: firstNameRef.current,
        ageBand: ageBandRef.current,
        region: regionRef.current,
        bio: bioRef.current,
      };
      const firstWrong = FIELD_ORDER.find((field) => result.fieldErrors[field]);
      if (firstWrong) {
        fieldElements[firstWrong]?.focus();
      }
      return;
    }

    if (result.outcome === "alreadyHasProfile") {
      setGeneralMessage(result.message);
      return;
    }

    if (result.outcome === "notLoggedIn") {
      setGeneralMessage("Log in first to save your profile");
      return;
    }

    setGeneralMessage("Connect is not reachable right now, your text is kept");
  };

  return {
    firstName,
    ageBand,
    region,
    bio,
    fieldErrors,
    generalMessage,
    saving,
    saved,
    firstNameRef,
    ageBandRef,
    regionRef,
    bioRef,
    setFirstName,
    setAgeBand,
    setRegion,
    setBio,
    submit,
  };
}
