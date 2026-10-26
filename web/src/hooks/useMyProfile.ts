import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { getCurrentMemberProfile } from "../api/memberProfiles";
import type { CurrentMemberProfile } from "../api/memberProfiles";

export type MyProfileState =
  | { status: "loading" }
  | { status: "loaded"; profile: CurrentMemberProfile }
  | { status: "notLoggedIn" }
  | { status: "notReachable" };

// The state of the page My profile and its call to the server, so the page only shows what it
// gets (frontend rule 13). A member without a profile goes on to Create your profile.
export function useMyProfile() {
  const navigate = useNavigate();
  const [state, setState] = useState<MyProfileState>({ status: "loading" });

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

  const retry = () => {
    setState({ status: "loading" });
    load();
  };

  return { state, retry };
}
