# Getting into the server, after the handover

This page is for the WailSalutem Foundation and for whoever looks after the server of Connect after the project ends in January 2027. It says how a person gets into the server, what to do when nobody with access is left, and what changes at the handover. The technical details are in part 10 of [onboarding.md](../team/onboarding.md).

No password, key or server number is on this page. Those pass in person or through a password manager, never by mail, chat, Git or GitLab.

## The server in short

The server belongs to the foundation. It is a Strato VPS in a data centre in Germany, and it is managed through the foundation's own Strato customer login. The machine, its address and the programs on it are listed in part 10 of [onboarding.md](../team/onboarding.md), section The machine.

## The normal way in: a personal login with a key

Every person who works on the server has a login of their own, and that login opens only with a key. A key is a small pair of files on a laptop. The private half stays on the laptop like a house key, and the public half is put on the server like the lock that fits it.

A login with a password over the internet is refused, for everyone. The server can be reached from the whole internet, and computers try passwords on servers like this all day. A key cannot be guessed. The team decided this for security (issue #237), and the foundation checks it in issue #10: "I see that the server refuses a login with a password".

How a new person gets in:

1. They make a key on their own laptop and send the public half, the line that starts with `ssh-ed25519`. Section Logging in of part 10 shows how.
2. Someone who already has access adds them with the steps of section New team member.
3. They log in with `ssh <their name>@<the address of the server>`.

Today all five team members have access, and each of them can add a new person.

## Without a key: the rescue system of Strato

When nobody with access is left, the foundation can always get in through its Strato customer login. The server restarts for this, so the site is offline for a few minutes. These steps follow Strato's own guide for its Linux VPS (Strato, n.d.-a).

1. Log in to the Strato customer login with the customer number and password of the foundation, choose the server and click **Rettungssystem starten** (start the rescue system). Confirm.
2. When the rescue system has started, click **VNC-Konsole öffnen** (open the console). A window with the server's screen opens in the browser, without a password. Press Enter at the first choice, choose US English and press Enter again.
3. Open the terminal behind the window and type these four lines. They open the disk of the server, so the next commands change the real server:

   ```
   sudo mkdir /media/vda1
   sudo mount /dev/vda1 /media/vda1
   cd /media/vda1
   sudo chroot . /bin/bash
   ```

4. Give a person access again with the steps of section New team member in part 10, without the word `sudo` in front, because you are root here already. Add a line for them in `/etc/sudoers.d/20-connect-admins` if they must manage the server.
5. Type `exit`, go back to the Strato customer login and click **Rettungssystem stoppen** (stop the rescue system). The server starts normally, and the person logs in with their key.

## The root password

The server's administrator account, root, has a password. It came with the server, and the team received the first login from the foundation on 2 October 2026. Root cannot log in over the internet, so this password alone lets nobody in from outside. It only works on a console of the server.

At the handover the foundation sets a new root password, so that nobody from the team knows it any more. Two ways:

- In the rescue system, after step 3 above, type `passwd` and enter the new password twice (Strato, n.d.-a).
- If the Strato customer login shows Server Configuration, Server Data with a button for a new root password, that works without a restart. Strato then shows the new password on that screen (Strato, n.d.-b).

## At the handover

The handover session gets a date before the last sprint review, see [deliveryPlan.md](../project/deliveryPlan.md). In that session:

1. The foundation logs in to Strato and finds the server and the button Rettungssystem starten, without starting it.
   - TODO team at the handover: write the server class Strato shows (for example VC) and the exact menu path here, with the date. Strato's guide for the rescue system holds for the classes VC and VDS.
2. The foundation sets a new root password, see The root password.
3. The foundation decides per team member whether they keep access. Whoever leaves is removed with section Someone leaves in part 10, including their line in the sudo file.
4. When GitLab no longer puts new versions on the server, the key of the user deploy is removed from `/home/deploy/.ssh/authorized_keys`, and the variables SSH_PRIVATE_KEY, SSH_KNOWN_HOSTS and TEST_SERVER_HOST are deleted in GitLab.
5. The person who looks after the server next gets a login of their own with their key, through section New team member.

A login with a password over the internet stays off after the handover. Turning it on is a choice for the foundation together with whoever looks after the server then, because the acceptance criterion in issue #10 says the server refuses it.

## References

Strato. (n.d.-a). *Das Rettungssystem Ihres STRATO Linux VPS/VDS* (FAQ 3793). Retrieved October 10, 2026, from https://www.strato.de/faq/server/das-rettungssystem-ihres-strato-linux-vps-vds/

Strato. (n.d.-b). *How do I reset the root password for my server?* (FAQ 919). Retrieved October 10, 2026, from https://www.strato-hosting.co.uk/faq/product/how-do-I-reset-the-root-password-for-my-server/

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.
