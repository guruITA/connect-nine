import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { addMemberProfileTag } from "../api/memberProfileTags";
import { getCurrentMemberProfile } from "../api/memberProfiles";
import type { CurrentMemberProfile } from "../api/memberProfiles";
import { getSuggestedTags } from "../api/tags";
import type { Tag } from "../api/tags";

export type MyProfileState =
  | { status: "loading" }
  | { status: "loaded"; profile: CurrentMemberProfile }
  | { status: "notLoggedIn" }
  | { status: "notReachable" };

function addSortedByName(tags: Tag[], tag: Tag): Tag[] {
  if (tags.some((existing) => existing.id === tag.id)) {
    return tags;
  }
  return [...tags, tag].sort((a, b) => a.name.localeCompare(b.name));
}

// The state of the page My profile and its calls to the server, so the page only shows what it
// gets (frontend rule 13). A member without a profile goes on to Create your profile.
export function useMyProfile() {
  const navigate = useNavigate();
  const [state, setState] = useState<MyProfileState>({ status: "loading" });
  const [suggestedTags, setSuggestedTags] = useState<Tag[] | null>(null);
  const [tagValue, setTagValue] = useState("");
  const [tagFieldError, setTagFieldError] = useState<string | undefined>(
    undefined,
  );
  const [tagBusyName, setTagBusyName] = useState<string | null>(null);
  const [tagStatusMessage, setTagStatusMessage] = useState<string | null>(null);
  const [tagAlertMessage, setTagAlertMessage] = useState<string | null>(null);

  const load = useCallback(() => {
    getCurrentMemberProfile().then((result) => {
      if (result.outcome === "loaded") {
        setState({ status: "loaded", profile: result.profile });
        return;
      }

      if (result.outcome === "noProfile") {
        navigate("/profile/new", { replace: true });
        return;
      }

      if (result.outcome === "notLoggedIn") {
        setState({ status: "notLoggedIn" });
        return;
      }

      setState({ status: "notReachable" });
    });
  }, [navigate]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    getSuggestedTags().then(setSuggestedTags);
  }, []);

  const retry = () => {
    setState({ status: "loading" });
    load();
  };

  const changeTagValue = (value: string) => {
    setTagValue(value);
    setTagFieldError(undefined);
  };

  const addTag = async (name: string) => {
    setTagBusyName(name);
    setTagAlertMessage(null);
    setTagStatusMessage(null);

    const result = await addMemberProfileTag(name);
    setTagBusyName(null);

    if (result.outcome === "added") {
      setState((previous) =>
        previous.status === "loaded"
          ? {
              ...previous,
              profile: {
                ...previous.profile,
                tags: addSortedByName(previous.profile.tags, result.tag),
              },
            }
          : previous,
      );
      setTagValue("");
      setTagFieldError(undefined);
      setTagStatusMessage("Saved");
      return;
    }

    if (result.outcome === "fieldError") {
      setTagFieldError(result.message);
      return;
    }

    if (result.outcome === "notLoggedIn") {
      setTagAlertMessage("Log in first to see this page");
      return;
    }

    if (result.outcome === "noProfile") {
      navigate("/profile/new", { replace: true });
      return;
    }

    setTagAlertMessage(
      "Connect is not reachable right now, your tags are kept",
    );
  };

  return {
    state,
    retry,
    suggestedTags,
    tagValue,
    tagFieldError,
    tagBusyName,
    tagStatusMessage,
    tagAlertMessage,
    changeTagValue,
    addTag,
  };
}
