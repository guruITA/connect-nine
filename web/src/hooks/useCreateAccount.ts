import { useState } from "react";
import type { CreateAccountResult } from "../api/accounts";
import { createAccount } from "../api/accounts";

export function useCreateAccount() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);
  const [result, setResult] = useState<CreateAccountResult | undefined>(undefined);

  const submit = async () => {
    setResult(undefined);
    setBusy(true);
    const response = await createAccount(email, password);
    setBusy(false);
    if (response.kind === "made") setPassword("");
    setResult(response);
  };

  return { email, password, busy, result, setEmail, setPassword, submit };
}
