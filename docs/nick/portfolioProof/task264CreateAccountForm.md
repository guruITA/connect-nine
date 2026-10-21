# Proof: create-account form (#264)

Task 3.6, board issue 03, GitLab work item [#264](https://gitlab.fdmci.hva.nl/studio/social-impact-apps/onderwijs/projects/2026-2027-semester-1/projects/project-9-wail-salutem-foundation-project-2/connect-nine/-/work_items/264): create the first account form for Connect. This proof records the frontend plan, implementation boundaries, validation evidence and remaining work. The merge request link will be added when the merge request exists.

Profile: [Full Stack Software Developer](https://boksa.hbo-ict-hva.nl/profiles/full-stack-developer/), level 3. Portflow candidates: OR1, OR3, OR4 and OR5.

## Result

The `/create-account` page provides the first browser form that calls `POST /api/accounts`. The implementation keeps the page responsible for presentation, the account API responsible for translating HTTP outcomes, and the shared request function responsible for performing requests.

The implemented scope is deliberately limited to Task 3.6:

- Email and Password fields only.
- Empty, busy, made, invalid, taken and not-reachable states.
- No social login, later password rules or confirmation-email flow.
- `Password again` is visible in the supplied broader design export, but it is not listed in Task 3.6. It is therefore not implemented here and should be discussed during review as a scope question.

## OR1 — plan, scope and design decisions

### Planned states

| State | What the person sees | Implementation evidence |
|---|---|---|
| Empty | Email, Password and Make account | [`CreateAccountForm.tsx`](../../../web/src/components/account/CreateAccountForm.tsx) |
| Busy | `Making your account...`; the button is disabled | [`CreateAccountForm.tsx`](../../../web/src/components/account/CreateAccountForm.tsx) |
| Made | `Your account is made` in a status area | [`CreateAccount.tsx`](../../../web/src/pages/CreateAccount.tsx) |
| Invalid | A message under each invalid field | [`CreateAccountForm.tsx`](../../../web/src/components/account/CreateAccountForm.tsx), [`accounts.ts`](../../../web/src/api/accounts.ts) |
| Taken | `This email address already has an account` next to Email | [`CreateAccountForm.tsx`](../../../web/src/components/account/CreateAccountForm.tsx) |
| Not reachable | `Connect is not reachable right now, try again later` in an alert above the form | [`CreateAccount.tsx`](../../../web/src/pages/CreateAccount.tsx), [`accounts.ts`](../../../web/src/api/accounts.ts) |

### Implementation order

1. Define the one-request boundary in [`client.ts`](../../../web/src/api/client.ts).
2. Translate account outcomes in [`accounts.ts`](../../../web/src/api/accounts.ts).
3. Keep field state and submission state in [`useCreateAccount.ts`](../../../web/src/hooks/useCreateAccount.ts).
4. Build the account-specific form in [`CreateAccountForm.tsx`](../../../web/src/components/account/CreateAccountForm.tsx).
5. Reuse accessible label/input/error wiring through [`FormField.tsx`](../../../web/src/components/FormField.tsx).
6. Register `/create-account` in [`App.tsx`](../../../web/src/App.tsx).
7. Validate the request boundary, page states and accessibility.

### Alternatives and decisions

The form uses native React state instead of a form library. This keeps the first form’s behavior visible and avoids adding a dependency before Connect has repeated form requirements. The API boundary remains separate so the page never calls `fetch` directly and later forms can reuse `request()`.

Field messages are kept beside their fields instead of using one shared message area. This makes it clear which value must be corrected and lets each input reference its own message through `aria-describedby`. The not-reachable message is different: it describes the whole request and therefore appears above the form in a `role="alert"` area.

The reusable boundary is [`FormField`](../../../web/src/components/FormField.tsx), while [`CreateAccountForm`](../../../web/src/components/account/CreateAccountForm.tsx) remains account-specific. A generic Form wrapper was not added because it would only forward a `<form>` element and would not yet provide shared behavior.

## OR3 — stakeholder feedback

This evidence is still outstanding. A teammate and a person outside the team still need to try the form on a phone. The final proof must record each person’s name, date, device/context, exact words, what changed because of the feedback, and what was deliberately not changed. No stakeholder feedback is claimed here yet.

The `Password again` scope question should be raised during review: it appears in the broader design export but is not part of Task 3.6. The decision and the reviewer’s exact feedback should be added here after that review happens.

## OR4 — validation and remaining limits

The implementation includes focused tests for the request boundary and page behavior:

- [`client.test.ts`](../../../web/src/api/client.test.ts) covers 201, 400, 409, 500, 502, 503 and a failed fetch.
- [`CreateAccount.test.tsx`](../../../web/src/pages/CreateAccount.test.tsx) covers labels, autocomplete, hidden password input, the busy button, made, invalid, taken and not-reachable states, password clearing and axe output.
- [`axe.ts`](../../../web/src/test/axe.ts) provides the accessibility check used by the page test.

`npm run build` passed during implementation. The local Vitest process currently hangs before producing a test summary, so this proof does not claim that the frontend test suite is green yet. A completed CI run must be attached before delivery.

The tests do not yet cover pressing Enter in the password field while the request is busy. They also do not replace a real 360-pixel and 1280-pixel browser check or phone screenshots of the success and taken states.

## OR5 — privacy, security and accessibility

The page protects the person in several concrete ways:

- Password is rendered with `type="password"` and is never placed in a message.
- The password is cleared after a successful account creation.
- No localStorage, sessionStorage or URL persistence is used.
- Account creation does not log the person in.
- Labels are visible and associated with their inputs.
- Field errors use `aria-describedby` and `aria-invalid`.
- Request failures are announced through `role="alert"`.
- Successful creation is announced through `role="status"`.

The required 409 message is useful because it tells the person that they should use the existing account instead. It also reveals whether an email address is registered. If the client later prioritises protection against account enumeration, the message should be changed to a neutral response such as “We could not create this account. Try logging in or use another address.” That would be a deliberate privacy trade-off and is not silently changed in this task because the checklist explicitly requires the current 409 wording.

## Evidence checklist

- [x] Task scope and six states written down.
- [x] Implementation and test files linked.
- [x] Privacy and accessibility choices explained.
- [x] `npm run build` passed.
- [ ] Frontend CI test run with CreateAccount tests and axe output.
- [x] 360px and 1280px browser checks.
- [ ] Phone screenshots for success and taken states.
- [ ] Teammate feedback with exact words, name and date.
- [ ] Feedback from a person outside the team with exact words, name and date.
- [ ] Merge request link and Portflow links.

## AI use

AI was used as a writing support tool while working on this document. AI helped draft and organise the wording from decisions and implementation details supplied by the author. The author reviewed the text and remains responsible for the scope, evidence and conclusions.
