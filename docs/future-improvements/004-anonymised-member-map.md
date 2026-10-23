# 004 - An anonymised map of members per place

## Context

In the meeting of 23 October 2026 the client named a possible later extension: a map that shows where Connect members live and how many live in each place. It is an idea, not a requirement, and nothing of it is planned this semester.

It only becomes possible once Region is a fixed list of places (#1553, question 19 of `docs/client/clientQuestions.md`), because a map needs every member's place written the same way.

## Possible approach

- The map shows one number per place from the fixed list, never a single member, a name or a pin.
- A place with only a few members shows no number, so nobody can work out who lives there. The team picks the lowest number together with the client.
- The numbers are counted on the server from the table profile, and the screen only gets the counts.

## Points to decide first

- Who may see the map: every visitor, logged in members, or only the foundation.
- The lowest number per place that is still shown, so the map stays anonymous for members under 18.
- Whether the privacy statement has to mention the map.

## Recommendation

Wait until #1553 is built and the list of places is fixed, then ask the client who the map is for before the team writes an issue for it.
