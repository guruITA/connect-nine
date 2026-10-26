import { useCallback, useEffect, useState } from "react";
import { getMemberProfile } from "../api/memberProfiles";
import type { PublicMemberProfile } from "../api/memberProfiles";

export type MemberProfileState =
  | { status: "loading" }
  | { status: "loaded"; profile: PublicMemberProfile }
  | { status: "notFound" }
  | { status: "notLoggedIn" }
  | { status: "notReachable" };

// The state of the page of another member and its call to the server, so the page only shows
// what it gets (frontend rule 13).
export function useMemberProfile(id: string | undefined) {
  const [state, setState] = useState<MemberProfileState>({ status: "loading" });

  const load = useCallback(() => {
    if (!id) {
      return;
    }
    getMemberProfile(id).then((result) => {
      if (result.outcome === "loaded") {
        setState({ status: "loaded", profile: result.profile });
        return;
      }

      if (result.outcome === "notFound") {
        setState({ status: "notFound" });
        return;
      }

      if (result.outcome === "notLoggedIn") {
        setState({ status: "notLoggedIn" });
        return;
      }

      setState({ status: "notReachable" });
    });
  }, [id]);

  useEffect(() => {
    load();
  }, [load]);

  const retry = () => {
    setState({ status: "loading" });
    load();
  };

  return { state, retry };
}
