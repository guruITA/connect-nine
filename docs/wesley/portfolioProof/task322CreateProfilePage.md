# Proof: task 7.3 The page Create your profile (#322)

Merge request !69, branch `feature/322-create-profile-page`. Issue: 07 Create a profile (#25). Files: `web/src/pages/CreateProfile.tsx`, `CreateProfile.module.css` and `CreateProfile.test.tsx`, `web/src/api/memberProfiles.ts` and its test, `web/src/App.tsx` and `web/src/vite-env.d.ts`.
Profile: Full Stack Software Developer, level 3. Portflow: Designing and Realising OR1, OR3, OR4 and OR5.

A point that needs someone else is marked TODO with who and what.

## OR1 Designing and building to a plan

School asks: "You design and implement systematically, justify choices in relation to alternatives, and adapt based on new insights."

1. **The four fields and the states.** First name, age band, region and About you. The page has the states empty, busy, wrong fields, already has a profile, not reachable, not logged in and saved. The typed values live in the page itself, so they stay when the server says no or cannot be reached.
2. **The page does not call the server itself.** `web/src/api/memberProfiles.ts` sends the form and turns every answer into one of five outcomes, and the page only shows them. So the page is tested with a fake, and the address is tested on its own.
3. **The alternatives I weighed.** A form library such as react-hook-form lost to plain React state, because four fields do not need it, four of us are new to React and a new package is one more thing to learn and to keep safe. One page for making and changing a profile lost to a page of its own, because the edit page of #64 starts from a saved profile and answers differently.
4. **Log in does not exist yet.** The route `/profile/new` is a plain route until `RequireLogin` of #15 exists, and there is no link from My account yet. Until then the page says who you are with the temporary header of task 7.2, set with `VITE_TEMPORARY_ACCOUNT_ID` in Development only.
- TODO Wesley: what I changed after a teammate used it on a phone.

## OR3 Involving stakeholders and weighing their feedback

School asks: "You proactively involve multiple stakeholders with diverse interests, coordinate systematically, and consciously weigh their feedback in your choices."

- TODO Wesley: the words, with the date and who, of a teammate and of one person outside the team, such as the client on a Friday, who tried the page on a phone, also whether the hints about school and address are clear and not scary. Then what I changed and what I did not, and why.

## OR4 Validating the solution

School asks: "You systematically validate that the solution does what it should, and draw conclusions for improvement."

1. **The tests.** `CreateProfile.test.tsx` checks the three age bands with none chosen, the counter, the busy button, the saved message, the field messages with the focus on the first wrong field, already has a profile, not reachable and not logged in with every value kept, and 0 axe problems on the empty form and with field messages. `memberProfiles.test.ts` checks the body and headers that are sent and every answer. 40 tests of the screen pass, and lint, Prettier and the build are clean.
2. **In a real browser against the real server.** With the address of task 7.2 running in Development and the temporary account, I filled the page in Chrome at 360 and 1280 pixels wide. An empty form with a bio of 281 characters got the four messages of the server under their fields, with the focus on First name. A valid form saved the profile, with `" Robin "` stored as `Robin`, and a second save showed You already have a profile. axe in the browser, colour contrast included, found 0 problems at both widths in every state.
3. **A test broken on purpose.** With the line that moves the focus to First name taken out, the test of the field messages failed, and it passed again once the line was put back.
4. **What I changed after the first run.** An answer that is not JSON now counts as not reachable instead of breaking the page, the counter says 1 character left instead of 1 characters left, and the Save button is as wide as the form, as in the Figma frame.
5. **What is not covered yet.** 500 characters pasted into About you: the counter goes below zero and the server refuses it with its message, but no test of the page types that much. A message under a field is bold navy text, because the colour tokens have no colour for an error yet.

![The empty form at 360 pixels](images/task322EmptyForm360.png)
![The field messages of the server at 360 pixels](images/task322FieldMessages360.png)
![The empty form at 1280 pixels](images/task322EmptyForm1280.png)
![The field messages at 1280 pixels](images/task322FieldMessages1280.png)
![Your profile is saved](images/task322Saved1280.png)
![You already have a profile](images/task322AlreadyHasProfile1280.png)

## OR5 Impact on users and society

School asks: "In your design and implementation, you consciously take into account the broader impact on end users and society, while forming your own ethical judgment."

1. **How the page protects a minor.** Three age bands and no field for a date or an age, a region that asks for a town or area, and the hint "Do not write your school, address or phone number here." right under About you.
- TODO Wesley: my own judgment on whether the region should be free text, a list of a few larger areas, or not asked for the youngest band, because with a first name and an age band it can make a 15 year old easy to find in a small village.

## TODO

- [ ] Wesley: the TODO lines above.
- [ ] Wesley: after the merge, tick the boxes of #322 with the evidence in a comment.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.
