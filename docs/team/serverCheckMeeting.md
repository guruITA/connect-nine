# Team meeting: the server check

This is the plan for our next team meeting. It closes the last points of sprint 1 that need all five of us: the check of the server together (#241), with the padlock check of #239, and the line of `dotnet --list-sdks` of every team member (#12).

**When.** The next team meeting. Wesley Oerlemans sets the date at the stand up.
**Who.** Bayon Eerhart, Nick Hoebe, Wesley Oerlemans, Gurpreet Singh and Kelvin van Voorst.
**Bring.** Your own laptop with the ssh key of your GitLab profile, and your phone.

## Before the meeting, each on your own

Bayon, Nick, Gurpreet and Kelvin, two minutes each:

1. Run `dotnet --list-sdks` on your laptop.
2. Paste the line that starts with 10 in a comment on #12. Wesley's line is already there.

You should see a line such as `10.0.401 [...]`. No line that starts with 10? Install the .NET 10 SDK, part 1 of `docs/team/onboarding.md`.

## At the meeting

Every step below ticks a box of #241. The commands are in part 10 of `docs/team/onboarding.md`, The client's server.

### 1. Everyone logs in with their own key

1. Each of us follows Logging in: `ssh <your first name>@<the address in Logging in>`, and compares the fingerprint with the three in the guide.
2. You should see a prompt such as `nick@connect:~$`.
3. Each of us says the date of their first login with their own user. Wesley writes the five dates in the table First login per team member.
4. One of us runs `last -n 20` and `sudo journalctl -u ssh --since "7 days ago" | grep Accepted`. The output goes in the part Who logged in, and it shows our first names.

### 2. The padlock, on a phone and on a laptop

1. Everyone opens the test address of the part The test address, on their phone with wifi off and on their laptop.
2. You should see the padlock in the address bar. The page itself shows an error until Connect is deployed by #267. That is expected, only the padlock counts here.
3. Nick, who reviewed the server work in !34, writes on #239 that he saw the padlock on his phone, with his name and the date.

### 3. A login with a password is refused

1. One of us tries `ssh -o PubkeyAuthentication=no -o PreferredAuthentications=password <your first name>@<the address in Logging in>`.
2. You should see `Permission denied (publickey)`.

### 4. A test user comes and goes

1. An admin follows New team member for the user `checkuser`, with the public key of one of our laptops.
2. That laptop logs in as `checkuser`. You should see `checkuser@connect:~$`.
3. The admin follows Someone leaves for `checkuser`. You should see `Removing user 'checkuser'`.
4. The login as `checkuser` again answers `Permission denied (publickey)`, and `id checkuser` answers `no such user`.

### 5. Someone who did not build it checks #10

Bayon, Gurpreet, Kelvin or Nick, who built none of the tasks of #10, reads its acceptance criteria and ticks each one that holds, with their name and the date in a comment.

### 6. What we stumbled on

Every step where one of us got stuck is written down during the meeting. Afterwards the one who got stuck fixes it in `docs/team/onboarding.md` through a small merge request.

### 7. The handover page

We read `docs/client/serverHandover.md` together, with part 10 of `docs/team/onboarding.md`. They say who has access, which ports are open, the test address and where the steps are. We check that neither holds a password, a key or the number of the server. The client needs no summary of his own, because he does not log in to the server and keeps the Strato customer login.

### 8. The next issues can start

Gurpreet and Nick, who work on #14, and Gurpreet, who works on #23, hear that the server is ready and that they can start.

## After the meeting

Wesley writes one comment on #241 with the date and who was there, ticks the boxes of #241, #239 and #10 that the meeting closed, and fills in the two tables of part 10.

## AI use

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.
